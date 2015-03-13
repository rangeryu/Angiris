namespace Angiris.Core.Messaging
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
    using System.Threading.Tasks;

	public interface IQueueTopicManager
	{
		string TopicName
		{
			get;
			set;
		}

        string ConnectionString
		{
			get;
			set;
		}

        async Task SendMessages();

        void Initialize();

        async Task StartReceiveMessages(Action onMsg);

        public virtual void Stop();

	}
}

