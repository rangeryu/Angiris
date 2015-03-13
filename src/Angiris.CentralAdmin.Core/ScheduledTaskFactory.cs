using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Angiris.Core.Models;
using Angiris.Core.Messaging;

namespace Angiris.CentralAdmin.Core
{
    public class ScheduledFlightCrawlRequestFactory
    {
        IQueueTopicManager<FlightCrawlEntity> queueManager;
 
        public async Task StartPushTaskMessages()
        {
            queueManager = QueueManagerFactory.CreateFlightCrawlEntityQueueMgr();
            queueManager.Initialize();


            var crawlRequests = FakeDataRepo.GenerateRandomFlightCrawlRequests(100);

            crawlRequests.ForEach(async(r) =>  {

                await SaveToTaskResultStore(r);
                var sendResult = await queueManager.SendMessage(r);
                if(!sendResult)
                {
                    //
                }
            
            });

        }
 

        private async Task SaveToTaskResultStore(FlightCrawlEntity crawlTask)
        {
            //async save to redis - EntityTaskResultStore
            //async save to docdb - EntityTaskResultStore
            var task = Task.Run(() => { });
            return;
        }


         

    }
}
