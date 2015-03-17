namespace Angiris.Backend.Core

{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
    using Angiris.Core.Models;
using Angiris.Core.DataStore;
    using System.Threading.Tasks;

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
            this.StatusData.IsStarted = true;

            int robotCount = 10;
            int robotCountP0 = 5;

            for (int i = 0; i < robotCount;i++ )
            {
                CreateTaskRobot(false);
            }

            for (int i = 0; i < robotCountP0; i++)
            {
                CreateTaskRobot(true);
            }

            while(this.StatusData.IsStarted)
            {
                await SyncStatus();
                await Task.Delay(5000);
            }

            
		}

		public async Task SyncStatus()
		{
            StatusData.MemoryRatio = perfCounter.GetMemoryRatio();
            StatusData.CPURatio = perfCounter.GetCPURatio();
            StatusData.LastUpdated = DateTime.UtcNow;
            StatusData.Remark = "Uptime: " + (StatusData.LastUpdated - StatusData.StartTime).ToString();
            StatusData.CrawlerCount = this.TaskRobotList.Count;

            await daemonStatusStore.UpdateEntity(StatusData.InstanceName, StatusData);
            //sync robot resouce plan here from central admin.

		}

        public async Task Stop()
		{
            await Task.Run(() =>
            {
                foreach (var robot in this.TaskRobotList)
                {
                    robot.Stop();
                    robot.Dispose();
                }
                this.TaskRobotList.Clear();
                this.StatusData.IsStarted = false;
            });
		}

        public void CreateTaskRobot(bool isP0)
		{
            FlightCrawlRobot robot = new FlightCrawlRobot(isP0);
            robot.Start();
            this.TaskRobotList.Add(robot);
		}
 

	}
}

