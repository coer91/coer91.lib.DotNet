using System.Text; 
using System.Reflection;  
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting; 
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.OpenApi;

namespace coer91
{
    public class Security
    {
        public static string ProjectName { get; private set; } = Assembly.GetEntryAssembly()?.GetName()?.Name;

        private readonly IServiceCollection _services;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _envirotment;

        public Security(
            IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment envirotment
        ) {
            _services = services;
            _configuration = configuration;
            _envirotment = envirotment;
        }


        public void AddSwaggerConfiguration(string title = "", string version = "")
        {
            if (string.IsNullOrWhiteSpace(title))
                title = ProjectName;

            else
                ProjectName = title;

            if (string.IsNullOrWhiteSpace(version))
                version = _configuration.GetSection("Version").Get<string>() ?? string.Empty; 

            if (_envirotment.IsDevelopment())
                title += " - Development";

            else if (_envirotment.IsStaging())
                title += " - Staging";

            _services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo { Title = title, Version = version });
                config.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                config.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                config.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("bearer", document)] = []
                });
            });
        }


        public BearerConfigurationBuilder Bearer() => new(_services, _configuration);  


        public void AddCorsConfiguration(string policy = "coer91Policy", CorsPolicyBuilder policyBuilder = null)
        {
            _services.AddCors(options =>
            {
                var builder = policyBuilder ?? new CorsPolicyBuilder()
                   .SetIsOriginAllowed(origin => true)
                   .AllowCredentials()
                   .AllowAnyMethod()
                   .AllowAnyHeader();

                options.AddPolicy(policy, builder.Build());
            });
        } 


        public static string GeneratePassword()
        {
            const int length = 4;
            string LOWER = "abcdefghijklmnopqrstuvwxyz";
            string UPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string NUMBERS = "1234567890";
            string SPECIALS = "@$#_!=*";

            char randomChar;
            Random random = new();
            StringBuilder passwordBuilder = new();

            for (int i = 0; i < length; i++)
            {
                randomChar = LOWER[random.Next(LOWER.Length)];
                passwordBuilder.Append(randomChar);
                LOWER = LOWER.Replace(randomChar.ToString(), "");
            }

            for (int i = 0; i < length; i++)
            {
                randomChar = UPPER[random.Next(UPPER.Length)];
                passwordBuilder.Append(randomChar);
                UPPER = UPPER.Replace(randomChar.ToString(), "");
            }

            for (int i = 0; i < length; i++)
            {
                randomChar = NUMBERS[random.Next(NUMBERS.Length)];
                passwordBuilder.Append(randomChar);
                NUMBERS = NUMBERS.Replace(randomChar.ToString(), "");
            }

            for (int i = 0; i < length; i++)
            {
                randomChar = SPECIALS[random.Next(SPECIALS.Length)];
                passwordBuilder.Append(randomChar);
                SPECIALS = SPECIALS.Replace(randomChar.ToString(), "");
            }

            _ = new Random();
            char[] caracters = passwordBuilder.ToString().ToCharArray();

            //Fisher-Yates
            for (int i = caracters.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (caracters[j], caracters[i]) = (caracters[i], caracters[j]);
            }

            return new string(caracters);
        }
    }
} 