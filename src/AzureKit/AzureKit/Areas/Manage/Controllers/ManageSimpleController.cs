using AzureKit.Data;
using AzureKit.Models;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AzureKit.Areas.Manage.Controllers
{
    /// <summary>
    /// Manages simple content 
    /// </summary>
    public class ManageSimpleController : BaseManageContentController
    {

        public ManageSimpleController(Data.ISiteContentRepository repo, ISiteMapRepository mapRepository) : base(repo, mapRepository)
        { }
        public async Task<ActionResult> Index()
        {
            var model = await base.GetListOfContentItemsAsync(ContentType.Simple).ConfigureAwait(false);

            return View(model);
        }
        // GET: Manage/ManageSimple/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Manage/ManageSimple/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SimpleContent model)
        {
            var savedItem = await base.SaveContentModelAsync<SimpleContent>(model).ConfigureAwait(false);
            return View("Confirm", savedItem);
        }

        // GET: Manage/ManageSimple/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var model = await base.GetContentModelAsync<SimpleContent>(id).ConfigureAwait(false);
            return View(model);
        }

        // POST: Manage/ManageSimple/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, SimpleContent model)
        {
            var updatedModel = await base.SaveContentModelAsync<SimpleContent>(model).ConfigureAwait(false);
            return View("Confirm", updatedModel);
        }

        // GET: Manage/ManageSimple/Delete/5
        public ActionResult Delete(string id)
        {
            return View();
        }

        // POST: Manage/ManageSimple/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, SimpleContent model)
        {
            await base.DeleteItemAsync(id).ConfigureAwait(false);
            return RedirectToRoute("ManageContent");
        }
    }
}
