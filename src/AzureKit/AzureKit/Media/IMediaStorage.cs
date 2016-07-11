using System.IO;
using AzureKit.Models;
using System.Threading.Tasks;
using System;

namespace AzureKit.Media
{
    public interface IMediaStorage
    {
        Task<string> StoreThumbnailAsync(string inputMediaName, string outputMediaName, string outputMediaContentType, Action<Stream,Stream> thumbnailCreator);

        MediaUploadEndpointDetails GetUploadEndpoint();

        string MediaBaseAddress { get; }    
    }
}
