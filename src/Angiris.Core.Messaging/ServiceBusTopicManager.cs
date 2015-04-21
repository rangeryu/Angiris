namespace Angiris.Core.Messaging

{
using Angiris.Core.Models;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

    public class ServiceBusQueueManager<TMsgBody> : IQueueTopicManager<TMsgBody> where TMsgBody : IQueuedTask
	{
        public string TopicName
        {
            get { return this._queueName; }
            private set { this._queueName = value; }
        }

        public string ConnectionString
        {
            get;
            private set;
        }

        public int MaxConcurrentCalls
        {
            get;
            private set;
        }

        public int ClientPrefetchCount
        {
            get;
            private set;
        }


        private readonly ManualResetEvent _pauseProcessingEvent;
        private QueueClient _client;
        private readonly TimeSpan _waitTime = TimeSpan.FromSeconds(30);
        
        //We only use queue feature in SB. However other msg queue solution like NSQ only have "Topic".
        private string _queueName;

        public QueueDescription QueueInfo
        {
            get;
            private set;
        }

        public ServiceBusQueueManager(string topicName, string connectionString, int maxConcurrentCalls = 10, int clientPrefetchCount = 100)
        {
            this.TopicName = topicName;
            this.ConnectionString = connectionString;
            this.MaxConcurrentCalls = maxConcurrentCalls;
            this.ClientPrefetchCount = clientPrefetchCount;
            this._pauseProcessingEvent = new ManualResetEvent(true);
        }
        public void Initialize()
        {
            // Check queue existence.
            var manager = NamespaceManager.CreateFromConnectionString(this.ConnectionString);

            try
            {
                this.QueueInfo = manager.GetQueue(this._queueName);
            }
            catch (MessagingEntityNotFoundException)
            {
                try
                {
                    var queueDescription = new QueueDescription(this._queueName)
                    {
                        LockDuration = TimeSpan.FromMinutes(1),
                        EnablePartitioning = true,
                        EnableBatchedOperations = true,
                        EnableExpress = true,
                        MaxDeliveryCount = 3
                    };

                    //1 min lock duration is the default value
                    //https://msdn.microsoft.com/en-us/library/microsoft.servicebus.messaging.queuedescription.lockduration.aspx
                    //Partitioning Messaging Entities https://msdn.microsoft.com/en-us/library/azure/dn520246.aspx
                    // Set the maximum delivery count for messages. A message is automatically deadlettered after this number of deliveries.  Default value is 10.

                    this.QueueInfo = manager.CreateQueue(queueDescription);
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                    Trace.TraceWarning(
                        "MessagingEntityAlreadyExistsException Creating Queue - Queue likely already exists for path: {0}", this._queueName);
                }
                catch (MessagingException ex)
                {
                    var webException = ex.InnerException as WebException;
                    if (webException != null)
                    {
                        var response = webException.Response as HttpWebResponse;

                        // It's likely the conflicting operation being performed by the service bus is another queue create operation
                        // If we don't have a web response with status code 'Conflict' it's another exception
                        if (response == null || response.StatusCode != HttpStatusCode.Conflict)
                        {
                            throw;
                        }

                        Trace.TraceWarning("MessagingException HttpStatusCode.Conflict - Queue likely already exists or is being created or deleted for path: {0}", this._queueName);
                    }
                }
            }
            catch(Exception ex)
            {
                Trace.TraceWarning("ServiceBusQueueManager Initialize " + ex.Message);
            }
            
  
            // Create the queue client. By default, the PeekLock method is used.
            this._client = QueueClient.CreateFromConnectionString(this.ConnectionString, this._queueName);
           
            
        }

        public async Task<bool> SendMessage(TMsgBody msg)
        {
            var brokeredMsg = new BrokeredMessage(msg) {MessageId = msg.TaskId};

            try
            {
                await this._client.SendAsync(brokeredMsg);
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception in QueueClient.SendAsync: {0}", ex.Message);

                return false;
            }

        }

        public async Task<bool> SendMessages(IEnumerable<TMsgBody> messages)
        {
            //you should not see this code.

            var 傻逼消息列表 = messages.Select(msg => new BrokeredMessage(msg) {MessageId = msg.TaskId}).ToList();

            try
            {
                await _client.SendBatchAsync(傻逼消息列表);
                return true;
            }
            catch(Exception ex)
            {
                Trace.TraceError("Exception in QueueClient.SendBatch: {0}", ex.Message);

                return false;
            }
            
        }



        /// <summary>
        /// bind action to client.OnMessageAsync. so it must be called only once.
        /// </summary>
        /// <param name="processMessageTask"></param>
        public void StartReceiveMessages(Func<TMsgBody,Task> processMessageTask)
        {
            // Setup the options for the message pump.
            var options = new OnMessageOptions
            {
                AutoComplete = false,
                MaxConcurrentCalls = this.MaxConcurrentCalls
            };

            // When AutoComplete is disabled, you have to manually complete/abandon the messages and handle errors, if any.
            options.ExceptionReceived += this.OptionsOnExceptionReceived;

            this._client.PrefetchCount = this.ClientPrefetchCount;
            // Use of Service Bus OnMessage message pump. The OnMessage method must be called once, otherwise an exception will occur.
            this._client.OnMessageAsync(
                async (msg) =>
                {
                        // Will block the current thread if Stop is called.
                        this._pauseProcessingEvent.WaitOne();
                        try
                        {
                            var msgBody = msg.GetBody<TMsgBody>();

                            Trace.TraceInformation("Start to process task {0} @{1}", msgBody.TaskId, DateTime.Now);
                            // Execute processing task here
                            await processMessageTask(msgBody);

                            Trace.TraceInformation("done task {0} @{1}", msgBody.TaskId, DateTime.Now);
                            switch (msgBody.Status)
                            {
                                case Models.TaskStatus.Completed: await msg.CompleteAsync(); break;
                                case Models.TaskStatus.InDeadletter: await msg.DeadLetterAsync(); break;
                                default: await msg.AbandonAsync(); break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError("Exception in QueueClient.OnMessageAsync: {0}", ex.Message);
                            msg.Abandon();
                        }
                },
                options);
        }

        public async Task Stop()
        {
            // Pause the processing threads
            this._pauseProcessingEvent.Reset();

            // There is no clean approach to wait for the threads to complete processing.
            // We simply stop any new processing, wait for existing thread to complete, then close the message pump and then return
            Thread.Sleep(_waitTime);

            await this._client.CloseAsync();
        }

        private void OptionsOnExceptionReceived(object sender, ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            if (exceptionReceivedEventArgs != null && exceptionReceivedEventArgs.Exception != null)
            {
                var exceptionMessage = exceptionReceivedEventArgs.Exception.Message;
                Trace.TraceError("Exception in QueueClient.ExceptionReceived: {0}", exceptionMessage);
            }
        }


 
    }
}

