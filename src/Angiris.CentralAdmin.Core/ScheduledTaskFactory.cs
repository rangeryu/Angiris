using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Angiris.Core.Models;

namespace Angiris.CentralAdmin.Core
{
    public class ScheduledFlightCrawlRequestFactory
    {
        public async Task StartPushJob()
        {
            var crawlRequests = FakeDataRepo.GenerateRandomFlightCrawlRequests(1000);

            crawlRequests.ForEach(r =>  {

                SaveToTaskResultStore(r).Wait();

            
            });

        }

        private async Task PushToMessageQueue(FlightEntityCrawlRequest crawlTask)
        {
            var msgBody = crawlTask.RequestData;
            


            //why not bulk push...
            //send to q
            //TODO: retry, error handling
            return;
        }

        private async Task SaveToTaskResultStore(FlightEntityCrawlRequest crawlTask)
        {
            //async save to redis - EntityTaskResultStore
            //async save to docdb - EntityTaskResultStore
            var task = Task.Run(() => { });
            return;
        }


         

    }
}
