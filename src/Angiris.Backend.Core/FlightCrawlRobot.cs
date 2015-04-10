namespace Angiris.Backend.Core
{
    using Angiris.Backend.Crawlers;
    using Angiris.Core.DataStore;
    using Angiris.Core.Messaging;
    using Angiris.Core.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class FlightCrawlRobot 
    {
        IQueueTopicManager<FlightCrawlEntity> queueManager;

        private INoSQLStoreProvider<FlightCrawlEntity> TaskCacheStore
        {
            get
            {
                return DataProviderFactory.SingletonRedisQueuedTaskStore;
            }
        }
 
        //private INoSQLStoreProvider<FlightCrawlEntity> PersistenceStore
        //{
        //    get
        //    {
        //        return DataProviderFactory.SingletonDocDBQueuedTaskStore;
        //    }
        //}
         
        private RedisFlightEntityDatabase FlightEntityDatabase
        {
            get
            {
                return DataProviderFactory.SingletonFlightEntityDatabase;
            }
        }

        private DocDBFlightEntityDatabase DocDBFlightEntityDatabase
        {
            get
            {
                return DataProviderFactory.SingletonDocDBFlightEntityDatabase;
            }
        }

        public RobotStatus Status
        {
            get;
            set;
        }

        public FlightCrawlRobot(QueueMgrProfile profile)
        {
            queueManager = QueueManagerFactory.CreateFlightCrawlEntityQueueMgr(profile);
            this.Status = new RobotStatus();
            this.Status.Id = Guid.NewGuid();
            
        }

        public void Initialize()
        {
            queueManager.Initialize();
        }
        
        public void Start()
        {
            //cacheStore = DataProviderFactory.GetRedisQueuedTaskStore<FlightCrawlEntity>();
            //cacheStore.Initialize();

            //persistenceStore = DataProviderFactory.GetDocDBQueuedTaskStore<FlightCrawlEntity>();
            //persistenceStore.Initialize();

            //TODO: need to have a better implementation for singleton of the instance.


            this.Status.StartTime = DateTime.UtcNow;
            this.Status.Name = "FlightCrawlRobot | " + this.queueManager.TopicName;
            queueManager.StartReceiveMessages(async (entity) =>
            {
                await this.ProcessTask(entity);
            });
        }

        public async Task Stop()
        {
           await queueManager.Stop();
        }
 

        int totalReceivedCount = 0;
        int concurrentJobCount = 0;

        protected async Task ProcessTask(FlightCrawlEntity crawlEntity)
        {
            try
            {
                

                Interlocked.Increment(ref totalReceivedCount);
                Interlocked.Increment(ref concurrentJobCount);

                crawlEntity.Status = Angiris.Core.Models.TaskStatus.Processing;
                crawlEntity.LastModifiedTime = DateTime.UtcNow;

                await TaskCacheStore.UpdateEntity(crawlEntity.TaskID, crawlEntity);



                if (crawlEntity.MaxExecutionTimeInMS == 0)
                {
                    crawlEntity.MaxExecutionTimeInMS = 50 * 1000;//50 seconds
                }

                var task = ExecuteTask(crawlEntity);

                if (await Task.WhenAny(task, Task.Delay(crawlEntity.MaxExecutionTimeInMS)) == task)
                {
                    // task completed within timeout

                    crawlEntity.FinishTime = DateTime.UtcNow;

                    Parallel.ForEach(crawlEntity.ResponseData, async (r) =>
                    {
                        await FlightEntityDatabase.CreateOrUpdateEntity(r);
                        await DocDBFlightEntityDatabase.CreateOrUpdateEntity(r);
                    });
               
                }
                else
                {
                    // timeout logic
                    crawlEntity.LastModifiedTime = DateTime.UtcNow;
                    crawlEntity.Status = Angiris.Core.Models.TaskStatus.TimedOut;
                }

                Interlocked.Decrement(ref concurrentJobCount);
                this.Status.TaskReceivedCount = totalReceivedCount;
                this.Status.ConcurrentJobCount = concurrentJobCount;

                await TaskCacheStore.UpdateEntity(crawlEntity.TaskID, crawlEntity);
                //await PersistenceStore.UpdateEntity(crawlEntity.TaskID, crawlEntity);

                Trace.TraceInformation("done by Robot {0} @{1}",crawlEntity.TaskID,DateTime.Now);
            }
            catch(Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        protected async Task ExecuteTask(FlightCrawlEntity crawlEntity)
        {
            FlightCrawlerBase crawler = FlightCrawlerFactory.Create(crawlEntity);

            if(crawler != null)
            {
                try
                {
                    crawler.Initialize(crawlEntity);
                    await crawler.StartProcessing();


                    //should the status be determined by crawler? in case the stupid crawler forgets to mark the latest status
                    if (crawlEntity.Status == Angiris.Core.Models.TaskStatus.Processing)
                        crawlEntity.Status = Angiris.Core.Models.TaskStatus.Completed;
                }
                catch(Exception ex)
                {
                    crawlEntity.Status = Angiris.Core.Models.TaskStatus.Failed;
                    crawlEntity.LogData.Add(DateTime.UtcNow.ToString() + ", " + ex.Message + ", " + ex.InnerException + ", " + ex.StackTrace);
                    
                }

            }
            else
            {
                //"No crawler applicable" should only occur when the farm is being updated
                //this status will make the queue manager do msg.AbandonAsync()
                //before it reaches the MaxDeliveryCount it will be put into the queue again and again
                //so good luck to the request for the next time when it is put into a instance having been updated.
                

                crawlEntity.Status = Angiris.Core.Models.TaskStatus.Failed;
                
                crawlEntity.LogData.Add(DateTime.UtcNow.ToString() + ", No crawler applicable");
                
            }


           
        }
    }
}

