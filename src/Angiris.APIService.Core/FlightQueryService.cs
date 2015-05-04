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

        public async Task<IEnumerable<string>> QueryKeys(string departure, string arrival, DateTime? date)
        {
            if(string.IsNullOrEmpty(departure))
                departure = "*";
            if(string.IsNullOrEmpty(arrival))
                arrival = "*";
        
            string dateStr = "*";
            if(date.HasValue)
                dateStr = date.Value.ToString("yyyyMMdd");

            var keys =
                (await entityDatabase.GetKeys(new Tuple<string, string, string>(departure, arrival, dateStr)));//.ToList();

            return keys;
        }

        public async Task<IEnumerable<FlightResponse>> QueryEntities(params FlightRequest[] flightRequests)
        {
            var requests = flightRequests.Where(r => !string.IsNullOrEmpty(r.ArrivalCity) && !string.IsNullOrEmpty(r.DepartureCity) && r.FlightDate > DateTime.UtcNow.Date.AddMonths(-1))
                .Distinct().ToList();

            var outputResults = new List<FlightResponse>();

            var queryTasks = requests.Select(r => entityDatabase.QueryEntities(r));

            var taskResults = await Task.WhenAll(queryTasks);

            taskResults.ToList().ForEach(r => outputResults.AddRange(r));
 
            return outputResults;
        }

    }
}
