using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Web;
using Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Point at the running API (see API/Properties/launchSettings.json).
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7167/") });
builder.Services.AddScoped<TokenStore>();
builder.Services.AddScoped<ApiClient>();
builder.Services.AddMudServices();

await builder.Build().RunAsync();
