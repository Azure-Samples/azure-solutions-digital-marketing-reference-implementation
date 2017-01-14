using System.Configuration;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Owin;

namespace AzureKit.Api
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
#if DEBUG
            // It is useful to be able to debug the API app locally, but this becomes challenging
            // once authentication is involved - Azure App Service does the authentication for
            // us, and of course if we're running locally then Azure App Service is no longer in
            // the picture.
            // What we can do, though, is arrange for the client to authenticate against the real
            // service, but then talk to localhost for everything else. That wouldn't normally work
            // because the local service would have no way of validating the token. But the following
            // code configures the token handling so that it will work correctly with tokens supplied
            // by the real service.when running locally.
            // On the client side, this 
            // First, we need to detect whether we are in fact running locally, or up in Azure:
            var config = GlobalConfiguration.Configuration;
            MobileAppSettingsDictionary settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();
            var cons = settings.Connections;
            if (string.IsNullOrEmpty(settings.HostName))
            {
                // By default, HostName will only have a value when running in an App Service application,
                // so if we get here it means we're running locally.

                // Next, has the developer supplied the config we require to be able to validate
                // Azure-supplied keys locally?
                // See
                //  http://www.systemsabuse.com/2015/12/04/local-debugging-with-user-authentication-of-an-azure-mobile-app-service/
                // for details on how to get these settings.
                var signingKey = ConfigurationManager.AppSettings["SigningKey"];
                if (!string.IsNullOrWhiteSpace(signingKey))
                {
                    // The dev has supplied a signing key (and we assume if they've worked
                    // out how to do that, they've also filled in the audience and issuer)
                    // so let's configure the middleware to be able to process tokens from
                    // Azure.
                    var options = new AppServiceAuthenticationOptions
                    {
                        SigningKey = ConfigurationManager.AppSettings["SigningKey"],
                        ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                        ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                        TokenHandler = config.GetAppServiceTokenHandler()
                    };
                    app.UseAppServiceAuthentication(options);
                }
            }
#endif

            // If we're not debugging locally, we rely entirely on Azure App
            // Service authentication, so there's nothing for us to do.
        }
    }
}
