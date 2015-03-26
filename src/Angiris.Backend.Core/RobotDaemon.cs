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

	public class RobotDaemon
	{
        public DaemonStatus StatusData { get; private set; }

        PerfCounter perfCounter;
        INoSQLStoreProvider<DaemonStatus> daemonStatusStore;

        public RobotDaemon()
        {
            TaskRobotList = new List<FlightCrawlRobot>();
            StatusData = new DaemonStatus() { StartTime = DateTime.UtcNow, InstanceName = System.Environment.MachineName };
            perfCounter = new PerfCounter();
            daemonStatusStore = DataProviderFactory.GetRedisDaemonStatusProvider();
            
        }

        public List<FlightCrawlRobot> TaskRobotList
		{
			get;
			private set;
		}

        public async Task Start()
		{
            daemonStatusStore.Initialize();

            this.StatusData.IsStarted = true;

            int robotCount = 7;
            int robotCountP0 = 3;

            Trace.TraceInformation("Creating robots...");
            for (int i = 0; i < robotCount;i++ )
            {
                CreateTaskRobot(false);
            }

            for (int i = 0; i < robotCountP0; i++)
            {
                CreateTaskRobot(true);
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
            StatusData.MemoryRatio = perfCounter.GetMemoryRatio();
            StatusData.CPURatio = perfCounter.GetCPURatio();
            StatusData.LastUpdated = DateTime.UtcNow;
            StatusData.Remark = "Uptime: " + (StatusData.LastUpdated - StatusData.StartTime).ToString();
            StatusData.CrawlerCount = this.TaskRobotList.Count;
            StatusData.RobotStatusList = this.TaskRobotList.Select(r => r.Status).ToList();

            try
            { 
                await daemonStatusStore.UpdateEntity(StatusData.InstanceName, StatusData);
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

            this.StatusData.IsStarted = false;
            await this.SyncStatus();

            List<Task> stopTasks = new List<Task>();
            foreach (var robot in this.TaskRobotList)
            {
                stopTasks.Add(robot.Stop());
            }
            Task.WaitAll(stopTasks.ToArray(), 30000);
            this.TaskRobotList.Clear();

            await this.SyncStatus();
		}

        public void CreateTaskRobot(bool isP0)
		{
            //await Task.Run(() =>
           // {
                FlightCrawlRobot robot = new FlightCrawlRobot(isP0);
                robot.Initialize();
                this.TaskRobotList.Add(robot);
           // });
		}
 

	}
}

