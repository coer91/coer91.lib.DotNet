using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace coer91.NET
{
    public class Security
    {
        private readonly WebApplicationBuilder _builder;

        public static string ProjectName { get; private set; } = Assembly.GetEntryAssembly()?.GetName()?.Name;
        public static bool IsDevelopment { get; private set; } 
        public static bool IsStaging { get; private set; }  
        public static bool IsProduction { get; private set; }  


        public Security(WebApplicationBuilder builder) 
        {
            _builder = builder;
            IsDevelopment = _builder.Environment.IsDevelopment();
            IsStaging = _builder.Environment.IsStaging();
            IsProduction = _builder.Environment.IsProduction();
        }


        public SwaggerConfigurationBuilder AddSwagger(string title = "")
        {
            if (string.IsNullOrWhiteSpace(title))
                title = ProjectName;

            else
                ProjectName = title;  

            return new(title, _builder); 
        }


        public BearerConfigurationBuilder AddAuthenticationBearer(string secretKey = "") => new(secretKey, _builder);


        public CorsConfigurationBuilder AddCors(string policy = "coer91.NET") => new(policy, _builder);


        #region static
        public static JWTBuilder JWT(string secretKey = "") => new(secretKey);


        public static JWTBuilder JWT(IConfiguration configuration) => new(configuration.GetSection("Security:SecretKey").Get<string>());


        public static IEnumerable<Claim> GetClaims(IHttpContextAccessor httpContextAccessor) => GetClaims(httpContextAccessor?.HttpContext);


        public static IEnumerable<Claim> GetClaims(HttpContext context) => context is not null ? (context.User?.Claims ?? []) : [];


        public static string GetClaimValue(string type, IHttpContextAccessor httpContextAccessor) => GetClaimValue(type, httpContextAccessor?.HttpContext);


        public static string GetClaimValue(string type, HttpContext context) => GetClaims(context).FirstOrDefault(_claim => _claim.Type == type)?.Value ?? string.Empty;


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
        #endregion
    }
} 