using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AzureKit.Areas.Manage.Controllers
{
    /// <summary>
    /// Manages the creation of lists of content (e.g. news, events)
    /// </summary>
    public class ManageListController : BaseManageContentController
    {
        public ManageListController(Data.ISiteContentRepository repository, Data.ISiteMapRepository mapRepository) :base(repository, mapRepository)
        { }

        // GET: Manage/ManageList
        public async Task<ActionResult> Index()
        {
            var model = await base.GetListOfContentItemsAsync(AzureKit.Models.ContentType.ListLanding);
            return View(model);
        }

        // GET: Manage/ManageList/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Manage/ManageList/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Manage/ManageList/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AzureKit.Models.ListLandingContent model)
        {       
            var result = await base.CreateContentModelAsync<AzureKit.Models.ListLandingContent>(model);
            return View("Confirm"); 
        }

        // GET: Manage/ManageList/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var model=  await base.GetContentModelAsync<AzureKit.Models.ListLandingContent>(id);
            return View(model);
        }

        // POST: Manage/ManageList/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, AzureKit.Models.ListLandingContent model)
        {
            var result = await base.SaveContentModelAsync<AzureKit.Models.ListLandingContent>(model);
            return View("Confirm");    
        }

        // GET: Manage/ManageList/Delete/5
        public ActionResult Delete(string id)
        {
            return View();
        }

        // POST: Manage/ManageList/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, AzureKit.Models.ListLandingContent model)
        {
            await base.DeleteItemAsync(id);
            return View("Confirm");    
        }
    }
}
