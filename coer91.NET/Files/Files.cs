using Microsoft.AspNetCore.Http;

namespace coer91.NET
{
    public static class Files
    {
        /// <summary>
        /// Get Extension File
        /// </summary> 
        /// <exception cref="FormatException"></exception> 
        public static string GetExtension(IFormFile file)
        {
            if (file.FileName.Contains('.'))
            {
                string[] worlds = file.FileName.Split('.');
                if (worlds.Length > 0)
                {
                    string extension = worlds.TakeLast(1).FirstOrDefault();
                    extension = extension.CleanUpBlanks();
                    extension = extension.ToLower();
                    if (extension.Length > 0) return extension;
                }
            }

            throw new FormatException("The file extension could not be recognized");
        }


        public class CSV 
        {
            public static readonly string[] EXTENSIONS = FilesCSV.EXTENSIONS;
            public static readonly string[] CONTENT_TYPES = FilesCSV.CONTENT_TYPES;

            /// <summary>
            /// Validate if is csv type file
            /// </summary>
            public static bool IsCSV(IFormFile file)
                => FilesCSV.IsCSV(file);
        }

         
        public static class Images
        {
            public static readonly string[] EXTENSIONS = FilesImages.EXTENSIONS;
            public static readonly string[] CONTENT_TYPES = FilesImages.CONTENT_TYPES;

            /// <summary>
            /// Validate if is an image type file
            /// </summary> 
            public static bool IsImage(IFormFile file)
                => FilesImages.IsImage(file);


            /// <summary>
            /// Convert a image file to byte array
            /// </summary> 
            /// <exception cref="ArgumentOutOfRangeException"></exception>
            /// <exception cref="FormatException"></exception> 
            public static byte[] ToImageBytes(IFormFile image, int maxMB = 4)
                => FilesImages.ToImageBytes(image, maxMB);


            /// <summary>
            /// Convert a byte array to Base64
            /// </summary>
            public static string ToImageBase64(byte[] image, string noImage = "", string extension = "png")
                => FilesImages.ToImageBase64(image, noImage, extension);


            /// <summary>
            ///  
            /// </summary>
            public static string GenerateBarcodeBase64(object value, NetBarcode.Type barcodeType = NetBarcode.Type.Code128)
                => FilesImages.GenerateBarcodeBase64(value, barcodeType);


            /// <summary>
            ///  
            /// </summary>
            public static string GenerateBarcode(object value, string width = null, string height = null, string showCaption = null, string captionAlign = null, string caption = null)
                => FilesImages.GenerateBarcode(value, width, height, showCaption, captionAlign, caption);


            /// <summary>
            ///  
            /// </summary>
            public static string GenerateQRBase64(object value)
                => FilesImages.GenerateQRBase64(value);


            /// <summary>
            ///  
            /// </summary>
            public static string GenerateQR(object value, string size = null, string showCaption = null, string captionAlign = null, string caption = null)
                => FilesImages.GenerateQR(value, size, showCaption, captionAlign, caption);
        } 
    }
}