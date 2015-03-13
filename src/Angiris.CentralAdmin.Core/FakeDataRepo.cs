using Angiris.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.CentralAdmin.Core
{
    public class FakeDataRepo
    {
        public static List<FlightCrawlEntity> GenerateRandomFlightCrawlRequests(int count)
        {
            //fake data

            List<FlightCrawlEntity> output = new List<FlightCrawlEntity>();

            for (int i = 0; i < count; i++)
            {
                var crawlRequest = new FlightCrawlEntity()
                {
                    CreateTime = DateTime.UtcNow,
                    Status = Angiris.Core.Models.TaskStatus.New,
                    RequestData = GenerateRandomFlightRequest(),
                    TaskID = Guid.NewGuid()
                };
                crawlRequest.LastModifiedTime = crawlRequest.CreateTime;

                output.Add(crawlRequest);
            }

            return output;
        }

        public static FlightRequest GenerateRandomFlightRequest()
        {
            var output = new FlightRequest();

            Random rnd = new Random(DateTime.Now.Millisecond);

            output.FlightDate = DateTime.UtcNow.AddDays(rnd.Next(0, 30));

            var cityList = FakeFlightDataSource.CityNameList;
            var companyList = FakeFlightDataSource.CompanyNameList;

            output.DeptureCity = cityList[rnd.Next(0, cityList.Count - 1)];
            output.ArrivalCity = cityList[rnd.Next(0, cityList.Count - 1)];
            output.Company = companyList[rnd.Next(0, companyList.Count - 1)];

            return output;
        }
    }
}
