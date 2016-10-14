using AzureKit.Data;
using AzureKit.Media;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AzureKit.Areas.Manage.Controllers
{
    /// <summary>
    /// Responsible for the actions that manage creating and editing a gallery
    /// </summary>
    public class ManageMediaGalleryController : BaseManageContentController
    {
        private IMediaStorage _media;

        public ManageMediaGalleryController(ISiteContentRepository repository, ISiteMapRepository mapRepository, IMediaStorage mediaRepository ): base(repository, mapRepository)
        {
            _media = mediaRepository;
        }
        // GET: Manage/ManageMediaGallery
        public async Task<ActionResult> Index()
        {
            var model = await base.GetListOfContentItemsAsync(AzureKit.Models.ContentType.MediaGallery);
            return View(model);
        }

        // GET: Manage/ManageMediaGallery/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return View(new AzureKit.Models.MediaGalleryContent());
            }
            else
            {
                var model = await base.GetContentModelAsync<AzureKit.Models.MediaGalleryContent>(id);
                model.BaseUrl = _media.MediaBaseAddress;
                return View(model);
            }
        }

        // POST: Manage/ManageMediaGallery/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, AzureKit.Models.MediaGalleryContent model)
        {
            var saveModel = model;

            //for edit just the metadata and not the items are managed so modify original model
            if(!String.IsNullOrEmpty(id))
            {
                var originalGallery = await base.GetContentModelAsync<AzureKit.Models.MediaGalleryContent>(id);
                originalGallery.AvailableOnMobileApps = model.AvailableOnMobileApps;
                originalGallery.Content = model.Content;
                originalGallery.Title = model.Title;
                saveModel = originalGallery;
            }
            
            var updatedModel = await base.SaveContentModelAsync<AzureKit.Models.MediaGalleryContent>(saveModel);
            updatedModel.BaseUrl = _media.MediaBaseAddress;

            //if this is an Add, then redirect back to Edit with the proper ID
            if(String.IsNullOrEmpty(id))
            {
                return RedirectToAction("Edit", new { Id = updatedModel.Id });
            }
            else
            {
                return View(updatedModel);
            }
        }

        // GET: Manage/ManageMediaGallery/Delete/5
        public ActionResult Delete(string id)
        {
            return View();
        }

        // POST: Manage/ManageMediaGallery/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, AzureKit.Models.MediaGalleryContent model)
        {
            await base.DeleteItemAsync(id);
            return RedirectToAction("Index");
        }
    }
}
