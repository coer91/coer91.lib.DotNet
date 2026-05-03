using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog; 

namespace coer91.NET
{
    public static class Logger
    {
        public static bool UseLogger { get; private set; } = false; 

        public static IHostBuilder AddLogger(this IHostBuilder host, IConfiguration configuration)
        {
            bool useLogger = !string.IsNullOrWhiteSpace(configuration["Logger:Enable"]) && configuration["Logger:Enable"].Equals("true", StringComparison.CurrentCulture);
           
            if (useLogger)
            {
                string path = configuration.GetSection("Logger:Path").Get<string>() ?? "../Logger/.log";
                string template = configuration.GetSection("Logger:Template").Get<string>() ?? "[{Level}][{Timestamp:yyyy-MM-dd HH:mm:ss zzz}]{NewLine}{Message}{NewLine}{NewLine}";
                int? retainedFiles = configuration.GetSection("Logger:retainedFiles").Get<int?>();

                host.UseSerilog((builder, configuration) => configuration
                    .WriteTo.Console(
                        outputTemplate: template
                    )
                    .WriteTo.File(
                        path: path,
                        outputTemplate: template,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: retainedFiles,
                        shared: true
                    )
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("System", global::Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft", global::Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.Hosting.LifeTime", global::Serilog.Events.LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                );

                UseLogger = useLogger;
            }

            return host;
        }


        public static IHostBuilder AddLogger(this IHostBuilder host, bool useLogger = false)
        { 
            if (useLogger)
            {
                
                host.UseSerilog((builder, configuration) => configuration
                    .WriteTo.Console(
                        outputTemplate: "[{Level}][{Timestamp:yyyy-MM-dd HH:mm:ss zzz}]{NewLine}{Message}{NewLine}{NewLine}"
                    )
                    .WriteTo.File(
                        path: "../Logger/.log",
                        outputTemplate: "[{Level}][{Timestamp:yyyy-MM-dd HH:mm:ss zzz}]{NewLine}{Message}{NewLine}{NewLine}",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        shared: true
                    )
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("System", global::Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft", global::Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.Hosting.LifeTime", global::Serilog.Events.LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                );

                UseLogger = useLogger;
            }

            return host;
        }


        public static void Information(string message) 
        {
            if (UseLogger) Log.Information(message);
        }


        public static void Warning(string message)
        {
            if (UseLogger) Log.Warning(message);
        }


        public static void Error(string message)
        {
            if (UseLogger) Log.Error(message);
        }
    }
}