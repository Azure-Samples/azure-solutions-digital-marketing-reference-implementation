///file used by gallery admin pages to upload or delete
///media items

//connect event handlers to controls

$("#imageFile").bind('change', onImageFileSelected);
$("#startImageUpload").bind('click', uploadImage);

//event handlers
function onImageFileSelected(e) {

    //show image file details for the selected file
    var fileDetails = e.target.files[0];
    if (fileDetails !== null) {
        showHideImageFileDetails(true);
        $("#imageFileName").text(fileDetails.name);
        $("#imageFileSize").text(fileDetails.size);
        $("#imageFileType").text(fileDetails.type);
        $("#imageUpProgress").attr('max',fileDetails.size);
    }
}

function clearUploadForm() {
    showHideImageFileDetails(false);
    $("#imageUp")[0].reset();
}

//visibility functions for details and progress bars
function showHideImageFileDetails(show) {
    if (show) {
        $("#imageFileDetailsPanel").show();
    }
    else {
        $("#imageFileDetailsPanel").hide();
    }
}


function showHideImageUploadProgress(show) {
    if (show) {
        $("#imageProgressPanel").show();
    }
    else {
        $("#imageProgressPanel").hide();
    }
}

///Called after the upload has completed
///this function calls an API on the server to 
///update the metadata in the content store
function finalizeUpload(uploadDetails) {
    $.ajax("/api/Media",
        {
            method: "POST",
            data: JSON.stringify(
            {
                "MediaUrl": uploadDetails.containerUrl + "/" + uploadDetails.name,
                "GalleryItemType": "Image",
                "Tags": uploadDetails.tags,
                "GalleryId": uploadDetails.galleryId,
                "MediaContentType": uploadDetails.fileType,
                "Description": uploadDetails.description,
                "Title": uploadDetails.title,
                "Name": uploadDetails.name

            }),
            contentType: "application/json",
            dataType: "json",
            headers: { "RequestVerificationToken": xrsfTokenHeader },
        }
        ).done(function (data, status, jqXhr) {
            if (jqXhr.status == 201) {
                var newItem = $("<div class='galleryItem container'><img src='" +
                       data.thumbnailUrl + "' alt='" + uploadDetails.name + "'/><br />" +
                       "<span>" + uploadDetails.name + "</span> <button class='btn btn-default' onclick='removeItem(this, \"" + data.newItemId + "\")'>" +
                           "<span class='glyphicon glyphicon-minus' aria-hidden='true'></span></button>" +
                       "<span>" + uploadDetails.description + "</span></div>");

                $("#galleryImages").append(newItem);

                //dismiss modal dialog
                $("#imageUploadModal").modal('hide');
                //clear form
                clearUploadForm();
            }
            else {
                alert("Image metadata upload failed");
            }
        }
        ).fail(function (jqXHR, textStatus, errorThrown) {
            alert("failed to upload image metadata; try again");
        });
}

//removes an item from the gallery
function removeItem(btn, mediaId) {
    var galleryId = $("#Id").val();
    
    var that = $(btn);

    $.ajax("/api/Media/" + galleryId + "/" + mediaId, {
        method: "DELETE",
        headers: { "RequestVerificationToken": xrsfTokenHeader }
    }).done(function (data, textStatus, jqXHR) {
        //remove item from list
        that.parent(".container").remove();
    }
    ).fail(function (jqXHR, textStatus, errorThrown) {
        alert("item not removed: error");
    });
};

//main function to upload a video. 
//starts the process by creating the metadata and 
//get a SAS URL from the API controller to use for 
// putting the blocks to Azure
function uploadVideo() {
    var fileInput = document.getElementById("videoFile");

    if (!fileInput.files || fileInput.files.length <= 0) {
        alert("Please select a video file before uploading");
        return;
    }

    //get the SAS URL to use for putting blocks
    $.getJSON("/api/Media/UploadURL").then(
        function (results) {
            var videoMetadata = new MediaFileContainer();

            //get container Url from results
            videoMetadata.containerUrl = results.ContainerUrl;
            videoMetadata.sasToken = results.SASToken;

            //get file information for video
            var file = fileInput.files[0];
            videoMetadata.fileSize = file.size;
            videoMetadata.fileType = file.type;
            videoMetadata.name = file.name;

            //get metadata for the video
            videoMetadata.title = $("#videoTitle").val();
            videoMetadata.description = $("#videoDescription").val();
            

            //create an uploader and show progress
            var uploader = new MediaUploader(videoMetadata, file);
            showHideVideoUploadProgress(true);

            //begin the upload and initialize the progress at 0
            uploader.beginUpload(function(progress){
                //$("#videoUpProgress").max = progress.totalBytes;
                $("#videoUpProgress").val(progress.currentBytes);   
            });
        },
        function (error) {
            alert(error);
        }
    );
}

//main function to upload an image. 
//starts the process by creating the metadata and 
//get a SAS URL from the API controller to use for 
// putting the blocks to Azure
function uploadImage() {
    var fileInput = document.getElementById("imageFile");

    if (!fileInput.files || fileInput.files.length <= 0) {
        alert("Please select an image file before uploading");
        return;
    }

    //get SAS URL for putting blocks to Azure
    $.getJSON("/api/Media").then(
        function (results) {
            var imageMetadata = new MediaFileContainer();

            //get container Url from results

            imageMetadata.containerUrl = results.ContainerUrl;
            imageMetadata.sasToken = results.SASToken;

            //get file information for image
            var file = fileInput.files[0];
            imageMetadata.fileSize = file.size;
            imageMetadata.fileType = file.type;
            imageMetadata.name = file.name;

            //get metadata for the video
            imageMetadata.title = $("#imageTitle").val();
            imageMetadata.description = $("#imageDescription").val();
            imageMetadata.tags = $("#imageTags").val().split(",");
            imageMetadata.galleryId = $("#Id").val();

            //start upload
            var uploader = new MediaUploader(imageMetadata, file);
            showHideImageUploadProgress(true);

            uploader.beginUpload(function (progress) {
                //$("#imageUpProgress").max = progress.totalBytes;
                $("#imageUpProgress").val(progress.currentBytes);
                if (progress.currentBytes == progress.totalBytes) {
                    finalizeUpload(imageMetadata);
                }
            });
        },
        function (error) {
            alert(error);
        }
    );
}
