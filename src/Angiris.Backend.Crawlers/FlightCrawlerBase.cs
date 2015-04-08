using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Angiris.Core.Models;
using System.Net.Http;

namespace Angiris.Backend.Crawlers
{
    public abstract class FlightCrawlerBase
    {
        public FlightCrawlEntity CrawlEntity { get; protected set; }

        public FlightCrawlerBase()
        {

        }
        public FlightCrawlerBase(FlightCrawlEntity crawlEntity)
        {
            Initialize(crawlEntity);
        }

        public virtual void Initialize(FlightCrawlEntity crawlEntity)
        {
            CrawlEntity = crawlEntity;
        }

        public virtual async Task StartProcessing()
        {
            //fake http call
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string[] fakeResource = { "http://www.microsoft.com/privacystatement/en-us/bingandmsn/default.aspx"
                                                , "http://www.bing.com/search?q=azure%20service%20bus"
                                                , "http://azure.microsoft.com/blog/"
                                                , "https://msdn.microsoft.com/en-US/"
                                                , "http://www.kjt.com/" };

                    string[] cabinNames = { "First Class", "Business Class", "Business Class", "Economy Class", "Economy Class", "Economy Class", "Economy Class" };

                    Random rnd = new Random(DateTime.UtcNow.Millisecond);
                    var resource = fakeResource[rnd.Next(0, fakeResource.Length)] + "?rnd=" + rnd.Next().ToString();

                    var result = await httpClient.GetAsync(resource);

                    if (result.IsSuccessStatusCode)
                    {


                        var flightReq = this.CrawlEntity.RequestData;

                        for (int r = 0; r < rnd.Next(1, 4); r++)
                        {
                            var flightResp = new FlightResponse()
                            {
                                ArrivalCity = flightReq.ArrivalCity,
                                Company = flightReq.Company,
                                DepartureCity = flightReq.DepartureCity,
                                FlightDate = flightReq.FlightDate,
                                FlightNumber = rnd.Next(1000, 1050).ToString(),
                                ArrivalTime = flightReq.FlightDate.Date.AddHours(rnd.Next(1, 20)),
                                DepartureTime = flightReq.FlightDate.Date.AddHours(rnd.Next(1, 10))
                            };

                            var cabins = new List<FlightCabin>();
                            for (int c = 0; c < rnd.Next(0, 5); c++)
                            {
                                var flightCabin = new FlightCabin()
                                {
                                    AirPortFee = rnd.Next(100, 1000),
                                    CabinPrice = rnd.Next(100, 1000),
                                    CurrencyType = "USD",
                                    FuelFee = rnd.Next(100, 1000),
                                    InsurancePrice = rnd.Next(100, 1000),
                                    OtherFee = rnd.Next(100, 1000),
                                    CabinName = cabinNames[rnd.Next(0, cabinNames.Length)]
                                };
                                cabins.Add(flightCabin);
                            }
                            flightResp.FlightCabins = cabins;

                            this.CrawlEntity.ResponseData.Add(flightResp);

                        }

                        
                        this.CrawlEntity.FinishTime = DateTime.UtcNow;


                    }
                    else
                    {
                        //this.CrawlEntity.Status = Core.Models.TaskStatus.Failed;
                        this.CrawlEntity.LogData.Add(DateTime.UtcNow.ToString() + ", " + result.StatusCode.ToString());
                    }

                    this.CrawlEntity.Status = Core.Models.TaskStatus.Completed;
                }

            }
            catch(Exception ex)
            {
                this.CrawlEntity.Status = Core.Models.TaskStatus.Failed;
                this.CrawlEntity.LogData.Add(DateTime.UtcNow.ToString() + ", " + ex.Message + ", " + ex.InnerException + ", " + ex.StackTrace);

            }

            this.CrawlEntity.LastModifiedTime = DateTime.UtcNow;
        }
    }
}
