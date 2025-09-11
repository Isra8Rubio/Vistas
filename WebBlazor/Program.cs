using MudBlazor.Services;
using WebBlazor.Components;

var builder = WebApplication.CreateBuilder(args);

// === HttpClient apuntando a tu API ===
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7049/";
builder.Services.AddScoped<HttpClient>(_ =>
{
    var client = new HttpClient
    {
        BaseAddress = new Uri(apiBaseUrl)
    };
    return client;
});

// MudBlazor
builder.Services.AddMudServices();

// Razor Components (Blazor Server)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
