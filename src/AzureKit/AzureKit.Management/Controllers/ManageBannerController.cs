using AzureKit.Config;
using AzureKit.Data;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AzureKit.Areas.Manage.Controllers
{
    /// <summary>
    /// Controller to manage the banner content shown on the home page
    /// </summary>
    public class ManageBannerController : BaseManageContentController
    {
        public ManageBannerController(ISiteContentRepository repository, ISiteMapRepository mapRepository) : base(repository, mapRepository)
        {}

        // GET: Manage/ManageBanner
        public async Task<ActionResult> Index()
        {
            //get banner content if it exists
            AzureKit.Models.BannerContent content = await base.GetContentModelAsync<AzureKit.Models.BannerContent>(Constants.KEY_BANNER_CONTENT);

            //if not found, then create an empty model to create a new banner
            if (content == null)
            {
                content = new AzureKit.Models.BannerContent { Expiration = DateTime.UtcNow.AddDays(7) };
            }
            return View(content);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AzureKit.Models.BannerContent model)
        {
            if (ModelState.IsValid)
            {
                //save banner
                var savedBanner = await base.SaveContentModelAsync<AzureKit.Models.BannerContent>(model);
                //redirect to confirm
                return View("Confirm");           
            }
            else
                return View(model);
        }
    }
}