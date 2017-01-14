using AzureKit.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureKit.Media
{
    public interface IMediaStorage
    {
        Task<string> StoreThumbnailAsync(string inputMediaName, string outputMediaName, string outputMediaContentType, Action<Stream,Stream> thumbnailCreator);

        MediaUploadEndpointDetails GetUploadEndpoint();

        string MediaBaseAddress { get; }    
    }
}
