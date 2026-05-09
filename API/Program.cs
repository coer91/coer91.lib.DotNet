using coer91.NET; 

/* Builder Configuration */
var builder = WebApplication.CreateBuilder(args); 

//Dependency Injection 
builder.Services.AddSetupCollection();  

//Security 
Security security = new(builder);
security.AddSwagger("coer91.NET").SetSecurityDefinitionBearer().Build();
security.AddAuthenticationBearer().Build();
security.AddCors().Build();
security.AddLogger(true);
security.AddControllers();
security.Run();