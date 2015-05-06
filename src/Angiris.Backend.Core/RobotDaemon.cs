namespace Angiris.Backend.Core

{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
    using Angiris.Core.Models;
    using Angiris.Core.DataStore;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using Angiris.Core.Messaging;
    using System.Net;

	public class RobotDaemon
	{
        public DaemonStatus StatusData { get; private set; }

	    PerfCounter _perfCounter;
        RedisDaemonStatusProvider _daemonStatusStore;

        public RobotDaemon()
        {
            TaskRobotList = new List<FlightCrawlRobot>();
            StatusData = new DaemonStatus() { StartTime = DateTime.UtcNow, InstanceName = System.Environment.MachineName };
            _perfCounter = new PerfCounter();
          
            
        }

        public List<FlightCrawlRobot> TaskRobotList
		{
			get;
			private set;
		}

        public async Task Start(DaemonConfig config)
		{
            //https://msdn.microsoft.com/en-us/library/system.net.servicepointmanager.defaultconnectionlimit(v=vs.110).aspx
            ServicePointManager.DefaultConnectionLimit = 50;

            DataProviderFactory.InitializeAll();

            _daemonStatusStore = DataProviderFactory.SingletonRedisDaemonStatusProvider;
          

            this.StatusData.IsStarted = true;
 
            Trace.TraceInformation("Creating robots...");

            for (int i = 0; i < config.FlightCrawlRobotP0; i++)
            {
                CreateTaskRobot(true);
            }

            for (int i = 0; i < config.FlightCrawlRobotP1; i++)
            {
                CreateTaskRobot(false);
            }


            Trace.TraceInformation("Starting robots...");
            this.TaskRobotList.ForEach(r => r.Start());

            while(this.StatusData.IsStarted)
            {
                await SyncStatus();
                await Task.Delay(3000);
            }
            
		}

		public async Task SyncStatus()
		{
            StatusData.MemoryRatio = _perfCounter.GetMemoryRatio();
            StatusData.CPURatio = _perfCounter.GetCPURatio();
            StatusData.LastUpdated = DateTime.UtcNow;
            StatusData.Remark = "Uptime: " + (StatusData.LastUpdated - StatusData.StartTime).ToString();
            StatusData.CrawlerCount = this.TaskRobotList.Count;
            StatusData.RobotStatusList = this.TaskRobotList.Select(r => r.Status).ToList();

            try
            { 
                await _daemonStatusStore.UpdateEntity(StatusData.InstanceName, StatusData);
                Trace.TraceInformation(this.StatusData.ToString());
            }
            catch(Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
            //sync robot resouce plan here from central admin.
            
		}

        public async Task Stop()
		{
            Trace.TraceInformation("Stopping Daemon in 30 seconds");

            await this.SyncStatus();

            int maxTimeoutMs = 30000;

            Task.WaitAll(this.TaskRobotList.Select(robot => robot.Stop()).ToArray(), maxTimeoutMs);

            this.TaskRobotList.Clear();

            await this.SyncStatus();

            this.StatusData.IsStarted = false;

		}

        public void CreateTaskRobot(bool isP0)
        {
            //await Task.Run(() =>
            // {
            var qMgrProfile = QueueMgrProfile.Default;
            qMgrProfile.IsHighPriority = isP0;

            FlightCrawlRobot robot = new FlightCrawlRobot(qMgrProfile);
            robot.Initialize();
            this.TaskRobotList.Add(robot);
            // });
        }
 

	}
}

