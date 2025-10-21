using Api.Service;
using Api.Service.GenesysAPI;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using GenesysConfig = Api.Service.GenesysAPI.Models.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });
builder.Services.AddOpenApiDocument(o => { o.Title = "Genesys API"; o.Version = "v1"; });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
    c.CustomSchemaIds(t => t.FullName?.Replace("+", "."));
});

builder.Services.AddHttpClient();

// Configuración de Genesys
builder.Services.AddSingleton<GenesysConfig>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var genesysSection = cfg.GetSection("GenesysAPI");

    var config = new GenesysConfig
    {
        GenesysClient = genesysSection["GenesysClient"] ?? string.Empty,
        GenesysSecret = genesysSection["GenesysSecret"] ?? string.Empty,
    };
    return config;
});

// Servicios
builder.Services.AddSingleton<GenesysAuthService>();
builder.Services.AddScoped<DivisionApiService>();
builder.Services.AddScoped<UserApiService>();
builder.Services.AddScoped<ConversationApiService>();
builder.Services.AddScoped<RoutingApiService>();
builder.Services.AddScoped<GroupApiService>();
builder.Services.AddScoped<CallService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();
app.MapControllers();
app.Run();