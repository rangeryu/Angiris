using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Angiris.Core.Models;
using Angiris.Core.Messaging;
using Angiris.Core.DataStore;

namespace Angiris.CentralAdmin.Core
{
    public class ScheduledFlightCrawlRequestFactory
    {
        IQueueTopicManager<FlightCrawlEntity> queueManager;
        INoSQLStoreProvider<FlightCrawlEntity> cacheStore;
        INoSQLStoreProvider<FlightCrawlEntity> persistenceStore;
 
        public async Task StartPushTaskMessages()
        {
            queueManager = QueueManagerFactory.CreateFlightCrawlEntityQueueMgr();
            queueManager.Initialize();

            cacheStore = DataProviderFactory.GetRedisQueuedTaskStore<FlightCrawlEntity>();
            cacheStore.Initialize();

            persistenceStore = DataProviderFactory.GetDocDBQueuedTaskStore<FlightCrawlEntity>();
            persistenceStore.Initialize();


            var crawlRequests = FakeDataRepo.GenerateRandomFlightCrawlRequests(1000);

            int i = 0;

            Parallel.ForEach(crawlRequests, async(r) =>  {
            //crawlRequests.ForEach(async(r) =>  {

                var currentTime = DateTime.UtcNow;
                r.CreateTime = currentTime;
                r.LastModifiedTime = currentTime;
                r.Status = Angiris.Core.Models.TaskStatus.New;
                r.TaskID = Guid.NewGuid();
               

                await SaveTaskResult(r);
                var sendResult = await queueManager.SendMessage(r);
                if(sendResult)
                {
                    r.Status = Angiris.Core.Models.TaskStatus.Queueing;
                    await UpdateTaskResult(r);
                }

                Console.WriteLine(i++);
                 
            });

        }
 

        private async Task SaveTaskResult(FlightCrawlEntity crawlTask)
        {
            await cacheStore.CreateEntity(crawlTask);
            await persistenceStore.CreateEntity(crawlTask); // can't we wait for its end?

            return;
        }

        private async Task UpdateTaskResult(FlightCrawlEntity crawlTask)
        {
            await cacheStore.UpdateEntity(crawlTask.TaskID.ToString(), crawlTask);
            await persistenceStore.UpdateEntity(crawlTask.TaskID.ToString(), crawlTask); // can't we wait for its end?

            return;
        }
         

    }
}
