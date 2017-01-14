using AzureKit.Config;
using AzureKit.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureKit.Media.AzureBlob
{
    /// <summary>
    /// Wrapper around blob information such as SAS urls
    /// and container URLs based on the configuration information
    /// </summary>
    public class AzureBlobClient
    {
        private IAzureBlobConfig _config;
        private CloudStorageAccount _account;
        private CloudBlobClient _blobClient;

        public AzureBlobClient(Config.IAzureBlobConfig blobConfig)
        {
            _config = blobConfig;
            //try to create the account
            
            var storageConnectionString = String.Format(Constants.FORMAT_AZURE_STORAGE_URL, _config.StorageAccountName, _config.StorageAccountKey);
            _account = CloudStorageAccount.Parse(storageConnectionString);
            
        }

        internal async Task<string> PutImageFromStreamAsync(Stream media, string mediaName, string mediaContentType)
        {

            var container = Client.GetContainerReference(_config.ImageContainerName);
            var blob = container.GetBlockBlobReference(mediaName);
            blob.Properties.ContentType = mediaContentType;
            try {
                await blob.UploadFromStreamAsync(media).ConfigureAwait(false);
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
                if (_blobClient == null)
                {
                    _blobClient = _account.CreateCloudBlobClient();
                }
                return _blobClient;
            }
        }

        public string VideoContainerUrl {
            get
            {
                return Client.GetContainerReference(_config.VideoContainerName).Uri.ToString();
            }
        }

        public string ImageContainerUrl
        {
            get
            {
                return Client.GetContainerReference(_config.ImageContainerName).Uri.ToString();
            }
        }

        public string CDNAddress
        {
            get
            {
                return String.IsNullOrWhiteSpace(_config.CDNAddress)
                    ? null
                    : String.Format("https://{0}", _config.CDNAddress);
            }
        }

        public string StorageAddress
        {
            get { return string.Format(Constants.FORMAT_AZURE_STORAGE_ADDRESS, _config.StorageAccountName);  }
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
            var videoContainer = Client.GetContainerReference(_config.VideoContainerName);

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
            var imageContainer = Client.GetContainerReference(_config.ImageContainerName);

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
            policy.SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(_config.SASURLExpiryDuration);
            policy.Permissions = SharedAccessBlobPermissions.Create | 
                SharedAccessBlobPermissions.Write | 
                SharedAccessBlobPermissions.Add;

            return policy;
        }

        public ICloudBlob GetReferenceForItem(string itemUrl)
        {
            ICloudBlob blobRef = Client.GetBlobReferenceFromServer(new Uri(itemUrl));
            return blobRef;
        }
    }
}