using Microsoft.AspNetCore.Mvc;
using System.Collections;
using WeatherWatch.Web.Models;
using WeatherWatch.Application.Interfaces;

namespace WeatherWatch.Web.Controllers
{
    public class EnvironmentController : Controller
    {
        private readonly IWeatherWatchApplication weatherApplication;
        private readonly ILogger<HomeController> _logger;

        public EnvironmentController(
            IWeatherWatchApplication weatherApplication,
            ILogger<HomeController> logger)
        {
            this.weatherApplication = weatherApplication;
            _logger = logger;
        }

        public IActionResult Index()
        {
            // list environment variables
            try
            {
                var variables = new List<EnvironmentVariable>();

                try
                {
                    var environment = Environment.GetEnvironmentVariables();
                    foreach (DictionaryEntry variable in environment)
                    {
                        var ev = new EnvironmentVariable()
                        {
                            Key = variable.Key.ToString() ?? String.Empty,
                            Value = variable.Value?.ToString() ?? String.Empty
                        };

                        variables.Add(ev);
                    }
                }
                catch
                {
                    // send errors somewhere
                }

                ViewBag.Variables = variables.OrderBy(e => e.Key).ToList();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Environment", ex.StackTrace ?? ex.Message);
            }

            return View();
        }
    }
}
