using Angiris.Core.DataStore;
using Angiris.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.APIService.Core
{
    public class FlightQueryService
    {
        RedisFlightEntityDatabase entityDatabase
        {
            get
            {
                return DataProviderFactory.SingletonFlightEntityDatabase;
            }
        }

        public IEnumerable<FlightResponse> QueryEntities(params FlightRequest[] flightRequests)
        {
            var requests = flightRequests.Where(r => !string.IsNullOrEmpty(r.ArrivalCity) && !string.IsNullOrEmpty(r.DepartureCity) && r.FlightDate > DateTime.UtcNow.Date.AddMonths(-1))
                .Distinct().ToList();

            var results = new List<FlightResponse>();
            Parallel.ForEach(requests, (r) =>
                {
                    Task.Run(async () =>
                    {
                        var result = await entityDatabase.QueryEntities(r);
                        results.AddRange(result);
                    }).Wait();
                }
            );

            results = results.ToList();
           
            return results;
        }

    }
}
