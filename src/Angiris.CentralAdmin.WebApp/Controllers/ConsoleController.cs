using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Angiris.Core.Models;
using Newtonsoft.Json;

namespace Angiris.CentralAdmin.WebApp.Controllers
{
    public class ConsoleController : Controller
    {
        // GET: Console
        public ActionResult Index()
        {
            return View("FlightQuery");
        }

        public ActionResult FlightQuery()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> _FlightQuery(string departure, string arrival, string date, string company = "")
        {
            // aaa-hhh-20150429

            HttpClient httpClient = new HttpClient();

            var apitoken = "73caed41790745e39d7fce419714980c";

            string requestUrl =
                string.Format(
                    "https://angirisdemo.azure-api.net/flights/query?departure={0}&arrival={1}&date={2}&company={3}&subscription-key={4}",
                    departure, arrival, date, company, apitoken
                    );

            string response = await httpClient.GetStringAsync(requestUrl);

            var responseData = JsonConvert.DeserializeObject<IEnumerable<FlightResponse>>(response);

            return PartialView("_FlightQueryResult",responseData);
        }
    }
}