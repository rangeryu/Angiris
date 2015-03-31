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
            //TODO extract configs from hardcode.

            string topicName = profile.IsHighPriority ? "flight-crawl-p0" : "flight-crawl-p1";
            string connString = "Endpoint=sb://angiris-demo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=z/GrYTgcgt3Fm2Ys2cF74w2WNftSn0kc/zAHA+OUVK4=";
 
            return new ServiceBusQueueManager<FlightCrawlEntity>(topicName, connString, profile.MaxConcurrentCalls, profile.ClientPrefetchCount);
        }
 
	}

}

