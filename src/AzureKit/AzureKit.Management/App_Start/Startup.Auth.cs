using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AzureKit.Management
{
    public partial class Startup
    {
        private static string s_clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string s_aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string s_tenantId = ConfigurationManager.AppSettings["ida:TenantId"];

        public static readonly string Authority = s_aadInstance + s_tenantId;

        // This is the resource ID of the AAD Graph API.  We'll need this to request a token to call the Graph API.
        //string graphResourceId = "https://graph.windows.net";

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                LoginPath = new PathString("/Account/SignIn"),
                Provider = new CookieAuthenticationProvider
                {
                    OnApplyRedirect = ctx =>
                    {
                        // By default the cookie auth provider redirects to the login page. That's fine for
                        // interactive use, but for API access it's not good, because clients see a 200 with
                        // an HTML body they won't be able to comprehend when what we should be telling them
                        // is 401. So we only do the redirect that the cookie auth provider wants to do if
                        // the request is not for an API endpoint.
                        var absPath = ctx.Request.Uri.AbsolutePath;
                        if (!absPath.StartsWith(VirtualPathUtility.ToAbsolute("~/api/")) &&
                            !absPath.StartsWith(VirtualPathUtility.ToAbsolute("~/mobile/"))
                            )
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                    }
                }
            });
            //only try to setup AAD auth if the configuration was ready
            if (!string.IsNullOrEmpty(s_clientId) &&
                !string.IsNullOrEmpty(Authority))
            {
                // When running in an Azure Web App, the WEBSITE_HOSTNAME environment variable
                // tells us our hostname. If it's not present, we're most likely debugging locally.
                string hostName = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
                string rootUri = string.IsNullOrWhiteSpace(hostName)
                    ? "https://localhost:26614/"
                    : "https://" + hostName + "/";

                app.UseOpenIdConnectAuthentication(
                    new OpenIdConnectAuthenticationOptions
                    {
                        ClientId = s_clientId,
                        Authority = Authority,
                        PostLogoutRedirectUri = rootUri,
                        RedirectUri = rootUri,
                        Notifications = new OpenIdConnectAuthenticationNotifications
                        {
                            AuthenticationFailed = (context) => {

                                context.HandleResponse();
                                context.Response.Redirect("/Account/Error");
                                return Task.CompletedTask;
                            }
                        }
                    });
            }
        }
    }
}
