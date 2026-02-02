using Microsoft.AspNetCore.Http;

namespace coer91
{
    public static class Files
    {
        public static readonly string[] CSV_EXTENSIONS = ["xls", "xlsx", "csv"];
        public static readonly string[] CSV_CONTENT_TYPES = ["application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.ms-excel.sheet.macroEnabled.12"];
        

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


        /// <summary>
        /// Validate if is csv type file
        /// </summary> 
        public static bool IsCSV(IFormFile file)
            => Array.Exists(CSV_EXTENSIONS, extension => extension.Equals(GetExtension(file)));


        #region IMAGE

        public static readonly string[] IMAGES_EXTENSIONS = ["png", "jpg", "jpeg", "gif", "svg", "ico"];
        public static readonly string[] IMAGES_CONTENT_TYPES = ["image/png", "image/jpeg", "image/gif", "image/svg+xml"];

        /// <summary>
        /// Validate if is an image type file
        /// </summary> 
        public static bool IsImage(IFormFile file)
            => Array.Exists(IMAGES_EXTENSIONS, extension => extension.Equals(GetExtension(file)));

       
        /// <summary>
        /// Convert a image file to byte array
        /// </summary> 
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception> 
        public static byte[] ToImageBytes(IFormFile image, int maxMB = 4)
        {
            if (image.Length > (maxMB * 1048576)) //1 MB = 1048576 Bytes
                throw new ArgumentOutOfRangeException(string.Empty, $"The maximun size allowed is <b>{maxMB}mb</b>");

            //Validate Format             
            if (IsImage(image))
            {
                using MemoryStream memoryStream = new();
                image.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }

            //Error Message
            string message = $"Only the following formats are accepted:<br>";

            for (int i = 0; i < IMAGES_CONTENT_TYPES.Length; i++)
            {
                message += $" - {IMAGES_CONTENT_TYPES[i]}";
                message += (i == (IMAGES_CONTENT_TYPES.Length - 1)) ? string.Empty : "<br>";
            }

            throw new FormatException(message);
        }


        /// <summary>
        /// Convert a byte array to Base64
        /// </summary>
        public static string ToImageBase64(byte[] image, string noImage = "", string extension = "png")
            => (image is not null) ? $"data:image/{extension};base64,{Convert.ToBase64String(image)}" : noImage;
        #endregion
    }
}