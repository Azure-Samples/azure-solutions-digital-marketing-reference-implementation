using AzureKit.Data;
using Microsoft.Azure.Mobile.Server.Config;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace AzureKit.Controllers.Mobile
{
    /// <summary>
    /// Mobile API for mobile clients to get data to present
    /// </summary>
    [MobileAppController]
    public class MobileContentController : ApiController
    {
        // GET api/MobileContent
        public async Task<List<Models.ContentModelBase>> Get()
        {
            //because mobile api controllers do not 
            //participate in the same DI as regular MVC/Web API 
            //controllers
            ISiteContentRepository repo;
            repo = (ISiteContentRepository)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ISiteContentRepository));
            try
            {
                var mobileContent = await repo.GetMobileContentAsync();

                return mobileContent;
            }
            catch(Exception ex)
            {
                //return a collection that the mobile client can
                //use to show the error
                System.Diagnostics.Trace.TraceError(ex.Message);

                return new List<Models.ContentModelBase> {
                    new Models.ContentModelBase {
                        Title = "Error loading mobile content" }};
            }
        }
    }
}
