using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AZKitMobile
{
    /// <summary>
    /// Wrapper around the Azure MObile Apps client.
    /// Handles all communication to the API
    /// and forwards client specific requests to the client handlers.
    /// </summary>
    public class azkitClient
    {
        // Set this to your API site's URL if you want to be able to debug locally while
        // still being able to log in through Azure App Service authentication.
        private const string AlternateLoginHost = null;
        private bool _isLoggedIn = false;
        private string _userFirstName = "";
        private static MobileServiceClient _client;

        static azkitClient()
        {
            var mobileAppUrl = Models.Settings.ServiceUrl;
            MakeClient(mobileAppUrl);
        }

        private static void MakeClient(string mobileAppUrl)
        {
            _client = string.IsNullOrEmpty(mobileAppUrl) ?
                null : new MobileServiceClient(mobileAppUrl);
#if DEBUG
            if (_client != null && !string.IsNullOrWhiteSpace(AlternateLoginHost))
            {
                _client.AlternateLoginHost = new Uri(AlternateLoginHost);
            }
#endif
        }

        /// <summary>
        /// Called from settings change to re-initialize the client
        /// with the new URL
        /// </summary>
        internal void InitClient()
        {
            var mobileAppUrl = Models.Settings.ServiceUrl;

            //only apply the changes if the URl has changed if there 
            //is an existing client object.
            if (_client == null ||
                (_client != null &&
                String.Compare(_client.MobileAppUri.ToString(), mobileAppUrl, StringComparison.OrdinalIgnoreCase) != 0))
            {
                _client = String.IsNullOrEmpty(mobileAppUrl) ?
                    null : new MobileServiceClient(mobileAppUrl);
            }
        }
        /// <summary>
        /// Indicates if the user has logged into the app yet.
        /// </summary>
        public bool IsLoggedIn
        {
            get { return this._isLoggedIn; }
        }

        /// <summary>
        /// Provides the user's first name after login has completed.
        /// Data is retrieved from the user profile on the backend.
        /// </summary>
        public string UserFirstName
        {
            get { return this._userFirstName; }
        }

        /// <summary>
        /// Gets the mobile content from the service by invoking an API.
        /// We are not using the Tables feature of Azure Mobile so instead use
        /// this simple API approach to query for some data. 
        /// </summary>
        /// <returns>A list of content that has been marked for mobile devices.</returns>
        public async Task<List<Models.ContentModelBase>> GetContentAsync()
        {
            try
            {
                var content = await _client.InvokeApiAsync<List<Models.ContentModelBase>>("MobileContent", HttpMethod.Get, null);
                return content;
            }
            catch(Exception ex)
            {
                MessagingCenter.Send<azkitClient, string>(this, Constants.KEY_MESSAGING_EXCEPTION, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Invokes the platform specific logic/code to log the user into the 
        /// mobile application.
        /// Also initiates the profile load on success and sets the IsLoggedIn property.
        /// </summary>
        /// <returns>a reference to the user object obtained which should contain a token and identifier.</returns>
        public async Task<MobileServiceUser> LoginUserAsync()
        {
            try
            {
                ILoginManager manager = Xamarin.Forms.DependencyService.Get<ILoginManager>(Xamarin.Forms.DependencyFetchTarget.NewInstance);
                var user = await manager.LoginAsync(_client);
                _isLoggedIn = (user != null && user.MobileServiceAuthenticationToken != null);
                //we want this to run in the background so we don't await it.
                await LoadProfileDetails();
                return user;
            }
            catch (InvalidOperationException e) when (e.Message == "Authentication was cancelled by the user.")
            {
                return null;
            }
            catch (Exception ex)
            {
                MessagingCenter.Send<azkitClient, string>(this, Constants.KEY_MESSAGING_EXCEPTION, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Registers for notifications with the mobile serivce.
        /// First invokes platform specific code, then updates the tags
        /// using a custom controller on the backend.
        /// </summary>
        /// <returns>True if registration succeeded with no errors or false if errors occurred.</returns>
        public async Task<bool> RegisterForNotificationsAsync()
        {
            var tags = GetPushTags();

            //use the dependency service to get the platform specific implementation
            INotificationsManager manager = Xamarin.Forms.DependencyService.Get<INotificationsManager>();
            if (manager != null)
            {
                //invoke the client specific registration
                await manager.RegisterForPushNotifications(_client);

                //if we succeeded in registering with the mobile app
                //then call back to update with the tags (set on the settings view)
                await UpdateRegistrationWithTags(tags);
                return true;

            }
            else { return false; }
        }

        /// <summary>
        /// Calls a custom API to add tags to our notification registration since this isn't supported natively in mobile apps client
        /// </summary>
        /// <param name="tags">The tags to be set on the notification</param>
        /// <returns></returns>
        private async Task UpdateRegistrationWithTags(List<string> tags)
        {
            JArray tagPayload = new JArray(tags.ToArray());
            
            var tagResponse = await _client.InvokeApiAsync("PushTags/" + _client.InstallationId, tagPayload, HttpMethod.Put, null);

        }

        /// <summary>
        /// Retrieves the list of tags to be supplied for push notifications. 
        /// This tags are based on the settings defined by the user.
        /// </summary>
        /// <returns></returns>
        private List<string> GetPushTags()
        {
            List<string> tags = new List<string>();
            if (AZKitMobile.Models.Settings.GeneralNotifications)
            {
                tags.Add(AZKitMobile.Constants.TAG_GENERAL);
            }
            if (AZKitMobile.Models.Settings.SiteUpdateNotifications)
            {
                tags.Add(Constants.TAG_SITE);
            }

            return tags;
        }

        /// <summary>
        /// Calls the profile endpoint on the azure mobile app to
        /// retrieve the profile information (claims) from the
        /// identity provider (AAD in our case)
        /// </summary>
        /// <returns></returns>
        private async Task LoadProfileDetails()
        {
            JToken profileInformation = null;
            
            try
            {
                //this is not part of the mobile app, so it's just a regular HTTP request
                HttpClient hclient = new HttpClient();
                hclient.BaseAddress = _client.MobileAppUri;

                //the request still needs to be authenticated though
                hclient.DefaultRequestHeaders.Add("X-ZUMO-AUTH", _client.CurrentUser.MobileServiceAuthenticationToken);
                var result = await hclient.GetAsync("/.auth/me");

                if(result.IsSuccessStatusCode)
                {
                    using (Stream contentStream = await result.Content.ReadAsStreamAsync())
                    {
                        using (var tReader = new StreamReader(contentStream))
                        {
                            using (var reader = new JsonTextReader(tReader))
                            {
                                //this is specific parsing for the payload containing the AAD 
                                //claims. We just get firstName, but other options are available based on the AAD configuration.
                                profileInformation = JObject.ReadFrom(reader);
                                if(profileInformation != null)
                                {
                                    var claims = profileInformation.First["user_claims"];
                                    if(claims != null)
                                    {
                                        var firstNameClaim = claims.Where(j => j["typ"].ToString() == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname").FirstOrDefault();
                                        if(firstNameClaim != null)
                                        {
                                            var firstName = firstNameClaim["val"].ToString();
                                            this._userFirstName = firstName;

                                            //notify any interested views or components that we loaded the profile
                                            Xamarin.Forms.MessagingCenter.Send<IMobileServiceClient, string>(
                                                _client, Constants.KEY_MESSAGING_PROFILE, firstName);
                                        }
                                    }
                                }    
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                //this will display on the home screen indicating an error
                Xamarin.Forms.MessagingCenter.Send<IMobileServiceClient, string>(
                                               _client, Constants.KEY_MESSAGING_PROFILE, "{profile error}");
            }
        }
    }
}
