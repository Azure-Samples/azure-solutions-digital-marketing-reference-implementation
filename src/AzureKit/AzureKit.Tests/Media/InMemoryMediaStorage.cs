using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureKit.Models;

namespace AzureKit.Tests.Media
{
    public class InMemoryMediaStorage : AzureKit.Media.IMediaStorage
    {
        public string MediaBaseAddress
        {
            get
            {
                return "https://localhost:44300/";
            }
        }

        public MediaUploadEndpointDetails GetUploadEndpoint()
        {
            return new MediaUploadEndpointDetails
            {
                ContainerUrl = "https://localhost:44300/images",
                SASToken = ""
            };
        }

        public Task<string> StoreThumbnailAsync(string inputMediaName, string outputMediaName, string outputMediaContentType, Action<Stream, Stream> thumbnailCreator)
        {
            return Task.FromResult<string>(outputMediaName);
        }
    }
}
