using AzureKit.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AzureKit.Areas.Manage.Controllers
{
    /// <summary>
    /// Manages the items that are part of a list
    /// </summary>
    public class ManageListItemController : BaseManageContentController
    {
        public ManageListItemController(ISiteContentRepository repository, ISiteMapRepository mapRepository) :base(repository,mapRepository)
        {}

        // GET: Manage/ManageListItem
        public async Task<ActionResult> Index(string id)
        {
            //not a specific list so get a list of lists to choose one
            if (String.IsNullOrEmpty(id))
            {
                var model = await base.GetListOfContentItemsAsync(AzureKit.Models.ContentType.ListLanding);

                return View(model);
            }
            else
            {
                //otherwise get a specific list to edit
                var listItemsModel = await base.GetListItemsAsync(id);
                return View("List",listItemsModel);
            }
        }

       

        // GET: Manage/ManageListItem/Create
        public async Task<ActionResult> Create()
        {
            await PopulateLandingPageListItemsAsync();
            return View();
        }

        private async Task PopulateLandingPageListItemsAsync()
        {
            var model = await base.GetListOfContentItemsAsync(AzureKit.Models.ContentType.ListLanding);
            List<SelectListItem> landingSelections = new List<SelectListItem>();
            foreach (var landingPage in model)
            {
                landingSelections.Add(new SelectListItem { Text = landingPage.Title, Value = landingPage.Id });
            }
            ViewBag.LandingPages = landingSelections;

        }

        // POST: Manage/ManageListItem/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AzureKit.Models.ListItemContent model)
        {
            if (ModelState.IsValid)
            {
                await base.CreateContentModelAsync<AzureKit.Models.ListItemContent>(model);
                return View("Confirm");
            }
            else
            {
                return View();
            }
        }

        // GET: Manage/ManageListItem/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            await PopulateLandingPageListItemsAsync();

            var model = await base.GetContentModelAsync<AzureKit.Models.ListItemContent>(id);

            return View(model);
        }

        // POST: Manage/ManageListItem/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, AzureKit.Models.ListItemContent model)
        {
            if (ModelState.IsValid)
            {
                await base.SaveContentModelAsync<AzureKit.Models.ListItemContent>(model);
                return View("Confirm");
            }
            else
            {
                return View();
            }
        }

        // GET: Manage/ManageListItem/Delete/5
        public ActionResult Delete(string id)
        {
            return View();
        }

        // POST: Manage/ManageListItem/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, AzureKit.Models.ListItemContent model)
        {
            await base.DeleteItemAsync(id);
            return View("Confirm");
        }
    }
}
