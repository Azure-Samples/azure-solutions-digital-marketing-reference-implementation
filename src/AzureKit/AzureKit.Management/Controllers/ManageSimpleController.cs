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
            var model = await base.GetListOfContentItemsAsync(ContentType.Simple);

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
            try
            {
                var savedItem = await base.CreateContentModelAsync<SimpleContent>(model);
                return View("Confirm", savedItem);
            }
            catch (ContentIdAlreadyExistsException)
            {
                ModelState.AddModelError("Id", "Content with this id already exists");
                return View("Create", model);
            }
        }

        // GET: Manage/ManageSimple/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var model = await base.GetContentModelAsync<SimpleContent>(id);
            return View(model);
        }

        // POST: Manage/ManageSimple/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, SimpleContent model)
        {
            var updatedModel = await base.SaveContentModelAsync<SimpleContent>(model);
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
            await base.DeleteItemAsync(id);
            return RedirectToRoute("ManageContent");
        }
    }
}
