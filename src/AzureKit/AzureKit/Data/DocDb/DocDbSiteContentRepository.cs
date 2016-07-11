using AutoMapper;
using AzureKit.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using AzureKit.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using AzureKit.Data.DocDb.Models;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Linq;

namespace AzureKit.Data.DocDb
{
    /// <summary>
    /// Document DB implementation of the site content repository to store data
    /// </summary>
    public class DocDbSiteContentRepository : ISiteContentRepository
    {
        private IMappingEngine map;
        private ICacheService cache;
        private DocumentClient docDbClient;
        private const string CACHE_KEY_PREFIX = "DocDbContent:";

        private Config.DocumentDBConfig config;

        public DocDbSiteContentRepository(IMappingEngine mapper, ICacheService cacheService, Config.DocumentDBConfig dbConfig)
        {
            map = mapper;
            cache = cacheService;
            this.config = dbConfig;
            this.docDbClient = config.Client;  
        }

        public async Task<T> SaveContentAsync<T>(T model) where T : ContentModelBase
        {
            var document = GetMappedRequest(model);
            try
            {
                var upsertResponse = await docDbClient.UpsertDocumentAsync(config.SiteContentCollectionUrl, document);

                T mappedResult = GetMappedResult<T>(upsertResponse);

                //update the cache so it has the current content item
                cache.PutItem<ContentModelBase>(CACHE_KEY_PREFIX + mappedResult.Id,  mappedResult);

                return mappedResult;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Could not save content", ex);
            }
        }

        private SiteContentItemDocument GetMappedRequest<T>(T model) where T : ContentModelBase
        {
            switch (model.ContentType)
            {
                case ContentType.Banner:
                    return map.Mapper.Map<BannerContentDocument>(model);
                case ContentType.Simple:
                    return map.Mapper.Map<SiteContentItemDocument>(model);
                case ContentType.ListLanding:
                    return map.Mapper.Map<ListLandingContentDocument>(model);
                case ContentType.ListItem:
                    return map.Mapper.Map<ListDetailContentDocument>(model);
                case ContentType.MediaGallery:
                    return map.Mapper.Map<MediaGalleryContentDocument>(model);
                default:
                    return null;
            }
            
        }

        private T GetMappedResult<T>(ResourceResponse<Document> upsertResponse) where T : ContentModelBase
        {

            string resourceType = upsertResponse.Resource.GetPropertyValue<string>("contentType");
            
            switch (resourceType)
            {
                case "Banner":
                    BannerContentDocument bannerDoc = (dynamic)upsertResponse.Resource;
                    return map.Mapper.Map<T>(bannerDoc);
                case "Simple":
                    SimpleContentDocument simpleDoc = (dynamic)upsertResponse.Resource;
                    return map.Mapper.Map<T>(simpleDoc);
                case "ListLanding":
                    ListLandingContentDocument listDoc = (dynamic)upsertResponse.Resource;
                    return map.Mapper.Map<T>(listDoc);
                case "ListItem":
                    ListDetailContentDocument listItemDoc = (dynamic)upsertResponse.Resource;
                    return map.Mapper.Map<T>(listItemDoc);

                case "MediaGallery":
                    MediaGalleryContentDocument mediaDoc = (dynamic)upsertResponse.Resource;
                    return map.Mapper.Map<T>(mediaDoc);
                default:
                    return null;
            }
            
        }

        public async Task<ContentModelBase> GetContentAsync(string contentId)
        {
            //check cache for content
            var model = cache.GetItem<ContentModelBase>(CACHE_KEY_PREFIX + contentId);
            
            if (model != null)
            {  
                model.Html = new System.Web.Mvc.MvcHtmlString(model.Content);
                return model;
            }
            else {

                SiteContentItemDocument content = null;
                //get content from docdb
                //create document query
                var query = (from cont in docDbClient.CreateDocumentQuery<SiteContentItemDocument>(config.SiteContentCollectionUrl)
                            where cont.Id == contentId
                            select cont).AsDocumentQuery();

                //async request of query results
                var response = await query.ExecuteNextAsync<SiteContentItemDocument>();
                if(response.Count == 1)
                {
                    // we got some data, so populate the content item
                    content = response.FirstOrDefault();
                }
                //if we got content from the db, map to the model type
                if (content != null)
                {
                    ContentModelBase mappedResult;

                    switch (content.ContentType)
                    {
                        case "Banner":
                            BannerContentDocument bannerDoc = (dynamic)content;
                            mappedResult = map.Mapper.Map<BannerContentDocument, BannerContent>(bannerDoc);
                            break;
                        case "Simple":
                            SimpleContentDocument simpleDoc = (dynamic)content;
                            mappedResult = map.Mapper.Map<SimpleContentDocument, SimpleContent>(simpleDoc);
                            break;
                        case "ListLanding":
                            ListLandingContentDocument listDoc = (dynamic)content;
                            mappedResult = map.Mapper.Map<ListLandingContentDocument, ListLandingContent>(listDoc);
                            break;
                        case "ListItem":
                            ListDetailContentDocument listItemDoc = (dynamic)content;
                            mappedResult = map.Mapper.Map<ListDetailContentDocument, ListItemContent>(listItemDoc);
                            break;
                        case "MediaGallery":
                            MediaGalleryContentDocument mediaDoc = (dynamic)content;
                            mappedResult = map.Mapper.Map<MediaGalleryContentDocument, MediaGalleryContent>(mediaDoc);
                            break;
                        default:
                            return null;
                    }

                    //add content to cache
                    cache.PutItem<ContentModelBase>(CACHE_KEY_PREFIX + contentId, mappedResult);
                    return mappedResult;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<List<ContentModelBase>> GetMobileContentAsync()
        {

            List<ContentModelBase> items = new List<ContentModelBase>();
            List<SiteContentItemDocument> dbItems = new List<SiteContentItemDocument>();

            var result = (from item in docDbClient.CreateDocumentQuery<SiteContentItemDocument>(config.SiteContentCollectionUrl)
                         where item.AvailableOnMobileApps == true
                         select item).AsDocumentQuery();

            do
            {
                var pageOfData = await result.ExecuteNextAsync<SiteContentItemDocument>();
                if(pageOfData.Count > 0)
                {
                    dbItems.AddRange(pageOfData);
                }
            } while (result.HasMoreResults);

            items = map.Mapper.Map<List<SiteContentItemDocument>, List<ContentModelBase>>(dbItems);

            return items;

        }
        public async Task DeleteContentAsync(string id)
        {
            await docDbClient.DeleteDocumentAsync(config.SiteContentCollectionUrl + "/docs/" + id);
            cache.PurgeItem(CACHE_KEY_PREFIX + id);
            return;
        }

        public async Task<List<ContentItemDescriptor>> GetListOfItemsAsync(string itemType)
        {
            List<ContentItemDescriptor> items = new List<ContentItemDescriptor>();

            var result = (from item in docDbClient.CreateDocumentQuery<SimpleContentDocument>(config.SiteContentCollectionUrl)
                         where item.ContentType == itemType
                         select item //new { Id = item.Id, Title = item.Title }
                         ).AsDocumentQuery();

            do
            {
                var pagedData = await result.ExecuteNextAsync<SimpleContentDocument>();

                foreach (var item in pagedData)
                {
                    items.Add(new ContentItemDescriptor { Id = item.Id, Title = item.Title });
                }
            } while (result.HasMoreResults);

            return items;
        }

        public async Task<List<ContentItemDescriptor>> GetListItemsAsync(string listId)
        {
            List<ContentItemDescriptor> listItems = new List<ContentItemDescriptor>();

            var items = (from li in docDbClient.CreateDocumentQuery<ListDetailContentDocument>(config.SiteContentCollectionUrl)
                        where li.ListLandingId == listId
                        select li).AsDocumentQuery();

            try {
                do
                {
                    var dbItems = await items.ExecuteNextAsync<ListDetailContentDocument>();

                    foreach (var item in dbItems)
                    {
                        listItems.Add(
                            new ContentItemDescriptor { Id = item.Id, Title = item.Title });
                    }
                } while (items.HasMoreResults);   
            }
            catch
            {}
            return listItems;
        }

        public async Task AddItemToGalleryAsync(string galleryId, MediaItemModel item)
        {
            var query = (from g in docDbClient.CreateDocumentQuery<MediaGalleryContentDocument>(
                config.SiteContentCollectionUrl)
                         where g.Id == galleryId
                         select g).AsDocumentQuery();

            var galleryFeed = await query.ExecuteNextAsync<MediaGalleryContentDocument>();
            var gallery = galleryFeed.FirstOrDefault();

            //copy the list of items and insert the new item
            List<MediaItem> items = new List<MediaItem>(gallery.Items);

            var mappedNewItem = map.Mapper.Map<MediaItem>(item);

            items.Add(mappedNewItem);

            //copy the list back to the array
            gallery.SetPropertyValue("items", items.ToArray());
           
            try
            {
                var upsertResponse = await docDbClient.UpsertDocumentAsync(config.SiteContentCollectionUrl, gallery);

                var updatedGallery = GetMappedResult<MediaGalleryContent>(upsertResponse);
                //cache document
                cache.PutItem<MediaGalleryContent>(CACHE_KEY_PREFIX + gallery.Id, updatedGallery);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to update the gallery metadata", ex);
            }

        }

        public async Task RemoveItemFromGalleryAsync(string galleryId, string itemMediaId)
        {
            var query = (from g in docDbClient.CreateDocumentQuery<MediaGalleryContentDocument>(
                config.SiteContentCollectionUrl)
                         where g.Id == galleryId
                         select g).AsDocumentQuery();

            var galleryFeed = await query.ExecuteNextAsync<MediaGalleryContentDocument>();
            var gallery = galleryFeed.FirstOrDefault();

            List<MediaItem> savedItems = new List<MediaItem>();

            if(gallery!= null && gallery.Items != null)
            {  
                for (int mediaItemIndex = 0; mediaItemIndex < gallery.Items.Length; mediaItemIndex++)
                {
                    if(!(String.Compare(gallery.Items[mediaItemIndex].Id, itemMediaId, true) == 0))
                    {
                        savedItems.Add(gallery.Items[mediaItemIndex]);
                    }
                }

                //copy the non-matching items back to the array
                gallery.SetPropertyValue("items", savedItems.ToArray());

                try
                {
                    var upsertResponse = await docDbClient.UpsertDocumentAsync(config.SiteContentCollectionUrl, gallery);
                    
                    //cache document
                    var updatedGallery = GetMappedResult<MediaGalleryContent>(upsertResponse);
                    cache.PutItem<MediaGalleryContent>(CACHE_KEY_PREFIX + updatedGallery.Id, updatedGallery);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to update the gallery metadata", ex);
                }

            }
        }

        public async Task<List<ListItemContent>> GetListItemsWithSummaryAsync(string listId)
        {
            List<ListItemContent> listItems = new List<ListItemContent>();

            var itemsQuery = (from li in docDbClient.CreateDocumentQuery<ListDetailContentDocument>(config.SiteContentCollectionUrl)
                        where li.ListLandingId == listId
                        select li).AsDocumentQuery();
            try
            {
                do
                {
                    var items = await itemsQuery.ExecuteNextAsync<ListDetailContentDocument>();
                    var tempItems =  map.Mapper.Map<List<ListDetailContentDocument>, List<ListItemContent>>(items.ToList());
                    listItems.AddRange(tempItems);

                } while (itemsQuery.HasMoreResults); 
            }
            catch
            { }
            return listItems;
        }
    }
}