using System.Threading.Tasks;
using System.Web.Mvc;

namespace AzureKit.Controllers
{
    /// <summary>
    /// This controller is responsible for rendering the dynamic content
    /// loaded from the content repository/cache.
    /// Based on the type of the content, one of several views will be rendered.
    /// </summary>
    public class ContentController : BaseController
    {
        Data.ISiteContentRepository repo;
        Media.IMediaStorage media;

        public ContentController(Data.ISiteContentRepository contentRepository, Data.ISiteMapRepository mapRepository, Media.IMediaStorage mediaStore) : base(mapRepository)
        {
            repo = contentRepository;
            media = mediaStore;
        }
        // GET: Content
        public async Task<ActionResult> Index(string id)
        {
            //Get Content
            var content = await repo.GetContentAsync(id);
            
            if(content == null)
            {
                return View("Missing");
            }
            
            //for landing pages, get the top items for the list
            if(content.ContentType == Models.ContentType.ListLanding)
            {
                var listItems = await repo.GetListItemsWithSummaryAsync(content.Id);
                ((Models.ListLandingContent)content).Items = listItems;
            }

            //for media galleries, setup the model with the base url for the storage provider
            if(content.ContentType == Models.ContentType.MediaGallery)
            {
                ((Models.MediaGalleryContent)content).BaseUrl = media.MediaBaseAddress;
            }
            //Create view based on content type
            return View(content.ContentType.ToString(), content);
        }
    }
}