using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens; 
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace coer91
{
    public class BearerConfigurationBuilder(WebApplicationBuilder _builder) { 

        protected string _secretKey = null;
        protected bool _setAutorizationToControllers = false;
        protected bool _validateIssuer = false;
        protected bool _validateAudience = false;
        protected bool _validateLifetime = true;
        protected bool _validateIssuerSigningKey = true;

        public BearerConfigurationBuilder SetSecretKey(string secretKey)
        {
            _secretKey = secretKey;
            return this;
        }

        public BearerConfigurationBuilder SetAutorizationToControllers(bool setAutorizationToControllers)
        {
            _setAutorizationToControllers = setAutorizationToControllers;
            return this;
        }

        public BearerConfigurationBuilder ValidateIssuer(bool validateIssuer)
        {
            _validateIssuer = validateIssuer;
            return this;
        }

        public BearerConfigurationBuilder ValidateAudience(bool validateAudience)
        {
            _validateAudience = validateAudience;
            return this;
        }

        public BearerConfigurationBuilder ValidateLifetime(bool validateLifetime)
        {
            _validateLifetime = validateLifetime;
            return this;
        }

        public BearerConfigurationBuilder ValidateIssuerSigningKey(bool validateIssuerSigningKey)
        {
            _validateIssuerSigningKey = validateIssuerSigningKey;
            return this;
        }


        public void AddConfiguration()
        { 
            //SecretKey
            if(string.IsNullOrWhiteSpace(_secretKey)) 
                _secretKey = _builder.Configuration.GetSection("Security:SecretKey").Get<string>() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(_secretKey)) return; 
                        
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            _builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = _validateIssuer,
                    ValidateAudience = _validateAudience,
                    ValidateLifetime = _validateLifetime,
                    ValidateIssuerSigningKey = _validateIssuerSigningKey,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
                    ClockSkew = TimeSpan.Zero
                }); 

            if (_setAutorizationToControllers) _builder.Services.AddControllers(config =>
            {
                AuthorizationPolicy policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                AuthorizeFilter filter = new(policy);
                config.Filters.Add(filter);
            });
        }
    }
} 