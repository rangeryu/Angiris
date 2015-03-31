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
        FlightQueryService flightQuerySvc = new FlightQueryService();
        
        [Route("query")]
        [HttpPost]
        public IEnumerable<FlightResponse> Query([FromBody]FlightQuerySyntax query)
        {
            return flightQuerySvc.QueryEntities(query.flightRequests);
        }


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
        public FlightRequest[] flightRequests { get; set; }
    }
}
