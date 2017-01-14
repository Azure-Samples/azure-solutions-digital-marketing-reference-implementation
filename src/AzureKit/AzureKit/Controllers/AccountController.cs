using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace AzureKit.Controllers
{
    /// <summary>
    /// Controller to manage sign in and out
    /// </summary>
    public class AccountController : Controller
    {
        public void SignIn()
        {
            // Send an OpenID Connect sign-in request.
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        public void SignOut()
        {
            string callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);

            HttpContext.GetOwinContext().Authentication.SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
        }

        public ActionResult SignOutCallback()
        {
            if (Request.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// After logging in, you can navigate to this action to 
        /// view the claims associated with your logged in user.
        /// This is for debugging purposes only.
        /// </summary>
        /// <returns></returns>

        [Authorize]
        public ActionResult Test()
        {
            List<string> claims =
                new List<string>();
             
            if(User.Identity != null 
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

            return View(claims);
        }
    }
}
