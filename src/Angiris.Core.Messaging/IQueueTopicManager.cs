namespace Angiris.Core.Messaging
{
    using Angiris.Core.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

	public interface IQueueTopicManager<TMsgBody> where TMsgBody: IQueuedTask
	{
		string TopicName
		{
			get;
		}

        string ConnectionString
		{
			get;
		}

        Task<bool> SendMessage(TMsgBody messages);

        Task<bool> SendMessages(IEnumerable<TMsgBody> messages);

        void Initialize();

        void StartReceiveMessages(Func<TMsgBody,Task> processMessageTask);

        Task Stop();

	}
}

