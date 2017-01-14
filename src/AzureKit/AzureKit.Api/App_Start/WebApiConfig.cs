using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;

namespace AzureKit.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //mobile app config
            new MobileAppConfiguration()
                .AddPushNotifications()
                .MapApiControllers()
                .ApplyTo(config);
        }
    }
}
