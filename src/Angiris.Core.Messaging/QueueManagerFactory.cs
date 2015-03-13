namespace Angiris.Core.Messaging
{
    using Angiris.Core.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

	public class QueueManagerFactory
	{
        public static IQueueTopicManager<FlightCrawlEntity> CreateFlightCrawlEntityQueueMgr()
        {
            //TODO extract configs from hardcode.

            string topicName = "entity-crawl";
            string connString = "Endpoint=sb://rangermsgq.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=gY3g9rQeje4FPBE2mKluzilipVAEy/JaV5hlaMVB2Zo=";
            
            return new ServiceBusQueueManager<FlightCrawlEntity>(topicName, connString);
        }
	}
}

