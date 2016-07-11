﻿//media upload class
//handles putting a file to azure blob storage
//this could be replaced with an implementation that stores to standard
//web server. The corresponding media store implementation on the server 
//would need to be updated to match.


//based on the example by Gaurav Mantri
// http://gauravmantri.com/2013/02/16/uploading-large-files-in-windows-azure-blob-storage-using-shared-access-signature-html-and-javascript/

function MediaUploader(metadata, file) {
    var currentPointer = 0;
    var blockSize = 1024 * 256;

    var totalBytes = metadata.fileSize;
    var remainingBytes = totalBytes;
    var uploadedBytes = 0;

    var blockPrefix = "blo_";
    var blockIds = [];

    var progressCB;

    //handle files smaller than default block size
    if (totalBytes < blockSize) {
        blockSize = totalBytes;
    }

    //HTML 5 file reader used to read the bytes from the file object
    var reader = new FileReader();

    //when the bytes of the file are ready, 
    // put the block to the blob
    reader.onloadend = function (evt) {
        if (evt.target.readyState == FileReader.DONE) {
            //create URL for putting a block
            var uri = metadata.blobSASUrl() + '&comp=block&blockid=' + blockIds[blockIds.length - 1];

            //convert data to correct data type and PUT
            var requestData = new Uint8Array(evt.target.result);
            $.ajax({
                url: uri,
                type: "PUT",
                data: requestData,
                processData: false,
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('x-ms-blob-type', 'BlockBlob');
                    //causing security warnings in some browsers, trying without
                    //xhr.setRequestHeader('Content-Length', requestData.length);
                },
                success: function (data, status) {
                    //update progress
                    uploadedBytes += requestData.length;
                    progressCB({ totalBytes: totalBytes, currentBytes: uploadedBytes })

                    //begin the upload of the next block
                    uploadBlock();
                },
                error: function (xhr, desc, err) {

                    //TODO: notify user of failure 
                    //TODO: Add retry
                    console.log(desc);
                    console.log(err);
                }
            });
        }

    };

    //uploads the next block of data from the file if there is any
    //if all blocks are uploaded, calls to commit the block blob
    function uploadBlock() {
        //if we have more bytes, read more, otherwise commit the blob
        if (remainingBytes > 0) {
            //get slice of file
            var fileSlice = file.slice(currentPointer, currentPointer + blockSize);

            // create block id
            var currentBlockId = blockPrefix + ('000000' + blockIds.length.toString()).slice(-6);
            blockIds.push(btoa(currentBlockId));

            //read file content (triggers onloadend [above] when done reading)
            reader.readAsArrayBuffer(fileSlice);

            //update pointers/counters
            currentPointer += blockSize;
            remainingBytes -= blockSize;
            if (remainingBytes < blockSize) {
                blockSize = remainingBytes;
            }
        }
        else {
            //commit the blob if all blocks are written
            commitBlob();
        }
    }

    //commits the blob by putting a request with the block IDs 
    //also adds metadata to the blob on commit (title and description)
    function commitBlob() {
        //create SAS URL for blob commit
        var uri = metadata.blobSASUrl() + '&comp=blocklist';

        //create XML payload to commit blocks
        var requestBody = '<?xml version="1.0" encoding="utf-8"?><BlockList>';
        for (var i = 0; i < blockIds.length; i++) {
            requestBody += '<Latest>' + blockIds[i] + '</Latest>';
        }
        requestBody += '</BlockList>';

        //send commit message to blob storage
        $.ajax({
            url: uri,
            type: "PUT",
            data: requestBody,
            beforeSend: function (xhr) {
                xhr.setRequestHeader('x-ms-blob-content-type', metadata.fileType);
                xhr.setRequestHeader('Content-Length', requestBody.length);
                //add metadata tags too
                xhr.setRequestHeader('x-ms-meta-title', metadata.title);
                xhr.setRequestHeader('x-ms-meta-description', metadata.description);

            },
            success: function (data, status) {
                console.log(data);
                console.log(status);
                
            },
            error: function (xhr, desc, err) {
                console.log(desc);
                console.log(err);
            }
        });

    }

    //the object returned when you "new" a MediaUploader"
    //provides a single function to begin the upload and provide a callback
    //for progress
    //metadata and file are provided in the "constructor" function
    return {
        beginUpload: function (progressCallback) {
            progressCB = progressCallback;
            //initial progress indicator
            progressCallback({ totalBytes: totalBytes, currentBytes: 0 });
            uploadBlock();
        }
    }
}