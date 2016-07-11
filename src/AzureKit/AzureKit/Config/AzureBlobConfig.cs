using System;
using System.Configuration;

namespace AzureKit.Config
{
    /// <summary>
    /// Represents the configuration information found in web.config
    /// or app settings from Azure 
    /// </summary>
    public class AzureBlobConfig
    {
        public void Load()
        {
            try
            {
                StorageAccountKey = ConfigurationManager.AppSettings[Constants.KEY_AZURE_STORAGE_ACCT_KEY];
                StorageAccountName = ConfigurationManager.AppSettings[Constants.KEY_AZURE_STORAGE_ACCT_NAME];

                VideoContainerName = ConfigurationManager.AppSettings[Constants.KEY_AZURE_STORAGE_VIDEO_CONTAINER];
                ImageContainerName = ConfigurationManager.AppSettings[Constants.KEY_AZURE_STORAGE_IMAGES_CONTAINER];

                CDNAddress = ConfigurationManager.AppSettings[Constants.KEY_AZURE_STORAGE_CDN];

                int tempTimeout = 0;
                if (int.TryParse(ConfigurationManager.AppSettings[Constants.KEY_AZURE_STORAGE_UPLOAD_POLICY_TIMEOUT], out tempTimeout))
                {
                    SASURLExpiryDuration = tempTimeout;
                }
                else
                {
                    SASURLExpiryDuration = Constants.DEFAULT_SASURL_EXPIRY;
                }

                LoadSucceeded = true;
            }
            catch (Exception ex)
            {
                LoadSucceeded = false;

                System.Diagnostics.Debug.WriteLine("Failed to initialize blog configuration from app settings: " + ex);
            }
        }

        public string VideoContainerName { get; set; }
        public string ImageContainerName { get; set; }

        public string StorageAccountKey { get; set; }

        public string StorageAccountName { get; set; }

        public int SASURLExpiryDuration { get; set; }

        public string CDNAddress { get; set; }

        public bool LoadSucceeded { get; set; }
    }
}