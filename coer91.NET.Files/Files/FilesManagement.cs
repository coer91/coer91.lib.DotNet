using Microsoft.AspNetCore.Http; 

namespace coer91.NET.Files
{
    public static class FilesManagement
    {

        public static string GetExtension(IFormFile file)
        {             
            if (file.FileName.Contains('.'))
            {
                string[] worlds = file.FileName.Split('.');
                if (worlds.Length > 0)
                { 
                    string extension = worlds.TakeLast(1).FirstOrDefault();
                    extension =  extension.CleanUpBlanks();
                    extension = extension.ToLower();
                    if (extension.Length > 0) return extension;
                }
            }

            throw new FormatException("The file extension could not be recognized");
        }


        public static string GetSolutionPath()
        {
            DirectoryInfo directory = new(Directory.GetCurrentDirectory());

            do
            {
                if (directory.GetFiles().Any(x => x.Extension.StartsWith(".sln")))
                    return directory.FullName.Replace("\\","/");

                directory = directory.Parent;
            } while (directory is not null);

            return string.Empty;
        }


        public static string CreateDirectory(string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                return directoryPath;
            }

            catch
            {
                return null;
            }          
        }
    }
}