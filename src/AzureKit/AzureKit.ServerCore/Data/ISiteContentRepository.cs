using AzureKit.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureKit.Data
{
    public interface ISiteContentRepository
    {
        Task<T> CreateContentAsync<T>(T model) where T : Models.ContentModelBase;
        Task<T> SaveContentAsync<T>(T model) where T : Models.ContentModelBase;

        Task<ContentModelBase> GetContentAsync(string contentId);

        Task<List<ContentModelBase>> GetMobileContentAsync();

        Task DeleteContentAsync(string id);

        Task<List<ContentItemDescriptor>> GetListOfItemsAsync(string itemType);

        Task<List<ContentItemDescriptor>> GetListItemsAsync(string listId);

        Task<List<ListItemContent>> GetListItemsWithSummaryAsync(string listId);

        Task AddItemToGalleryAsync(string galleryId, Models.MediaItemModel item);

        Task RemoveItemFromGalleryAsync(string galleryId, string itemMediaId);
    }
}
