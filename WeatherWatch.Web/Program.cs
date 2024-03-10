//using App.Metrics;
//using App.Metrics.Reporting.Wavefront.Builder;
//using StackExchange.Redis;
//using System.Collections.Immutable;
//using Tap.Dotnet.Weather.Application;
//using Tap.Dotnet.Weather.Application.Interfaces;
//using Tap.Dotnet.Weather.Application.Models;
//using Wavefront.SDK.CSharp.Common;
//using Wavefront.SDK.CSharp.Common.Application;
//using Wavefront.SDK.CSharp.DirectIngestion;

using WeatherWatch.Application;
using WeatherWatch.Application.Interfaces;
using WeatherWatch.Application.Models;

var builder = WebApplication.CreateBuilder(args);

//var serviceBindings = Environment.GetEnvironmentVariable("SERVICE_BINDING_ROOT") ?? String.Empty;

// read environment variables
var weatherApi = Environment.GetEnvironmentVariable("WEATHER_API") ?? String.Empty;

//// read secrets from files
//var wavefrontUrl = System.IO.File.ReadAllText(Path.Combine(serviceBindings, "wavefront-api-resource-claim", "host"));
//var wavefrontToken = System.IO.File.ReadAllText(Path.Combine(serviceBindings, "wavefront-api-resource-claim", "token"));
//var cacheHost = System.IO.File.ReadAllText(Path.Combine(serviceBindings, "redis-cache-class-claim", "host"));
//var cachePort = System.IO.File.ReadAllText(Path.Combine(serviceBindings, "redis-cache-class-claim", "port"));
//var cachePassword = System.IO.File.ReadAllText(Path.Combine(serviceBindings, "redis-cache-class-claim", "password"));

//var cacheConfig = $"{cacheHost}:{cachePort},password={cachePassword}";

//// setup Redis cache
//var redisConnection = ConnectionMultiplexer.Connect(cacheConfig);
//// var cacheServer = redisConnection.GetServer(cacheConnection);
//var cacheDb = redisConnection.GetDatabase();

//builder.Services.AddSingleton<IDatabase>(cacheDb);

// setup weather api service
builder.Services.AddSingleton<IWeatherApi>(new WeatherApi() { Url = weatherApi });

//// setup wavefront service
//var wfSender = new WavefrontDirectIngestionClient.Builder(wavefrontUrl, wavefrontToken).Build();

//builder.Services.AddSingleton<WavefrontDirectIngestionClient>(wfSender);

// weather app
builder.Services.AddSingleton<IWeatherWatchApplication, WeatherWatchApplication>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

//app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

