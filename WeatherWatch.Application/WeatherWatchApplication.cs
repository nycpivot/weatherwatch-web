using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Net;
using WeatherWatch.Application.Interfaces;
using WeatherWatch.Application.Models;

namespace WeatherWatch.Application
{
    public class WeatherWatchApplication : IWeatherWatchApplication
    {
        private readonly IWeatherApi weatherApi;

        public WeatherWatchApplication(IWeatherApi weatherApi)
        {
            this.weatherApi = weatherApi;
        }

        public HomeViewModel GetForecast(string zipCode, Guid traceId, Guid spanId)
        {
            //var start = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);
            //Thread.Sleep(100);
            //var end = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);

            //this.wavefrontSender.SendSpan(
            //    "GetForecast", start, end, "WeatherApplication", traceId, Guid.NewGuid(),
            //    ImmutableList.Create(new Guid("82dd7b10-3d65-4a03-9226-24ff106b5041")), null,
            //    ImmutableList.Create(
            //        new KeyValuePair<string, string>("application", "tap-dotnet-weather-web"),
            //        new KeyValuePair<string, string>("service", "GetForecast"),
            //        new KeyValuePair<string, string>("zipcode", zipCode),
            //        new KeyValuePair<string, string>("http.method", "GET")), null);

            var homeViewModel = GetHomeView(zipCode, traceId, spanId);

            return homeViewModel;
        }

        public HomeViewModel GetRandom(Guid traceId, Guid spanId)
        {
            var homeViewModel = new HomeViewModel();

            //var start = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);
            //Thread.Sleep(100);
            //var end = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);

            //this.wavefrontSender.SendSpan(
            //    "GetRandom", start, end, "WeatherApplication", traceId, Guid.NewGuid(),
            //    ImmutableList.Create(new Guid("82dd7b10-3d65-4a03-9226-24ff106b5041")), null,
            //    ImmutableList.Create(
            //        new KeyValuePair<string, string>("application", "tap-dotnet-weather-web"),
            //        new KeyValuePair<string, string>("service", "GetRandom"),
            //        new KeyValuePair<string, string>("zipcode", "random"),
            //        new KeyValuePair<string, string>("http.method", "GET")), null);

            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };

                using (var httpClient = new HttpClient(handler))
                {
                    httpClient.BaseAddress = new Uri(this.weatherApi.Url);
                    httpClient.DefaultRequestHeaders.Add("X-TraceId", traceId.ToString());
                    httpClient.DefaultRequestHeaders.Add("X-SpanId", spanId.ToString());

                    // get saved favorite zip codes
                    var favoritesResponse = httpClient.GetAsync("favorites").Result;
                    if (favoritesResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var content = favoritesResponse.Content.ReadAsStringAsync().Result;
                        homeViewModel.Favorites = JsonConvert.DeserializeObject<IList<FavoriteViewModel>>(content) 
                            ?? new List<FavoriteViewModel>();
                    }

                    // get weather forecast for incoming zip code
                    var forecastResponse = httpClient.GetAsync($"forecast/random").Result;
                    if (forecastResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var content = forecastResponse.Content.ReadAsStringAsync().Result;

                        homeViewModel.WeatherInfo = JsonConvert.DeserializeObject<WeatherInfoViewModel>(content) 
                            ?? new WeatherInfoViewModel();
                    }
                }
            }

            return homeViewModel;
        }

        public HomeViewModel SaveFavorite(string zipCode, Guid traceId, Guid spanId)
        {
            var homeViewModel = new HomeViewModel();

            //var start = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);
            //Thread.Sleep(100);
            //var end = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);

            //this.wavefrontSender.SendSpan(
            //    "Post", start, end, "WeatherApplication", traceId, Guid.NewGuid(),
            //    ImmutableList.Create(new Guid("82dd7b10-3d65-4a03-9226-24ff106b5041")), null,
            //    ImmutableList.Create(
            //        new KeyValuePair<string, string>("application", "tap-dotnet-weather-web"),
            //        new KeyValuePair<string, string>("service", "SaveFavorite"),
            //        new KeyValuePair<string, string>("zipcode", zipCode),
            //        new KeyValuePair<string, string>("http.method", "POST")), null);

            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };

                using (var httpClient = new HttpClient(handler))
                {
                    httpClient.BaseAddress = new Uri(this.weatherApi.Url);
                    httpClient.DefaultRequestHeaders.Add("X-TraceId", traceId.ToString());
                    httpClient.DefaultRequestHeaders.Add("X-SpanId", spanId.ToString());

                    var result = httpClient.GetAsync($"favorites/{zipCode}").Result;
                }
            }

            homeViewModel = GetHomeView(zipCode, traceId, spanId);

            return homeViewModel;
        }

        private HomeViewModel GetHomeView(string zipCode, Guid traceId, Guid spanId)
        {
            var homeViewModel = new HomeViewModel();

            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };

                using (var httpClient = new HttpClient(handler))
                {
                    httpClient.BaseAddress = new Uri(this.weatherApi.Url);
                    httpClient.DefaultRequestHeaders.Add("X-TraceId", traceId.ToString());
                    httpClient.DefaultRequestHeaders.Add("X-SpanId", spanId.ToString());

                    // get saved favorite zip codes
                    var favoritesResponse = httpClient.GetAsync("favorites").Result;
                    if (favoritesResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var content = favoritesResponse.Content.ReadAsStringAsync().Result;
                        homeViewModel.Favorites = JsonConvert.DeserializeObject<IList<FavoriteViewModel>>(content) 
                            ?? new List<FavoriteViewModel>();
                    }

                    // get weather forecast for incoming zip code
                    var forecastResponse = httpClient.GetAsync($"forecast/{zipCode}").Result;
                    if (forecastResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var content = forecastResponse.Content.ReadAsStringAsync().Result;

                        homeViewModel.WeatherInfo = JsonConvert.DeserializeObject<WeatherInfoViewModel>(content) 
                            ?? new WeatherInfoViewModel();
                        homeViewModel.WeatherInfo.ZipCode = zipCode;
                    }
                }
            }

            return homeViewModel;
        }
    }
}