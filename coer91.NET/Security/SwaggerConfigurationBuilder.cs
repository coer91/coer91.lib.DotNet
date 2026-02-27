using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;

namespace coer91.NET
{
    public class SwaggerConfigurationBuilder(string _title, WebApplicationBuilder _builder)
    {

        protected string _version = "";
        protected string _documentName = "v1";
        protected bool _securityDefinitionBearer = false;
        protected bool _setComments = false;

        protected string _name = "Authorization";
        protected SecuritySchemeType _type = SecuritySchemeType.ApiKey;
        protected string _scheme = "bearer";
        protected string _format = "JWT";
        protected ParameterLocation _location = ParameterLocation.Header; 
        protected string _description = "";


        public SwaggerConfigurationBuilder SetVersion(string version)
        {
            _version = version;
            return this;
        }

        public SwaggerConfigurationBuilder SetDocumentName(string documentName)
        {
            _documentName = documentName;
            return this;
        }

        public SwaggerConfigurationBuilder SetSecurityDefinitionBearer(bool securityDefinition = true)
        {
            _securityDefinitionBearer = securityDefinition;
            return this;
        }

        public SwaggerConfigurationBuilder SetComments(bool setComments = true)
        {
            _setComments = setComments;
            return this;
        }

        public SwaggerConfigurationBuilder SetSecurityName(string name)
        {
            _name = name;
            return this;
        }

        public SwaggerConfigurationBuilder SetSecurityType(SecuritySchemeType type)
        {
            _type = type;
            return this;
        }

        public SwaggerConfigurationBuilder SetSecurityScheme(string scheme)
        {
            _scheme = scheme;
            return this;
        }

        public SwaggerConfigurationBuilder SetSecurityFormat(string format)
        {
            _format = format;
            return this;
        }

        public SwaggerConfigurationBuilder SetSecurityLocation(ParameterLocation location)
        {
            _location = location;
            return this;
        }

        public SwaggerConfigurationBuilder SetSecurityDescription(string description)
        {
            _description = description;
            return this;
        } 

        public void Set() 
        {
            if (_builder.Environment.IsDevelopment())
                _title += " - Development";

            else if (_builder.Environment.IsStaging())
                _title += " - Staging";

            if (string.IsNullOrWhiteSpace(_version))
                _version = _builder.Configuration.GetSection("Version").Get<string>() ?? "0.0.0";

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            _builder.Services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc(_documentName, new OpenApiInfo { Title = _title, Version = _version });
                config.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                if (_securityDefinitionBearer) 
                { 
                    config.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                    {
                        Name = _name,
                        Type = _type,
                        Scheme = _scheme,
                        BearerFormat = _format,
                        In = _location,
                        Description = _description
                    });

                    config.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("bearer", document)] = []
                    }); 
                }

                if (_setComments) 
                {
                    foreach (string xmlPath in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly))
                        config.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }
            });
        }
    }
} 