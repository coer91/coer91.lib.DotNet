using Microsoft.AspNetCore.Http;

namespace coer91.NET.Files
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

         
        public class Image
        {
            public static readonly string[] EXTENSIONS    = FilesIMAGE.EXTENSIONS;
            public static readonly string[] CONTENT_TYPES = FilesIMAGE.CONTENT_TYPES;


            /// <summary>
            /// Validate if is an image type file
            /// </summary> 
            public static bool IsImage(IFormFile file)
                => FilesIMAGE.IsImage(file);


            /// <summary>
            /// Convert a image file to byte array
            /// </summary> 
            /// <exception cref="ArgumentOutOfRangeException"></exception>
            /// <exception cref="FormatException"></exception> 
            public static byte[] GetBytes(IFormFile image, int maxMB = 4)
                => FilesIMAGE.GetBytes(image, maxMB);


            /// <summary>
            /// Convert a byte array to Base64
            /// </summary>
            public static string ToBase64(byte[] image, string noImage = "", string extension = "png")
                => FilesIMAGE.ToBase64(image, noImage, extension);
        }


        public class PDF 
        {
            public class TEMPLATE_TYPE : PDF_TEMPLATE_TYPE { }


            public static FileDTO GenerateDocument(DocumentPdf documentPdf, object data = null, int offset = 0)
                => FilesPDF.GenerateDocument(documentPdf, data, offset);
        }
    }
}