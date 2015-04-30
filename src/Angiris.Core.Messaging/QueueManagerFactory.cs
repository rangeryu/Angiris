namespace Angiris.Core.Messaging
{
    using Angiris.Core.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

	public class QueueManagerFactory
	{
        public static IQueueTopicManager<FlightCrawlEntity> CreateFlightCrawlEntityQueueMgr(QueueMgrProfile profile)
        {
         
            dynamic cfg = Config.ConfigMgr.ReleaseCfg.FlightCrawlEntityQueue;

            string topicName = profile.IsHighPriority ? cfg.P0Name : cfg.P1Name;
            string connString = cfg.SBConn;
 
            return new ServiceBusQueueManager<FlightCrawlEntity>(topicName, connString, profile.MaxConcurrentCalls, profile.ClientPrefetchCount);
        }
 
	}

}

