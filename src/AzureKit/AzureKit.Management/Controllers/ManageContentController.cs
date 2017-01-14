using System.Web.Mvc;

namespace AzureKit.Areas.Manage.Controllers
{
    /// <summary>
    /// Landing page for management
    /// </summary>
    [Authorize]
    public class ManageContentController : Controller
    {
        // GET: Manage/ManageContent
        public ActionResult Index()
        {
            return View();
        }
    }
}