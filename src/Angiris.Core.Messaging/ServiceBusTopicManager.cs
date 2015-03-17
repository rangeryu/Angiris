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
            get { return this.queueName; }
            private set { this.queueName = value; }
        }

        public string ConnectionString
        {
            get;
            private set;
        }



        private ManualResetEvent pauseProcessingEvent;
        private QueueClient client;
        private TimeSpan waitTime = TimeSpan.FromSeconds(30);
        
        //We only use queue feature in SB. However other msg queue solution like NSQ only have "Topic".
        private string queueName;

        public QueueDescription QueueInfo
        {
            get;
            private set;
        }

        public ServiceBusQueueManager(string topicName, string connectionString)
        {
            this.TopicName = topicName;
            this.ConnectionString = connectionString;
            this.pauseProcessingEvent = new ManualResetEvent(true);
        }
        public void Initialize()
        {
            // Check queue existence.
            var manager = NamespaceManager.CreateFromConnectionString(this.ConnectionString);

            try
            {
                this.QueueInfo = manager.GetQueue(this.queueName);
            }
            catch (MessagingEntityNotFoundException)
            {
                try
                {
                    var queueDescription = new QueueDescription(this.queueName);
                    
                    //1 min lock duration is the default value
                    //https://msdn.microsoft.com/en-us/library/microsoft.servicebus.messaging.queuedescription.lockduration.aspx
                    queueDescription.LockDuration = TimeSpan.FromMinutes(1);
                    //Partitioning Messaging Entities https://msdn.microsoft.com/en-us/library/azure/dn520246.aspx
                    //queueDescription.EnablePartitioning = true;
                    
                    // Set the maximum delivery count for messages. A message is automatically deadlettered after this number of deliveries.  Default value is 10.
                    queueDescription.MaxDeliveryCount = 3;

                    this.QueueInfo = manager.CreateQueue(queueDescription);
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                    Trace.TraceWarning(
                        "MessagingEntityAlreadyExistsException Creating Queue - Queue likely already exists for path: {0}", this.queueName);
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

                        Trace.TraceWarning("MessagingException HttpStatusCode.Conflict - Queue likely already exists or is being created or deleted for path: {0}", this.queueName);
                    }
                }
            }
            catch(Exception)
            {

            }
            
  
            // Create the queue client. By default, the PeekLock method is used.
            this.client = QueueClient.CreateFromConnectionString(this.ConnectionString, this.queueName);

        }

        public async Task<bool> SendMessage(TMsgBody msg)
        {
            var brokeredMsg = new BrokeredMessage(msg);
            brokeredMsg.MessageId = msg.TaskID.ToString();

            try
            {
                await this.client.SendAsync(brokeredMsg);
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
            var sbMsgList = new List<BrokeredMessage>();

            foreach(var msg in messages)
            {
                var brokeredMsg = new BrokeredMessage(msg);
                brokeredMsg.MessageId = msg.TaskID.ToString();
                
                sbMsgList.Add(brokeredMsg);
            }

            try
            {
                await this.client.SendBatchAsync(sbMsgList);
                return true;
            }
            catch(Exception ex)
            {
                Trace.TraceError("Exception in QueueClient.SendBatch: {0}", ex.Message);

                return false;
            }
            
        }



        public void StartReceiveMessages(Action<TMsgBody> processMessageTask)
        {
            // Setup the options for the message pump.
            var options = new OnMessageOptions();

            // When AutoComplete is disabled, you have to manually complete/abandon the messages and handle errors, if any.
            options.AutoComplete = false;
            options.MaxConcurrentCalls = 10;
            options.ExceptionReceived += this.OptionsOnExceptionReceived;

            //this.client.PrefetchCount = 10;
            // Use of Service Bus OnMessage message pump. The OnMessage method must be called once, otherwise an exception will occur.
            this.client.OnMessageAsync(
                async (msg) =>
                {
                    // Will block the current thread if Stop is called.
                    this.pauseProcessingEvent.WaitOne();

                    try
                    {

                        var msgBody = msg.GetBody<TMsgBody>();
                        // Execute processing task here
                        processMessageTask(msgBody);

                        switch (msgBody.Status)
                        {
                            case Models.TaskStatus.Completed: await msg.CompleteAsync(); break;
                            case Models.TaskStatus.InDeadletter: await msg.DeadLetterAsync(); break;
                            default: await msg.AbandonAsync(); break;
                        }
                    }
                    catch(Exception ex)
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
            this.pauseProcessingEvent.Reset();

            // There is no clean approach to wait for the threads to complete processing.
            // We simply stop any new processing, wait for existing thread to complete, then close the message pump and then return
            Thread.Sleep(waitTime);

            await this.client.CloseAsync();
        }

        private void OptionsOnExceptionReceived(object sender, ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var exceptionMessage = "null";
            if (exceptionReceivedEventArgs != null && exceptionReceivedEventArgs.Exception != null)
            {
                exceptionMessage = exceptionReceivedEventArgs.Exception.Message;
                Trace.TraceError("Exception in QueueClient.ExceptionReceived: {0}", exceptionMessage);
            }
        }


 
    }
}

