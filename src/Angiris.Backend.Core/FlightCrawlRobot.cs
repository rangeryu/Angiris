using System.Globalization;

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

        private readonly INoSqlStoreProvider<FlightCrawlEntity> _taskCacheStore;

        private readonly RedisFlightEntityDatabase _flightEntityDatabase;

        private readonly DocDbFlightEntityDatabase _docDbFlightEntityDatabase;

        private readonly IQueueTopicManager<FlightCrawlEntity> _queueManager;

        public RobotStatus Status
        {
            get;
            set;
        }

        public FlightCrawlRobot(QueueMgrProfile profile)
        {
            _taskCacheStore = DataProviderFactory.SingletonRedisQueuedTaskStore;
            _flightEntityDatabase = DataProviderFactory.SingletonRedisFlightEntityDatabase;
            _docDbFlightEntityDatabase = DataProviderFactory.SingletonDocDbFlightEntityDatabase;
            _queueManager = QueueManagerFactory.CreateFlightCrawlEntityQueueMgr(profile);

            this.Status = new RobotStatus { Id = Guid.NewGuid() };

        }

        public void Initialize()
        {
            _queueManager.Initialize();
        }
        
        public void Start()
        {
  
            this.Status.StartTime = DateTime.UtcNow;
            this.Status.Name = "FlightCrawlRobot | " + this._queueManager.TopicName;
            _queueManager.StartReceiveMessages(async (entity) =>
            {
                await this.ProcessTask(entity);
            });
        }

        public async Task Stop()
        {
           await _queueManager.Stop();
        }
 

        int _totalReceivedCount = 0;
        int _concurrentJobCount = 0;

        protected async Task ProcessTask(FlightCrawlEntity crawlEntity)
        {
            try
            {
                

                Interlocked.Increment(ref _totalReceivedCount);
                Interlocked.Increment(ref _concurrentJobCount);

                crawlEntity.Status = Angiris.Core.Models.TaskStatus.Processing;
                crawlEntity.LastModifiedTime = DateTime.UtcNow;

                await _taskCacheStore.UpdateEntity(crawlEntity.TaskId, crawlEntity);



                if (crawlEntity.MaxExecutionTimeInMs == 0)
                {
                    crawlEntity.MaxExecutionTimeInMs = 50 * 1000;//50 seconds
                }

                var task = ExecuteTask(crawlEntity);

                if (await Task.WhenAny(task, Task.Delay(crawlEntity.MaxExecutionTimeInMs)) == task)
                {
                    // task completed within timeout

                    crawlEntity.FinishTime = DateTime.UtcNow;
                    

                    Parallel.ForEach(crawlEntity.ResponseData, async (r) =>
                    {
                        try
                        {
                            r.TimeStamp = DateTime.UtcNow;
                            await _flightEntityDatabase.CreateOrUpdateEntity(r);
                            await _docDbFlightEntityDatabase.CreateOrUpdateEntity(r);
                        }
                        catch(Exception ex)
                        {
                            string errorMsg = string.Format("Exception on CreateOrUpdateEntity, {0},{1},{2}", r.Id, ex.Message, ex.InnerException.Message);
                            Trace.TraceWarning(errorMsg);
                            crawlEntity.LogData.Add(errorMsg);
                        }
                    });
               
                }
                else
                {
                    // timeout logic
                    crawlEntity.LastModifiedTime = DateTime.UtcNow;
                    crawlEntity.Status = Angiris.Core.Models.TaskStatus.TimedOut;
                }

                Interlocked.Decrement(ref _concurrentJobCount);
                this.Status.TaskReceivedCount = _totalReceivedCount;
                this.Status.ConcurrentJobCount = _concurrentJobCount;

                await _taskCacheStore.UpdateEntity(crawlEntity.TaskId, crawlEntity);
                //await PersistenceStore.UpdateEntity(crawlEntity.TaskID, crawlEntity);

                Trace.TraceInformation("done by Robot {0} @{1}",crawlEntity.TaskId,DateTime.Now);
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
                    crawlEntity.LogData.Add(DateTime.UtcNow.ToString(CultureInfo.CurrentCulture) + ", " + ex.Message + ", " + ex.InnerException + ", " + ex.StackTrace);
                    
                }

            }
            else
            {
                //"No crawler applicable" should only occur when the farm is being updated
                //this status will make the queue manager do msg.AbandonAsync()
                //before it reaches the MaxDeliveryCount it will be put into the queue again and again
                //so good luck to the request for the next time when it is put into a instance having been updated.
                

                crawlEntity.Status = Angiris.Core.Models.TaskStatus.Failed;
                
                crawlEntity.LogData.Add(DateTime.UtcNow.ToString(CultureInfo.CurrentCulture) + ", No crawler applicable");
                
            }


           
        }
    }
}

