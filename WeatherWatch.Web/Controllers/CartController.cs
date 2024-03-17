using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace WeatherWatch.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly DaprClient daprClient;

        public CartController(
            DaprClient daprClient)
        {
            this.daprClient = daprClient;
        }

        public IActionResult Index()
        {
            zipCode = Guid.NewGuid().ToString();

            //var result = daprClient.GetStateAsync<string>("statestore-web", cartId.ToString());

            daprClient.SaveStateAsync("statestore-web", zipCode, zipCode);

            return View();
        }
    }
}
