using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Angiris.Core.Models;
using Newtonsoft.Json;
using Angiris.APIService.Core;

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

        /// <summary>
        /// Query via client redis api
        /// </summary>
        /// <param name="departure"></param>
        /// <param name="arrival"></param>
        /// <returns></returns>
        public async Task<ActionResult> ListDates(string departure, string arrival)
        {
            FlightQueryService querySvc = new FlightQueryService();
            var keys = await querySvc.QueryKeys(departure, arrival, null);

            var dates = keys.Select(k => k.Split('-').LastOrDefault()).OrderBy(t=>t).ToList();

            return Json(dates,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Query via exposed API management
        /// </summary>
        /// <param name="departure"></param>
        /// <param name="arrival"></param>
        /// <param name="date"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> _FlightQuery(string departure, string arrival, string date, string company = "")
        {
            // aaa-hhh-20150429
            //HttpClient httpClient = new HttpClient();
            //var apitoken = "73caed41790745e39d7fce419714980c";
            //string requestUrl =
            //    string.Format(
            //        "https://angirisdemo.azure-api.net/flights/query?departure={0}&arrival={1}&date={2}&company={3}&subscription-key={4}",
            //        departure, arrival, date, company, apitoken
            //        );
            //string response = await httpClient.GetStringAsync(requestUrl);
            //var responseData = JsonConvert.DeserializeObject<IEnumerable<FlightResponse>>(response);

            FlightQueryService querySvc = new FlightQueryService();

            DateTime flightDate;

            if (string.IsNullOrEmpty(departure) || string.IsNullOrEmpty(arrival)
                ||
                !(DateTime.TryParse(date, out flightDate) || DateTime.TryParseExact(date, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out flightDate))
                )
                throw new HttpResponseException(HttpStatusCode.Forbidden);

            FlightRequest req = new FlightRequest() { DepartureCity = departure, ArrivalCity = arrival, FlightDate = flightDate, Company = company };

            var responseData =  await querySvc.QueryEntities(req);

            return PartialView("_FlightQueryResult",responseData);
        }

        
    }
}