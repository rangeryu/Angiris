using Angiris.APIService.Core;
using Angiris.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Angiris.APIService.WebApp.WebApiControllers
{
    [RoutePrefix("api/flights")]
    public class FlightApiController : ApiController
    {
        readonly FlightQueryService _flightQuerySvc = new FlightQueryService();
        
        /// <summary>
        /// Query for flight responses 
        /// https://angirisdemo.azure-api.net/flights/query?subscription-key=73caed41790745e39d7fce419714980c
        /// </summary>
        /// <param name="query">the query syntax containing the list of flight request</param>
        /// <returns>List of FlightResponse</returns>
        [Route("query")]
        [HttpPost]
        public IEnumerable<FlightResponse> Query([FromBody]FlightQuerySyntax query)
        {
            return _flightQuerySvc.QueryEntities(query.FlightRequests);
        }

        /// <summary>
        /// Query for flight responses 
        /// https://angirisdemo.azure-api.net/flights/query[?departure][&arrival][&date][&company]&subscription-key=73caed41790745e39d7fce419714980c
        /// </summary>
        /// <param name="departure">Departure City</param>
        /// <param name="arrival">Arrival City</param>
        /// <param name="date">Flight Date. In yyyyMMdd or yyyy-MM-dd format.</param>
        /// <param name="company">optional. specify the airline company </param>
        /// <returns>List of FlightResponse</returns>
        [Route("query")]
        [HttpGet]
        public IEnumerable<FlightResponse> Query(string departure, string arrival, string date, string company = "")
        {
            DateTime flightDate;

            if (string.IsNullOrEmpty(departure) || string.IsNullOrEmpty(arrival) 
                || 
                !(DateTime.TryParse(date, out flightDate) 
                || DateTime.TryParseExact(date, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out flightDate))
                )
                throw new HttpResponseException(HttpStatusCode.Forbidden );

            FlightRequest req = new FlightRequest() { DepartureCity = departure, ArrivalCity = arrival, FlightDate = flightDate, Company = company };

            return _flightQuerySvc.QueryEntities(req);
        }
        
//http://localhost:58178/api/flights/query?departure=bbb&arrival=ddd&date=2015-04-03

//POST http://localhost:58178/api/flights/query HTTP/1.1
//User-Agent: Fiddler
//Host: localhost:58178
//Content-Length: 345
//Content-type: application/json

//{
//  "flightRequests": [

//    {
//      "DepartureCity": "bbb",
//      "ArrivalCity": "ddd",
//      "FlightDate": {
//        "Date": "2015-04-03T00:00:00.000Z"
//         }
//    }
//  ,
//    {
//      "DepartureCity": "bbb",
//      "ArrivalCity": "ddd",
//      "FlightDate": {
//        "Date": "2015-04-03T00:00:00.000Z"
//         }
//    }
//  ]
//}
    }

    public class FlightQuerySyntax
    {
        public FlightRequest[] FlightRequests { get; set; }
    }
}
