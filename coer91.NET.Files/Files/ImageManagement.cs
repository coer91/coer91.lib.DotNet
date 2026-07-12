using Microsoft.AspNetCore.Http;

namespace coer91.NET.Files
{
    public static class ImageManagement
    {
        public static readonly string[] EXTENSIONS = ["png", "jpg", "jpeg", "gif", "svg", "ico"];
        public static readonly string[] CONTENT_TYPES = ["image/png", "image/jpeg", "image/gif", "image/svg+xml"];

        
        public static bool IsImage(IFormFile file)
            => Array.Exists(EXTENSIONS, extension => extension.Equals(FilesManagement.GetExtension(file)));

               
        public static byte[] GetBytes(IFormFile image, int maxMB = 4)
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

            for (int i = 0; i < CONTENT_TYPES.Length; i++)
            {
                message += $" - {CONTENT_TYPES[i]}";
                message += (i == (CONTENT_TYPES.Length - 1)) ? string.Empty : "<br>";
            }

            throw new FormatException(message);
        }

               
        public static string ToBase64(byte[] image, string noImage = "", string extension = "png")
            => (image is not null) ? $"data:image/{extension};base64,{Convert.ToBase64String(image)}" : noImage;
    }
}