using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection; 

namespace coer91.NET
{
    public class CorsConfigurationBuilder(string _policy, WebApplicationBuilder _builder)
    {
        private readonly string[] HTTP_METHODS = ["GET", "POST", "PUT", "PATCH", "DELETE"];

        protected bool _allowCredentials = false;         
        protected bool _allowAnyHeader = true;
        protected bool _allowAnyMethod = true;
        protected bool _allowAnyDomain = true;
        protected bool _allowAnyOrigin = true;
        protected string[] _headers = [];
        protected string[] _methods = [];
        protected string[] _domains = [];
        protected string[] _origins = [];


        public CorsConfigurationBuilder AllowedMethods(params string[] methods)
        {
            _methods = [.. methods.Select(x => x.ToUpper()).Where(x => HTTP_METHODS.Contains(x))];
            _allowAnyMethod = false;
            return this;
        }
        
        public CorsConfigurationBuilder AllowedHeaders(params string[] headers)
        {
            _headers = [.. headers];
            _allowAnyHeader = false;
            return this;
        } 

        public CorsConfigurationBuilder AllowedDomains(params string[] domains)
        {
            _domains = [.. domains];
            _allowAnyDomain = false;
            return this;
        } 

        public CorsConfigurationBuilder AllowedOrigins(params string[] origins)
        {
            _origins = [.. origins];
            _allowAnyOrigin = false;
            return this;
        }
         

        public void Build() 
        { 
            _builder.Services.AddCors(options =>
            {
                CorsPolicyBuilder builder = new();

                //METHODS
                if (_allowAnyMethod) builder.AllowAnyMethod();
                else if (_methods.Length > 0) builder.WithMethods(_methods);

                //HEADERS
                if (_allowAnyHeader) builder.AllowAnyHeader();
                else if(_headers.Length > 0) builder.WithHeaders(_headers);
                
                //CREDENTIALS
                if (_allowCredentials) builder.AllowCredentials();

                //ORIGIN & DOMAIN
                if (_allowAnyOrigin) 
                {
                    if (_allowAnyDomain) builder.SetIsOriginAllowed(origin => true);
                    else builder.SetIsOriginAllowed(origin => _domains.Any(domain => origin.StartsWith(domain))); 
                }
                
                else if(_origins.Length > 0) builder.WithOrigins(_origins);

                options.AddPolicy(_policy, builder.Build());
            });
        }
    }
} 