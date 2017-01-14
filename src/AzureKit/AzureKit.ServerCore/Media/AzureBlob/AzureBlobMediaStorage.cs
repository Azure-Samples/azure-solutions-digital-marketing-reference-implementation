using AzureKit.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;

namespace AzureKit.Media.AzureBlob
{
    public class AzureBlobMediaStorage : IMediaStorage
    {
        private AzureBlobClient _blobClient;

        public AzureBlobMediaStorage(AzureBlobClient client)
        {
            _blobClient = client;
        }
        private AzureBlobClient Client
        {
            get
            {
                if (_blobClient == null)
                {
                    _blobClient = (AzureBlobClient)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(AzureBlobClient));
                }

                return _blobClient;
            }
        }
        public MediaUploadEndpointDetails GetUploadEndpoint()
        {
            return Client.GetImageSASUrl();
        }


        private async Task<string> PutMediaStreamAsync(Stream media, string mediaName, string mediaContentType)
        {
            return await Client.PutImageFromStreamAsync(media, mediaName, mediaContentType).ConfigureAwait(false);
        }

        public async Task<string> StoreThumbnailAsync(string inputMediaName, string outputMediaName, string outputMediaContentType, Action<Stream, Stream> thumbnailCreator)
        {
            MediaUploadEndpointDetails endpointDetails = Client.GetImageSASUrl();
            var blobRef = Client.GetReferenceForItem(endpointDetails.ContainerUrl + "/" + inputMediaName);
            
            using (Stream inputStream = blobRef.OpenRead())
            {
                using (MemoryStream thumbnailStream = new MemoryStream())
                {
                    thumbnailCreator(inputStream, thumbnailStream);
                    thumbnailStream.Seek(0, SeekOrigin.Begin);
                    return await PutMediaStreamAsync(thumbnailStream, outputMediaName, outputMediaContentType).ConfigureAwait(false);
                }
            }
            
        }

        public string MediaBaseAddress
        {
            get
            {  
                if (!string.IsNullOrEmpty(Client.CDNAddress))
                {
                    return Client.CDNAddress;
                }
                else
                {
                    return Client.StorageAddress;
                }
            }
        }
    }
}