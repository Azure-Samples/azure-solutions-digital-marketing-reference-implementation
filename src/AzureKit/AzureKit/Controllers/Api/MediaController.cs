﻿using AutoMapper;
using AzureKit.Config;
using AzureKit.Data;
using AzureKit.Media;
using AzureKit.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AzureKit.Controllers.Api
{
    /// <summary>
    /// Api called from JavaScript to work with media items (gallery content)
    /// </summary>
    [Authorize]
    public class MediaController : ApiController
    {
        private IMediaStorage _mediaStore;
        private IMappingEngine _map;
        private ISiteContentRepository _repo;

        public MediaController(IMediaStorage mediaStorage, ISiteContentRepository repository, IMappingEngine mapper)
        {
            _map = mapper;
            _repo = repository;
            _mediaStore = mediaStorage;
        }

        /// <summary>
        /// After teh client uploads the media to Azure Blob, this method 
        /// is invoked to store the metadata.
        /// </summary>
        /// <param name="details">The details about the media item including the URL and the gallery to which it is being added.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> AddGalleryItem(MediaUploadDetails details)
        {
            if (details == null ||
                String.IsNullOrEmpty(details.GalleryId) ||
                String.IsNullOrEmpty(details.MediaUrl))
            {
                return BadRequest("Invalid or missing details for media item");
            }
            
            //start with the static video thumbnail
            string thumbnailUrl = Constants.PATH_STATIC_VIDEO_THUMBNAIL; ;

            //for images, create the thumbnail and store it
            if (details.MediaContentType.StartsWith("image", StringComparison.InvariantCultureIgnoreCase))
            {
                var thumbnailName = MediaUtilities.CreateThumbnailFileName(details.Name);
                thumbnailUrl= await _mediaStore.StoreThumbnailAsync(details.Name, thumbnailName, details.MediaContentType, MediaUtilities.CreateThumbnailForImage);
            }
            var modelItem = _map.Mapper.Map<Models.MediaItemModel>(details);
           
            //strip out server details for the image files being stored
            modelItem.ThumbnailUrl = MakeMediaURLRelative(thumbnailUrl);
            modelItem.MediaUrl = MakeMediaURLRelative(modelItem.MediaUrl);

            //update the gallery with the new item
            await _repo.AddItemToGalleryAsync(
                details.GalleryId, modelItem);

            //return JSON content
            var responseContent = new JObject(
                new JProperty("thumbnailUrl", thumbnailUrl),
                new JProperty("newItemId", modelItem.Id));
            var response = Request.CreateResponse<JObject>(HttpStatusCode.Created,responseContent);
            response.Headers.Location  = new Uri(details.MediaUrl);

            return ResponseMessage(response);
        }

        /// <summary>
        /// Gets the endpoint used to upload the media item
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public MediaUploadEndpointDetails GetUploadURL()
        {
            return _mediaStore.GetUploadEndpoint();
        }

        /// <summary>
        /// Removes an item from the gallery
        /// </summary>
        /// <param name="galleryId"></param>
        /// <param name="mediaItemId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/Media/{galleryId}/{mediaItemId}")]
        public async Task<IHttpActionResult> RemoveGalleryItem(string galleryId, string mediaItemId)
        {
            await _repo.RemoveItemFromGalleryAsync(galleryId, mediaItemId);
            return base.StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Makes the given URI relative for storage in the content store.
        /// Clients will need to make absolute URIs using the CDN or media storage client.
        /// </summary>
        /// <param name="fullUrl"></param>
        /// <returns></returns>
        private string MakeMediaURLRelative(string fullUrl)
        {
            var uri = new System.Uri(fullUrl);
            return uri.PathAndQuery;
        }
    }
}
