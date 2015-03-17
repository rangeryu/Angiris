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
        IQueueTopicManager<FlightCrawlEntity> queueManagerP0;
        IQueueTopicManager<FlightCrawlEntity> queueManager;
        INoSQLStoreProvider<FlightCrawlEntity> cacheStore;
        INoSQLStoreProvider<FlightCrawlEntity> persistenceStore;
 
        public async Task StartPushTaskMessages()
        {
            queueManager = QueueManagerFactory.CreateFlightCrawlEntityQueueMgr();
            queueManager.Initialize();

            queueManagerP0 = QueueManagerFactory.CreateFlightCrawlEntityQueueMgr(true);
            queueManagerP0.Initialize();

            cacheStore = DataProviderFactory.GetRedisQueuedTaskStore<FlightCrawlEntity>();
            cacheStore.Initialize();

            persistenceStore = DataProviderFactory.GetDocDBQueuedTaskStore<FlightCrawlEntity>();
            persistenceStore.Initialize();
    
            var crawlRequests = FakeDataRepo.GenerateRandomFlightCrawlRequests(500);
            var crawlRequestsP0 = FakeDataRepo.GenerateRandomFlightCrawlRequests(100);
           //int i = 0;


            //ParallelOptions pOption = new ParallelOptions();
            //pOption.MaxDegreeOfParallelism = 20;

            var startTime = DateTime.Now;

            var result = Parallel.ForEach(crawlRequests.Concat(crawlRequestsP0), (r) =>  {
            //crawlRequests.ForEach((r) =>  {

                Task.Run(async () =>
                {
                    //Console.WriteLine("processing " + i.ToString());
                    var currentTime = DateTime.UtcNow;
                    r.CreateTime = currentTime;
                    r.LastModifiedTime = currentTime;
                    r.Status = Angiris.Core.Models.TaskStatus.New;
                    r.TaskID = Guid.NewGuid();


                    await cacheStore.CreateEntity(r);
                    bool sendResult;
                    if(crawlRequestsP0.Contains(r))
                    {
                        sendResult = await queueManagerP0.SendMessage(r);
                    }
                    else
                    {
                        sendResult = await queueManager.SendMessage(r);
                    }

                    if (sendResult)
                    {
                        r.Status = Angiris.Core.Models.TaskStatus.Queueing;
                        r.LastModifiedTime = DateTime.UtcNow;
                        await cacheStore.UpdateEntity(r.TaskID.ToString(), r);
                    }
                    await persistenceStore.CreateEntity(r);
                    Console.WriteLine("done " + crawlRequests.IndexOf(r));
                    
                    //Console.WriteLine(i++);
                }).Wait();
            });

            var endTime = DateTime.Now;
            Console.WriteLine("End in " + (endTime - startTime).TotalSeconds + " seconds");
        }
 

    }
}
