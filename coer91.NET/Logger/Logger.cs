using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog; 

namespace coer91.NET
{
    public static class Logger
    {
        public static bool UseLogger { get; private set; } = false;

        public static IHostBuilder AddLogger(this IHostBuilder host, IConfiguration configuration)
            => AddLogger(host, !string.IsNullOrWhiteSpace(configuration["UseLogger"]) && configuration["UseLogger"].Equals("true", StringComparison.CurrentCulture));

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


        public static void Wrning(string message)
        {
            if (UseLogger) Log.Warning(message);
        }


        public static void Error(string message)
        {
            if (UseLogger) Log.Error(message);
        }
    }
}