///simple object for holding information about a media item
function MediaFileContainer() {

    return {
        name: "",
        description:"",
        title: "",
        fileType: "",
        fileSize: 0,
        containerUrl: "",
        sasToken: "",
        galleryId: "",
        blobSASUrl: function () {
            return this.containerUrl + "/" + this.name + this.sasToken;
        }

    }
}