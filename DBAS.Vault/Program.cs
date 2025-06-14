using Asp.Versioning;
using DBAS.Vault.Database;
using DBAS.Vault.Database.Interfaces;
using DBAS.Vault.Initializer;
using DBAS.Vault.Jwt;
using DBAS.Vault.Jwt.Interfaces;
using DBAS.Vault.Models;
using DBAS.Vault.Services;
using DBAS.Vault.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<APIEnvironment>(builder.Configuration.GetSection(nameof(APIEnvironment)));
var env = Initializer.GetAPIEnvironment(builder.Configuration);

Initializer.Initialize(builder.Services, env);
builder.Services.AddSingleton(env);
builder.Services.AddScoped<IDatabase, MySqlDb>();
builder.Services.AddScoped<IEncryption, Encryption>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ISecretService, SecretService>();

builder.Services.AddHealthChecks().AddCheck(nameof(HealthCheck), new HealthCheck(env), HealthStatus.Unhealthy, [nameof(HealthCheck)]);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    options.SerializerSettings.Formatting = Formatting.None;
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.SerializerSettings.MaxDepth = null;
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(v =>
{
    v.DefaultApiVersion = new ApiVersion(env.Version.Major, env.Version.Minor);
    v.AssumeDefaultVersionWhenUnspecified = true;
    v.ReportApiVersions = true;
});
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.RequireAuthenticatedSignIn = env.ValidateIssuerSigningKey;
})
.AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;
    o.TokenValidationParameters = Encryption.GetTokenValidationParameters(env);
});
builder.Services.AddAuthorization();
builder.Services.AddResponseCompression();

var app = builder.Build();

app.UseMiddleware<CustomMiddleware>();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapControllerRoute("default", "{controller}/{action}");
});

app.UseHttpsRedirection();
app.UseResponseCompression();
app.Run();