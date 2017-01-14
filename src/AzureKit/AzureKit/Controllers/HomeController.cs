using System.Threading.Tasks;
using System.Web.Mvc;

namespace AzureKit.Controllers
{
    [OutputCache(CacheProfile = Config.Constants.CACHE_DEFAULT_PROFILE)]
    public class HomeController : BaseController
    {
        private Data.ISiteContentRepository _repo;

        public HomeController(Data.ISiteMapRepository mapRepo, Data.ISiteContentRepository contentRepo) : base(mapRepo)
        {
            _repo = contentRepo;
        }

        
        public async Task<ActionResult> Index()
        {
            // get the banner content if there is any
            Models.BannerContent banner = null;
            try
            {
                banner = await _repo.GetContentAsync(Config.Constants.KEY_BANNER_CONTENT) as Models.BannerContent;
            }
            catch
            {
                //if we can't load, let the user know to make sure and configure
                ViewBag.ErrorMessage = "Error loading data. Make sure you have deployed your resources to Azure and configured the app accordingly.";
            }


            if (banner != null && banner.Expiration > System.DateTime.UtcNow) 
            {
                return View(banner);
            }
            else
            {
                return View();
            }
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}