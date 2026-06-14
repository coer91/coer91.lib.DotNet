using Microsoft.AspNetCore.Http; 

namespace coer91.NET.Files
{
    internal static class FilesCSV
    {
        public static readonly string[] EXTENSIONS = ["xls", "xlsx", "csv"];
        public static readonly string[] CONTENT_TYPES = ["application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.ms-excel.sheet.macroEnabled.12"];


         
        public static bool IsCSV(IFormFile file)
            => Array.Exists(EXTENSIONS, extension => extension.Equals(Files.GetExtension(file)));
    }
} 