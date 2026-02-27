using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens; 
using System.Text;

namespace coer91.NET
{
    public class BearerConfigurationBuilder(string _secretKey, WebApplicationBuilder _builder) 
    {  
        protected bool _setToControllers = false;
        protected bool _validateIssuer = false;
        protected bool _validateAudience = false;
        protected bool _validateLifetime = true;
        protected bool _validateIssuerSigningKey = true;
        protected TimeSpan _clockSkew = TimeSpan.Zero;

        public BearerConfigurationBuilder SetToControllers(bool setToControllers)
        {
            _setToControllers = setToControllers;
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

        public BearerConfigurationBuilder ClockSkew(TimeSpan clockSkew)
        {
            _clockSkew = clockSkew;
            return this;
        }

        public void Set()
        {  
            if(string.IsNullOrWhiteSpace(_secretKey)) 
                _secretKey = _builder.Configuration.GetSection("Security:SecretKey").Get<string>() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(_secretKey)) return;  

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
                    ClockSkew = _clockSkew
                }); 

            if (_setToControllers) _builder.Services.AddControllers(config =>
            {
                AuthorizationPolicy policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                AuthorizeFilter filter = new(policy);
                config.Filters.Add(filter);
            });
        }
    }
} 