namespace Angiris.Core.Messaging
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class QueueManagerFactory
	{
        public static QueueTopicManager Create()
        {
            return new ServiceBusTopicManager();
        }
	}
}

