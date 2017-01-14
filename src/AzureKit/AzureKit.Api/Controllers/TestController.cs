using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;

namespace AzureKit.Api.Controllers
{
    [MobileAppController]
    public class TestController : ApiController
    {
        /// <summary>
        /// After logging in, you can invoke this action to 
        /// view the claims associated with your logged in user.
        /// This is for debugging purposes only.
        /// </summary>
        /// <returns></returns>
        // GET api/Test
        [Authorize]
        public Task<IHttpActionResult> Get()
        {
            List<string> claims =
                new List<string>();

            if (User.Identity != null
                && User.Identity is System.Security.Claims.ClaimsIdentity)
            {
                System.Security.Claims.ClaimsIdentity identity =
                    User.Identity as System.Security.Claims.ClaimsIdentity;
                foreach (var claim in identity.Claims)
                {
                    claims.Add(System.String.Format("{0} -> {1}",
                        claim.Type,
                        claim.Value));
                }
            }

            IHttpActionResult result = Ok(claims);
            return Task.FromResult(result);
        }
    }
}