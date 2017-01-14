using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Web;

namespace AzureKit
{
    public partial class Startup
    {
        private static string s_clientId = ConfigurationManager.AppSettings["ClientId"];
        private static string s_aadInstance = ConfigurationManager.AppSettings["AADInstance"];
        private static string s_tenantId = ConfigurationManager.AppSettings["TenantId"];
        private static string s_b2cPolicy = ConfigurationManager.AppSettings["B2cSignInOrUpPolicy"];

        public static readonly string Authority = s_aadInstance + s_tenantId;

        public static bool IsAuthenticationConfigured => !string.IsNullOrEmpty(s_clientId) && !string.IsNullOrEmpty(Authority);

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
                        // interactive use, but for script access it's not good, because clients see a 200 with
                        // an HTML body they won't be able to comprehend when what we should be telling them
                        // is 401. So we only do the redirect that the cookie auth provider wants to do if
                        // the request is not for an endpoint intended to be consumed directly by script.
                        var absPath = ctx.Request.Uri.AbsolutePath;
                        if (!absPath.StartsWith(VirtualPathUtility.ToAbsolute("~/api/")))
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                    }
                }
            });

            //only try to setup AAD auth if it has been configured - this is the public-facing web site,
            //and although it supports having end users log in, this is not required. You can have the
            //site accessible purely anonymously
            if (IsAuthenticationConfigured)
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
                        AuthenticationType = OpenIdConnectAuthenticationDefaults.AuthenticationType,
                        ClientId = s_clientId,

                        // We supply the complete OpenID metadata address instead of just the Authority,
                        // because with B2C, there's a different metadata endpoint for each policy.
                        // (Since the MetadataAddress includes the Authority, we don't also need to set
                        // theAuthority property.)
                        MetadataAddress = $"{Authority}/v2.0/.well-known/openid-configuration?p={s_b2cPolicy}",

                        PostLogoutRedirectUri = rootUri,
                        RedirectUri = rootUri,
                        Notifications  = new OpenIdConnectAuthenticationNotifications
                        {
                            AuthenticationFailed = AuthenticationFailed,
                        },
                        Scope = "openid",
                        ResponseType = "id_token",

                        // I think this piece is optional, although I dimly remember having problems without it
                        TokenValidationParameters = new TokenValidationParameters
                        {
                            NameClaimType = "name",
                        }
                    });
            }
        }

        // Used for avoiding yellow-screen-of-death when Azure AD B2C tells us that
        // authentication failed when it redirects back to us.
        private Task AuthenticationFailed(
            AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
        {
            notification.HandleResponse();
            if (notification.Exception.Message == "access_denied")
            {
                notification.Response.Redirect("/");
            }
            else
            {
                notification.Response.Redirect("/Account/Error?message=" + notification.Exception.Message);
            }

            return Task.FromResult(0);
        }
    }
}
