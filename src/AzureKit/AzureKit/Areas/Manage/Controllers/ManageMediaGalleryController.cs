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
        private IMediaStorage media;

        public ManageMediaGalleryController(ISiteContentRepository repository, ISiteMapRepository mapRepository, IMediaStorage mediaRepository ): base(repository, mapRepository)
        {
            media = mediaRepository;
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
                model.BaseUrl = media.MediaBaseAddress;
                return View(model);
            }
        }

        // POST: Manage/ManageMediaGallery/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, AzureKit.Models.MediaGalleryContent model)
        {
            try
            {
                var updatedModel = await base.SaveContentModelAsync<AzureKit.Models.MediaGalleryContent>(model);
                updatedModel.BaseUrl = media.MediaBaseAddress;
                return View(updatedModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error saving gallery content - {0}", ex.Message);
                return View("Error");
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
            try
            {
                await base.DeleteItemAsync(id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error deleting gallery content - {0}", ex.Message);
                return View("Error");
            }
        }
    }
}
