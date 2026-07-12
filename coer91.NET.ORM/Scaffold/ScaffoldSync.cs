using coer91.NET.Files;
using System.Diagnostics;

namespace coer91.NET.ORM
{
    public class ScaffoldSync
    {
        public static async Task SQLServerSync(ScaffoldProfile profile) 
        {
            Console.WriteLine("Build started...");

            if (string.IsNullOrWhiteSpace(profile.ConnectionString))
                throw new ArgumentException("Connection string not provided"); 
                        
            string connection = profile.ConnectionString.Contains(';') && profile.ConnectionString.Contains("Server", StringComparison.OrdinalIgnoreCase) && profile.ConnectionString.Contains("Catalog", StringComparison.OrdinalIgnoreCase)
                ? $"\"{profile.ConnectionString}\"" : $"Name={profile.ConnectionString}";

            using Process process = new()
            {
                StartInfo = new()
                {
                    FileName = "dotnet",
                    Arguments = "ef dbcontext scaffold " 
                    + $"{connection} "
                    + $"Microsoft.EntityFrameworkCore.SqlServer "
                    + $"--startup-project {profile.StartupProject} "
                    + $"--project {profile.Project} "
                    + $"--namespace {profile.ContextNamespace} "
                    + $"--output-dir {profile.ContextOutput} "
                    + $"--context-namespace {profile.ContextNamespace} "
                    + $"--context-dir {profile.ContextOutput} "
                    + $"--context {profile.ContextName} "
                    + "--no-onconfiguring "
                    + "--force",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = FilesManagement.GetSolutionPath(),
                    EnvironmentVariables = { ["DOTNET_ENVIRONMENT"] = "Development" }
                } 
            };
             
            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync();
            string error  = await process.StandardError.ReadToEndAsync();

            if (!string.IsNullOrWhiteSpace(error))
            {
                Console.WriteLine("Scaffold Errors:");
                Console.WriteLine("\nTry to install: dotnet tool install --global dotnet-ef");
                Console.WriteLine(error);
                Console.ReadKey();
                Environment.Exit(0);
            } 
        }
    }
}