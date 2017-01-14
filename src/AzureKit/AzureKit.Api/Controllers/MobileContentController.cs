using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using AzureKit.Data;
using Microsoft.Azure.Mobile.Server.Config;

namespace AzureKit.Controllers.Mobile
{
    /// <summary>
    /// Mobile API for mobile clients to get data to present
    /// </summary>
    [MobileAppController]
    public class MobileContentController : ApiController
    {
        // GET api/MobileContent
        public async Task<IHttpActionResult> Get()
        {
            //because mobile api controllers do not 
            //participate in the same DI as regular MVC/Web API 
            //controllers
            ISiteContentRepository repo;
            repo = (ISiteContentRepository)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ISiteContentRepository));
            try
            {
                var mobileContent = await repo.GetMobileContentAsync();

                return base.Ok<List<Models.ContentModelBase>>(mobileContent);
                   
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message);
                return InternalServerError(ex);
            }
        }
    }
}
