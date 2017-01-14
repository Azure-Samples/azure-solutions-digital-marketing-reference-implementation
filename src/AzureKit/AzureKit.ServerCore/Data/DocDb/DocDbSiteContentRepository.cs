using AutoMapper;
using AzureKit.Data.DocDb.Models;
using AzureKit.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AzureKit.Data.DocDb
{
    /// <summary>
    /// Document DB implementation of the site content repository to store data
    /// </summary>
    public class DocDbSiteContentRepository : ISiteContentRepository
    {
        private IMappingEngine _map;
        private DocumentClient _docDbClient;

        private Config.IDocumentDBConfig _config;

        public DocDbSiteContentRepository(IMappingEngine mapper, Config.IDocumentDBConfig dbConfig)
        {
            _map = mapper;

            this._config = dbConfig;
            this._docDbClient = _config.Client;  
        }

        public async Task<T> CreateContentAsync<T>(T model) where T : ContentModelBase
        {
            var document = GetMappedRequest(model);
            try
            {
                var upsertResponse = await _docDbClient.CreateDocumentAsync(_config.SiteContentCollectionUrl, document).ConfigureAwait(false);

                T mappedResult = GetMappedResult<T>(upsertResponse);

                return mappedResult;
            }
            catch (DocumentClientException ex)
            when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                throw new ContentIdAlreadyExistsException(model.Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Could not save content", ex);
            }
        }

        public async Task<T> SaveContentAsync<T>(T model) where T : ContentModelBase
        {
            var document = GetMappedRequest(model);
            try
            {
                var upsertResponse = await _docDbClient.UpsertDocumentAsync(_config.SiteContentCollectionUrl, document).ConfigureAwait(false);

                T mappedResult = GetMappedResult<T>(upsertResponse);

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
                    return _map.Mapper.Map<BannerContentDocument>(model);
                case ContentType.Simple:
                    return _map.Mapper.Map<SiteContentItemDocument>(model);
                case ContentType.ListLanding:
                    return _map.Mapper.Map<ListLandingContentDocument>(model);
                case ContentType.ListItem:
                    return _map.Mapper.Map<ListDetailContentDocument>(model);
                case ContentType.MediaGallery:
                    return _map.Mapper.Map<MediaGalleryContentDocument>(model);
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
                    return _map.Mapper.Map<T>(bannerDoc);
                case "Simple":
                    SimpleContentDocument simpleDoc = (dynamic)upsertResponse.Resource;
                    return _map.Mapper.Map<T>(simpleDoc);
                case "ListLanding":
                    ListLandingContentDocument listDoc = (dynamic)upsertResponse.Resource;
                    return _map.Mapper.Map<T>(listDoc);
                case "ListItem":
                    ListDetailContentDocument listItemDoc = (dynamic)upsertResponse.Resource;
                    return _map.Mapper.Map<T>(listItemDoc);

                case "MediaGallery":
                    MediaGalleryContentDocument mediaDoc = (dynamic)upsertResponse.Resource;
                    return _map.Mapper.Map<T>(mediaDoc);
                default:
                    return null;
            }
            
        }

        public async Task<ContentModelBase> GetContentAsync(string contentId)
        {
            SiteContentItemDocument content = null;
            //get content from docdb
            //create document query
            var query = (from cont in _docDbClient.CreateDocumentQuery<SiteContentItemDocument>(_config.SiteContentCollectionUrl)
                        where cont.Id == contentId
                        select cont).AsDocumentQuery();

            //async request of query results
            var response = await query.ExecuteNextAsync<SiteContentItemDocument>().ConfigureAwait(false);
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
                        mappedResult = _map.Mapper.Map<BannerContentDocument, BannerContent>(bannerDoc);
                        break;
                    case "Simple":
                        SimpleContentDocument simpleDoc = (dynamic)content;
                        mappedResult = _map.Mapper.Map<SimpleContentDocument, SimpleContent>(simpleDoc);
                        break;
                    case "ListLanding":
                        ListLandingContentDocument listDoc = (dynamic)content;
                        mappedResult = _map.Mapper.Map<ListLandingContentDocument, ListLandingContent>(listDoc);
                        break;
                    case "ListItem":
                        ListDetailContentDocument listItemDoc = (dynamic)content;
                        mappedResult = _map.Mapper.Map<ListDetailContentDocument, ListItemContent>(listItemDoc);
                        break;
                    case "MediaGallery":
                        MediaGalleryContentDocument mediaDoc = (dynamic)content;
                        mappedResult = _map.Mapper.Map<MediaGalleryContentDocument, MediaGalleryContent>(mediaDoc);
                        break;
                    default:
                        return null;
                }

                return mappedResult;

            }
            else
            {
                return null;
            }
        }

        public async Task<List<ContentModelBase>> GetMobileContentAsync()
        {

            List<ContentModelBase> items = new List<ContentModelBase>();
            List<SiteContentItemDocument> dbItems = new List<SiteContentItemDocument>();

            var result = (from item in _docDbClient.CreateDocumentQuery<SiteContentItemDocument>(_config.SiteContentCollectionUrl)
                         where item.AvailableOnMobileApps == true
                         select item).AsDocumentQuery();

            do
            {
                var pageOfData = await result.ExecuteNextAsync<SiteContentItemDocument>().ConfigureAwait(false);
                if(pageOfData.Count > 0)
                {
                    dbItems.AddRange(pageOfData);
                }
            } while (result.HasMoreResults);

            items = _map.Mapper.Map<List<SiteContentItemDocument>, List<ContentModelBase>>(dbItems);

            return items;

        }
        public async Task DeleteContentAsync(string id)
        {
            await _docDbClient.DeleteDocumentAsync(_config.SiteContentCollectionUrl + "/docs/" + id).ConfigureAwait(false);
        }

        public async Task<List<ContentItemDescriptor>> GetListOfItemsAsync(string itemType)
        {
            List<ContentItemDescriptor> items = new List<ContentItemDescriptor>();

            var result = (from item in _docDbClient.CreateDocumentQuery<SimpleContentDocument>(_config.SiteContentCollectionUrl)
                         where item.ContentType == itemType
                         select item //new { Id = item.Id, Title = item.Title }
                         ).AsDocumentQuery();

            do
            {
                var pagedData = await result.ExecuteNextAsync<SimpleContentDocument>().ConfigureAwait(false);

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

            var items = (from li in _docDbClient.CreateDocumentQuery<ListDetailContentDocument>(_config.SiteContentCollectionUrl)
                        where li.ListLandingId == listId
                        select li).AsDocumentQuery();

                do
                {
                    var dbItems = await items.ExecuteNextAsync<ListDetailContentDocument>().ConfigureAwait(false);
                    if (dbItems != null)
                    {
                        foreach (var item in dbItems)
                        {
                            listItems.Add(
                                new ContentItemDescriptor { Id = item.Id, Title = item.Title });
                        }
                    }
                } while (items.HasMoreResults);   
            
            return listItems;
        }

        public async Task AddItemToGalleryAsync(string galleryId, MediaItemModel item)
        {
            var query = (from g in _docDbClient.CreateDocumentQuery<MediaGalleryContentDocument>(
                _config.SiteContentCollectionUrl)
                         where g.Id == galleryId
                         select g).AsDocumentQuery();

            var galleryFeed = await query.ExecuteNextAsync<MediaGalleryContentDocument>().ConfigureAwait(false);
            var gallery = galleryFeed.FirstOrDefault();

            //copy the list of items and insert the new item
            List<MediaItem> items = new List<MediaItem>(gallery.Items);

            var mappedNewItem = _map.Mapper.Map<MediaItem>(item);

            items.Add(mappedNewItem);

            //copy the list back to the array
            gallery.SetPropertyValue("items", items.ToArray());
           
            try
            {
                var upsertResponse = await _docDbClient.UpsertDocumentAsync(_config.SiteContentCollectionUrl, gallery).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to update the gallery metadata", ex);
            }

        }

        public async Task RemoveItemFromGalleryAsync(string galleryId, string itemMediaId)
        {
            var query = (from g in _docDbClient.CreateDocumentQuery<MediaGalleryContentDocument>(
                _config.SiteContentCollectionUrl)
                         where g.Id == galleryId
                         select g).AsDocumentQuery();

            var galleryFeed = await query.ExecuteNextAsync<MediaGalleryContentDocument>().ConfigureAwait(false);
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
                    var upsertResponse = await _docDbClient.UpsertDocumentAsync(_config.SiteContentCollectionUrl, gallery).ConfigureAwait(false);
                    
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

            var itemsQuery = (from li in _docDbClient.CreateDocumentQuery<ListDetailContentDocument>(_config.SiteContentCollectionUrl)
                        where li.ListLandingId == listId
                        select li).AsDocumentQuery();
            
            do
            {
                var items = await itemsQuery.ExecuteNextAsync<ListDetailContentDocument>().ConfigureAwait(false);
                if (items != null)
                {
                    var tempItems = _map.Mapper.Map<List<ListDetailContentDocument>, List<ListItemContent>>(items.ToList());
                    listItems.AddRange(tempItems);
                }

            } while (itemsQuery.HasMoreResults); 
            
            return listItems;
        }
    }
}