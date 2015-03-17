using Angiris.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.Backend.Crawlers
{
    public class FlightCrawlerFactory
    {
        public static FlightCrawlerBase Create(FlightCrawlEntity crawlEntity)
        {
            FlightCrawlerBase crawler = null;

            switch(crawlEntity.RequestData.Company.ToUpper())
            {
                case "AIRASIA": crawler = new AirAsiaFlightCrawler(); break;
                case "SPRING": crawler = new SpringFlightCrawler(); break; 
                default: break;
            }

            return crawler;
        }

    }
}
