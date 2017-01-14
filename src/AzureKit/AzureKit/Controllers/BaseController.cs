using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace AzureKit.Controllers
{
    /// <summary>
    /// The base controller is resonsible for managing the dynamic
    /// site map information and making sure it is present on each request.
    /// 
    /// </summary>
    public class BaseController : Controller
    {
        Data.ISiteMapRepository _siteMapRepo;

        public BaseController(Data.ISiteMapRepository repository)
        {
            _siteMapRepo = repository;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            LoadSiteMap();
        }

        protected void LoadSiteMap()
        {
            var map = Task.Run<Models.SiteMap>(() => _siteMapRepo.GetMapAsync()).GetAwaiter().GetResult();
            ViewBag.SiteMap = map;
        }
    }
}