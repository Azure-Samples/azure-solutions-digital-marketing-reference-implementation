using AzureKit.Config;
using System;
using System.Drawing;
using System.IO;

namespace AzureKit.Media
{
    public class MediaUtilities
    {
        public static void CreateThumbnailForImage(Stream inputStream,Stream outputStream)
        {
            if (inputStream == null || !inputStream.CanRead)
            {
                throw new ArgumentException("Must provide a readable input stream", nameof(inputStream));
            }

            if (outputStream == null || !outputStream.CanWrite)
            {
                throw new ArgumentException("Must provide a writable output stream", nameof(outputStream));
            }

            using (Image inputImage = Image.FromStream(inputStream))
            {
                int targetHeight = Constants.DEFAULT_THUMBNAIL_HEIGHT;
                var adjustedWidth = (((float)Constants.DEFAULT_THUMBNAIL_HEIGHT / (float)inputImage.Height) * (inputImage.Width));
                int targetWidth = (int)adjustedWidth;

                using (Bitmap outputImage = new Bitmap(inputImage, targetWidth, targetHeight))
                {
                    outputImage.Save(outputStream, inputImage.RawFormat);
                }
            }
        }

        public static string CreateThumbnailFileName(string sourceImageFileName)
        {
            var originalExtension = System.IO.Path.GetExtension(sourceImageFileName);
            var originalFilename = System.IO.Path.GetFileNameWithoutExtension(sourceImageFileName);
            return originalFilename + "-thumb" + originalExtension;
        }
    }
}