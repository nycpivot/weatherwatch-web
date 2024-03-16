﻿using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
//using StackExchange.Redis;
using System.Diagnostics;
using WeatherWatch.Application.Interfaces;
using WeatherWatch.Application.Models;
using WeatherWatch.Web.Models;

namespace WeatherWatch.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly DaprClient daprClient;
        //private readonly IDatabase cache;
        
        private readonly IWeatherWatchApplication weatherApplication;
        //private readonly WavefrontDirectIngestionClient wavefrontSender;
        private readonly ILogger<HomeController> logger;

        //public HomeController(
        //    IDatabase cache, IWeatherWatchApplication weatherApplication,
        //    WavefrontDirectIngestionClient wavefrontSender, ILogger<HomeController> logger)
        //{
        //    this.cache = cache;
        //    this.weatherApplication = weatherApplication;
        //    this.wavefrontSender = wavefrontSender;
        //    this.logger = logger;
        //}

        public HomeController(
            DaprClient daprClient,
            IWeatherWatchApplication weatherApplication,
            ILogger<HomeController> logger)
        {
            this.daprClient = daprClient;
            this.weatherApplication = weatherApplication;
            this.logger = logger;
        }

        public IActionResult Index(HomeViewModel model)
        {
            var traceId = Guid.NewGuid();
            var spanId = Guid.NewGuid();

            //var start = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);
            //Thread.Sleep(100);
            //var end = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);

            //this.wavefrontSender.SendSpan(
            //    "Index", start, end, "tap-dotnet-weather-web", traceId, Guid.NewGuid(),
            //    ImmutableList.Create(new Guid("82dd7b10-3d65-4a03-9226-24ff106b5041")), null,
            //    ImmutableList.Create(
            //        new KeyValuePair<string, string>("application", "tap-dotnet-weather-web"),
            //        new KeyValuePair<string, string>("service", "Index"),
            //        new KeyValuePair<string, string>("zipcode", model.WeatherInfo.ZipCode),
            //        new KeyValuePair<string, string>("http.method", "GET")), null);

            var homeViewModel = this.weatherApplication.GetForecast(model.WeatherInfo.ZipCode, traceId, spanId);

            homeViewModel.Recents = Cache(model.WeatherInfo.ZipCode);

            return View(homeViewModel);
        }

        [HttpPost]
        public ActionResult Search(HomeViewModel model)
        {
            var traceId = Guid.NewGuid();
            var spanId = Guid.NewGuid();

            //var start = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);
            //Thread.Sleep(100);
            //var end = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);

            //this.wavefrontSender.SendSpan(
            //    "Search", start, end, "tap-dotnet-weather-web", traceId, Guid.NewGuid(),
            //    ImmutableList.Create(new Guid("82dd7b10-3d65-4a03-9226-24ff106b5041")), null,
            //    ImmutableList.Create(
            //        new KeyValuePair<string, string>("application", "tap-dotnet-weather-web"),
            //        new KeyValuePair<string, string>("service", "Search"),
            //        new KeyValuePair<string, string>("zipcode", model.WeatherInfo.ZipCode),
            //        new KeyValuePair<string, string>("http.method", "GET")), null);

            var homeViewModel = this.weatherApplication.GetForecast(model.WeatherInfo.ZipCode, traceId, spanId);

            homeViewModel.Recents = Cache(model.WeatherInfo.ZipCode);

            return View("Index", homeViewModel);
        }

        [HttpPost]
        public ActionResult Randomize(HomeViewModel model)
        {
            var traceId = Guid.NewGuid();
            var spanId = Guid.NewGuid();

            //var start = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);
            //Thread.Sleep(100);
            //var end = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);

            //this.wavefrontSender.SendSpan(
            //    "Randomize", start, end, "tap-dotnet-weather-web", traceId, Guid.NewGuid(),
            //    ImmutableList.Create(new Guid("82dd7b10-3d65-4a03-9226-24ff106b5041")), null,
            //    ImmutableList.Create(
            //        new KeyValuePair<string, string>("application", "tap-dotnet-weather-web"),
            //        new KeyValuePair<string, string>("service", "Randomize"),
            //        new KeyValuePair<string, string>("zipcode", "random"),
            //        new KeyValuePair<string, string>("http.method", "GET")), null);

            var homeViewModel = this.weatherApplication.GetRandom(traceId, spanId);

            return View("Index", homeViewModel);
        }

        [HttpPost]
        public ActionResult Save(HomeViewModel model)
        {
            var traceId = Guid.NewGuid();
            var spanId = Guid.NewGuid();

            //var start = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);
            //Thread.Sleep(100);
            //var end = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);

            //this.wavefrontSender.SendSpan(
            //    "Save", start, end, "tap-dotnet-weather-web", traceId, Guid.NewGuid(),
            //    ImmutableList.Create(new Guid("82dd7b10-3d65-4a03-9226-24ff106b5041")), null,
            //    ImmutableList.Create(
            //        new KeyValuePair<string, string>("application", "tap-dotnet-weather-web"),
            //        new KeyValuePair<string, string>("service", "Save"),
            //        new KeyValuePair<string, string>("zipcode", model.WeatherInfo.ZipCode),
            //        new KeyValuePair<string, string>("http.method", "POST")), null);

            var homeViewModel = this.weatherApplication.SaveFavorite(model.WeatherInfo.ZipCode, traceId, spanId);

            homeViewModel.Recents = Cache(model.WeatherInfo.ZipCode);

            return View("Index", homeViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IList<RecentViewModel> Cache(string zipCode)
        {
            var recents = new List<RecentViewModel>();

            await client.SaveStateAsync("statestore-web", zipCode, zipCode);

            // var result = await client.GetStateAsync<string>(DAPR_STORE_NAME, orderId.ToString());

            // await client.DeleteStateAsync(DAPR_STORE_NAME, orderId.ToString());

            //this.cache.KeyDelete(new RedisKey("Recent"));

            //var recent = this.cache.StringGet(new RedisKey("Recent"));

            //if (!recent.HasValue)
            //{
            //    var zipList = new List<string>() { zipCode };

            //    var recentList = JsonConvert.SerializeObject(zipList);

            //    this.cache.StringSet(new RedisKey("Recent"), new RedisValue(recentList));

            //    recents.Add(new RecentViewModel { ZipCode = zipCode });
            //}
            //else
            //{
            //    var zipList = JsonConvert.DeserializeObject<IList<string>>(recent);

            //    if (!zipList.Any(z => z == zipCode))
            //    {
            //        zipList.Add(zipCode);
            //    }

            //    var recentList = JsonConvert.SerializeObject(zipList);

            //    this.cache.StringSet(new RedisKey("Recent"), new RedisValue(recentList));

            //    foreach(var zip in zipList)
            //    {
            //        recents.Add(new RecentViewModel() { ZipCode = zip });
            //    }
            //}

            return recents;
        }
    }
}
