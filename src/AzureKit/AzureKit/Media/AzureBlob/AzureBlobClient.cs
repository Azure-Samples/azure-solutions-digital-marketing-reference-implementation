using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using AzureKit.Models;
using System.IO;
using AzureKit.Config;
using System.Threading.Tasks;

namespace AzureKit.Media.AzureBlob
{
    /// <summary>
    /// Wrapper around blob information such as SAS urls
    /// and container URLs based on the configuration information
    /// </summary>
    public class AzureBlobClient
    {
        private AzureBlobConfig config;
        private CloudStorageAccount account;
        private CloudBlobClient blobClient;

        public AzureBlobClient(Config.AzureBlobConfig blobConfig)
        {
            config = blobConfig;
            //only try to create the account if load succeeded
            if (config.LoadSucceeded)
            {
                var storageConnectionString = String.Format(Constants.FORMAT_AZURE_STORAGE_URL, config.StorageAccountName, config.StorageAccountKey);
                account = CloudStorageAccount.Parse(storageConnectionString);
            }
        }

        internal async Task<string> PutImageFromStreamAsync(Stream media, string mediaName, string mediaContentType)
        {

            var container = Client.GetContainerReference(config.ImageContainerName);
            var blob = container.GetBlockBlobReference(mediaName);
            blob.Properties.ContentType = mediaContentType;
            try {
                await blob.UploadFromStreamAsync(media);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to save thumbnail", ex);
            }

            //return the regular URL for the blob
            return blob.Uri.ToString();
        }

        private Uri GetSASURIForImageFile(string fileName)
        {
            var endpointDetails = GetImageSASUrl();
            return new Uri(endpointDetails.ContainerUrl + "/" + fileName + endpointDetails.SASToken, UriKind.Absolute);
        }
        private CloudBlobClient Client
        {
            get {
                if (blobClient == null)
                {
                    blobClient = account.CreateCloudBlobClient();
                }
                return blobClient;
            }
        }

        public string VideoContainerUrl {
            get
            {
                return Client.GetContainerReference(config.VideoContainerName).Uri.ToString();
            }
        }

        public string ImageContainerUrl
        {
            get
            {
                return Client.GetContainerReference(config.ImageContainerName).Uri.ToString();
            }
        }

        public string CDNAddress
        {
            get
            {
                return String.Format("https://{0}", config.CDNAddress);
            }
        }

        public string StorageAddress
        {
            get { return string.Format(Constants.FORMAT_AZURE_STORAGE_ADDRESS, config.StorageAccountName);  }
        }

        /// <summary>
        /// Creates an object that provides the container URL and SAS token
        /// to be combined to create a SAS URL for the container.
        /// For container operations these can be concatenated together.
        /// For blob operations the blob name needs to be inserted, which is why 
        /// two values are returned separately.
        /// </summary>
        /// <returns></returns>
        public MediaUploadEndpointDetails GetVideoSASUrl()
        {
            var videoContainer = Client.GetContainerReference(config.VideoContainerName);

            var sasSignature = videoContainer.GetSharedAccessSignature(
                GetBlobUploadAccessPolicy());

            return  new MediaUploadEndpointDetails
            {
                ContainerUrl = videoContainer.Uri.ToString(),
                SASToken = sasSignature
            };
        }

        /// <summary>
        /// Creates an object that provides the container URL and SAS token
        /// to be combined to create a SAS URL for the container.
        /// For container operations these can be concatenated together.
        /// For blob operations the blob name needs to be inserted, which is why 
        /// two values are returned separately.
        /// </summary>
        /// <returns></returns>
        public MediaUploadEndpointDetails GetImageSASUrl()
        {
            var imageContainer = Client.GetContainerReference(config.ImageContainerName);

            var sasSignature = imageContainer.GetSharedAccessSignature(
                GetBlobUploadAccessPolicy());

            return new MediaUploadEndpointDetails
            {
                ContainerUrl = imageContainer.Uri.ToString(),
                SASToken = sasSignature
            };
        }

        /// <summary>
        /// Creates the shared access policy used for SAS URLs that 
        /// permits putting objects to Azure blob storage.
        /// </summary>
        /// <returns></returns>
        private SharedAccessBlobPolicy GetBlobUploadAccessPolicy()
        {
            SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy();
            policy.SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(config.SASURLExpiryDuration);
            policy.Permissions = SharedAccessBlobPermissions.Create | 
                SharedAccessBlobPermissions.Write | 
                SharedAccessBlobPermissions.Add;

            return policy;

        }

        public Stream GetReadStreamForItem(string itemUrl)
        {
            ICloudBlob blobRef = null;

            try {
                blobRef = Client.GetBlobReferenceFromServer(new Uri(itemUrl));
            }
            catch(Exception)
            {
                return null;
            }


            if (blobRef != null)
            {
                return blobRef.OpenRead();
            }
            else
                return null;

        }
  
    }
}