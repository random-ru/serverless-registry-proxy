using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using rpnpm;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost
    .UseUrls($"http://*.*.*.*:{Environment.GetEnvironmentVariable("PORT") ?? "9080"}");

builder.Services
    .AddOcelot()
    .AddDelegatingHandler<GoogleHandler>(true);
builder.Services
    .AddAuthentication(x => x.DefaultAuthenticateScheme = "GoogleSSE")
    .AddScheme<GoogleSSEAuthOptions, GoogleSSEAuthHandler>("GoogleSSE", o => { });

var app = builder.Build();

await app.UseOcelot();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseForwardedHeaders();

app.Run();