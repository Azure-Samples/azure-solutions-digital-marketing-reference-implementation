using System;
using System.Configuration;
using Microsoft.Owin.Security;
using Owin;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.Cookies;

namespace AzureKit
{
    public partial class Startup
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];

        public static readonly string Authority = aadInstance + tenantId;

        // This is the resource ID of the AAD Graph API.  We'll need this to request a token to call the Graph API.
        //string graphResourceId = "https://graph.windows.net";

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
           
            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            //only try to setup AAD auth if the configuration was ready
            if (!string.IsNullOrEmpty(clientId) &&
                !string.IsNullOrEmpty(Authority) &&
                !string.IsNullOrEmpty(postLogoutRedirectUri))
            {
                // When running in an Azure Web App, the WEBSITE_HOSTNAME environment variable
                // tells us our hostname. If it's not present, we're most likely debugging locally.
                string hostName = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
                string rootUri = string.IsNullOrWhiteSpace(hostName)
                    ? "https://localhost:44300/"
                    : "https://" + hostName + "/";

                app.UseOpenIdConnectAuthentication(
                    new OpenIdConnectAuthenticationOptions
                    {
                        ClientId = clientId,
                        Authority = Authority,
                        PostLogoutRedirectUri = postLogoutRedirectUri,
                        RedirectUri = rootUri
                    });
            }
        }
    }
}
