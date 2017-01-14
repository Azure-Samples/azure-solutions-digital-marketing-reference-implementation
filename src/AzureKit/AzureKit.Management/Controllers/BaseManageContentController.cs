using AzureKit.Data;
using AzureKit.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AzureKit.Areas.Manage.Controllers
{
    /// <summary>
    /// Base controller for all content management.
    /// Provides access to common functions for content and site maps
    /// </summary>
    [Authorize]
    public class BaseManageContentController : Controller
    {
        ISiteContentRepository _repo;
        ISiteMapRepository _mapRepo;

        public BaseManageContentController(ISiteContentRepository repository, ISiteMapRepository siteMapRepository)
        {
            _repo = repository;
            _mapRepo = siteMapRepository;
        }

        /// <summary>
        /// Get the specified conten item from the content store
        /// </summary>
        /// <typeparam name="T">The type of content item being requested</typeparam>
        /// <param name="contentId">The id of the item being requested.</param>
        /// <returns>A single content item.</returns>
        protected async Task<T> GetContentModelAsync<T>(string contentId) where T : AzureKit.Models.ContentModelBase
        {
            var content = await _repo.GetContentAsync(contentId);
            return (T)content;
        }

        /// <summary>
        /// Add the given content item to the content store
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        protected async Task<T> CreateContentModelAsync<T>(T content) where T : AzureKit.Models.ContentModelBase
        {
            var response = await _repo.CreateContentAsync(content);
            return (T)response;
        }

        /// <summary>
        /// Save the given content item to the content store
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        protected async Task<T> SaveContentModelAsync<T>(T content) where T : AzureKit.Models.ContentModelBase
        {
            var response = await _repo.SaveContentAsync(content);
            return (T)response;
        }

        /// <summary>
        /// Deletes the content item with the specified id
        /// </summary>
        /// <param name="id">The id of the item to delete</param>
        /// <returns></returns>
        protected async Task DeleteItemAsync(string id)
        {
            await _repo.DeleteContentAsync(id);

            return;
        }

        /// <summary>
        /// Returns a list of items of a particular type. This is used for management pages only.
        /// </summary>
        /// <param name="type">The type of content items to query.</param>
        /// <returns>A collection of ContentItemDescriptor for each item of the given type.</returns>
        protected async Task<List<ContentItemDescriptor>> GetListOfContentItemsAsync(ContentType type)
        {
            var foundItems = await _repo.GetListOfItemsAsync(type.ToString());
            foreach (var item in foundItems)
            {
                item.IsInSiteMap = await _mapRepo.IsItemInSiteMapAsync(item.Id);
            }

            return foundItems;
        }

        /// <summary>
        /// Gets a descriptive list of items from a "list" content type.
        /// </summary>
        /// <param name="listId">The id of the list to which the items should belong</param>
        /// <returns>A collection of ContentItemDescriptor for each item in the list</returns>
        protected async Task<List<ContentItemDescriptor>> GetListItemsAsync(string listId)
        {
            var foundItems = await _repo.GetListItemsAsync(listId);
            foreach (var item in foundItems)
            {
                item.IsInSiteMap = await _mapRepo.IsItemInSiteMapAsync(item.Id);
            }

            return foundItems;
        }

        /// <summary>
        /// removes the item with the given ID from the sitemap
        /// </summary>
        /// <param name="id">the id of the item to remove</param>
        /// <returns></returns>
        public async Task<ActionResult> Remove(string id)
        {
            await _mapRepo.RemoveItemFromSiteMapAsync(id);

            return View("SiteMapConfirm");
        }

        /// <summary>
        /// Adds the given item to the sitemap with the name as the display value
        /// </summary>
        /// <param name="id">The unique id of the content item to which the map will point</param>
        /// <param name="name">The display name to appear in the navigation</param>
        /// <returns></returns>
        public async Task<ActionResult> Add(string id, string name)
        {
            var entry = new SiteMapEntry { ContentIdentifier = id, Title = name };
            await _mapRepo.AddItemToSiteMapAsync(entry);

            return View("SiteMapConfirm");
        }
    }
}