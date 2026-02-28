using coer91.NET; 

/* Builder Configuration */
var builder = WebApplication.CreateBuilder(args);
builder.Host.AddLogger(builder.Configuration);
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

//Dependency Injection 
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//builder.Services.AddDBConnection(builder.Configuration);
//builder.Services.AddRepositoryCollection();
//builder.Services.AddMicroserviceCollection();
//builder.Services.AddExceptionFilter();
//builder.Services.AddLogCode500();

//Security 
Security security = new(builder);
security.AddSwagger("My System").Set();
security.AddAuthenticationBearer().Set();
security.AddCors().Set();

//Web Application
WebApplication app = builder.Build();
app.UseSwagger(true);
app.UseCors("coer91");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
//app.UseLogCode500();
app.MapControllers();
app.Run();