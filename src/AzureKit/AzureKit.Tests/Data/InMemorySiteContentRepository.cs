using AzureKit.Data;
using AzureKit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureKit.Tests.Data
{
    class InMemorySiteContentRepository : AzureKit.Data.ISiteContentRepository
    {
        Dictionary<string, ContentModelBase> _content = new Dictionary<string, ContentModelBase>();

        public Task AddItemToGalleryAsync(string galleryId, MediaItemModel item)
        {
            ContentModelBase gallery;
            if (_content.TryGetValue(galleryId, out gallery))
            {
                ((MediaGalleryContent)gallery).Items.Add(item);
            }
            return Task.FromResult(0);
        }

        public Task DeleteContentAsync(string id)
        {

            _content.Remove(id.ToLower());
            return Task.FromResult(0);
        }

        public Task<ContentModelBase> GetContentAsync(string contentId)
        {
            string key = contentId.ToLower();
            ContentModelBase result;
            _content.TryGetValue(key, out result);
            return Task.FromResult(result);
        }

        public Task<List<ContentItemDescriptor>> GetListItemsAsync(string listId)
        {
            var result = (from item in _content
                          where item.Value.ContentType == ContentType.ListItem
                          select item).Cast<ListItemContent>().Where(li => li.ListLandingId == listId).Select(li => li); ;
            var descriptors = from r in result
                              select new ContentItemDescriptor { Id = r.Id, Title = r.Title };
            return Task.FromResult(descriptors.ToList());
        }

        public Task<List<ListItemContent>> GetListItemsWithSummaryAsync(string listId)
        {
            var result = (from item in _content
                          where item.Value.ContentType == ContentType.ListItem
                          select item.Value).Cast<ListItemContent>().Where(li => li.ListLandingId == listId).Select(li => li); ;
            
            return Task.FromResult(result.ToList());
        }

        public Task<List<ContentItemDescriptor>> GetListOfItemsAsync(string itemType)
        {
            var result = from item in _content
                         where item.Value.ContentType.ToString() == itemType
                         select new ContentItemDescriptor {
                             Id = item.Value.Id,
                             Title = item.Value.Title };

            return Task.FromResult(result.ToList());
        }

        public Task<List<ContentModelBase>> GetMobileContentAsync()
        {
            var result = from item in _content
                         where item.Value.AvailableOnMobileApps == true
                         select item.Value;

            return Task.FromResult(result.ToList());
        }

        public Task RemoveItemFromGalleryAsync(string galleryId, string itemMediaUrl)
        {
            ContentModelBase gallery;
            if(_content.TryGetValue(galleryId, out gallery))
            {
                ((MediaGalleryContent)gallery).Items.RemoveAll((mi) => mi.MediaUrl == itemMediaUrl);
            }

            return Task.FromResult(0);
        }

        public Task<T> CreateContentAsync<T>(T model) where T : ContentModelBase
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = Guid.NewGuid().ToString();
            }

            string key = model.Id.ToLower();
            if (_content.ContainsKey(key))
            {
                throw new ContentIdAlreadyExistsException(model.Id);
            }

            _content.Add(key, model);

            return Task.FromResult(model);
        }

        public Task<T> SaveContentAsync<T>(T model) where T : ContentModelBase
        {
            string key = model.Id.ToLower();

            _content[key] = model;

            return Task.FromResult(model);
        }
    }
}
