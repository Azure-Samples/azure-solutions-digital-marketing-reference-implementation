using System;
using System.IO;
using System.Threading.Tasks;
using AzureKit.Models;
using System.Web.Http;

namespace AzureKit.Media.AzureBlob
{
    public class AzureBlobMediaStorage : IMediaStorage
    {
        private AzureBlobClient _blobClient;

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

        private Stream GetMediaStream(string mediaName)
        {      
            MediaUploadEndpointDetails endpointDetails = Client.GetImageSASUrl();
            
            return Client.GetReadStreamForItem(endpointDetails.ContainerUrl + "/" + mediaName);
        }

        private async Task<string> PutMediaStreamAsync(Stream media, string mediaName, string mediaContentType)
        {
            return await Client.PutImageFromStreamAsync(media, mediaName, mediaContentType);
        }

        public async Task<string> StoreThumbnailAsync(string inputMediaName, string outputMediaName, string outputMediaContentType, Action<Stream, Stream> thumbnailCreator)
        {
            using (Stream inputStream = GetMediaStream(inputMediaName))
            {
                using (MemoryStream thumbnailStream = new MemoryStream())
                {
                    thumbnailCreator(inputStream, thumbnailStream);
                    thumbnailStream.Seek(0, SeekOrigin.Begin);
                    return await PutMediaStreamAsync(thumbnailStream, outputMediaName, outputMediaContentType);
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