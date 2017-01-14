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

        // GET: Manage/ManageMediaGallery/Create
        public ActionResult Create()
        {
            return View(new AzureKit.Models.MediaGalleryContent());
        }

        // POST: Manage/ManageMediaGallery/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AzureKit.Models.MediaGalleryContent model)
        {
            try
            {
                var updatedModel = await base.CreateContentModelAsync(model);
                //This is an Add, so redirect back to Edit with the proper ID
                return RedirectToAction("Edit", new { Id = updatedModel.Id });
            }
            catch (ContentIdAlreadyExistsException)
            {
                ModelState.AddModelError("Id", "Content with this id already exists");
                return View(model);
            }
        }

        // GET: Manage/ManageMediaGallery/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var model = await base.GetContentModelAsync<AzureKit.Models.MediaGalleryContent>(id);
            model.BaseUrl = _media.MediaBaseAddress;
            return View(model);
        }

        // POST: Manage/ManageMediaGallery/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, AzureKit.Models.MediaGalleryContent model)
        {
            //for edit just the metadata and not the items are managed so modify original model
            var originalGallery = await base.GetContentModelAsync<AzureKit.Models.MediaGalleryContent>(id);
            originalGallery.AvailableOnMobileApps = model.AvailableOnMobileApps;
            originalGallery.Content = model.Content;
            originalGallery.Title = model.Title;
            var updatedModel = await base.SaveContentModelAsync<AzureKit.Models.MediaGalleryContent>(originalGallery);
            updatedModel.BaseUrl = _media.MediaBaseAddress;
            return View(updatedModel);
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
