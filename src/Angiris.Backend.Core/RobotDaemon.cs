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

		public List<TaskRobot> TaskRobotList
		{
			get;
			private set;
		}

        public async Task Start()
		{
            TaskRobotList = new List<TaskRobot>();
            StatusData = new DaemonStatus() { StartTime = DateTime.UtcNow, InstanceName = System.Environment.MachineName };
            perfCounter = new PerfCounter();
            daemonStatusStore = DataProviderFactory.GetRedisDaemonStatusProvider();
            await SyncStatus();
		}

		public async Task SyncStatus()
		{

            StatusData.MemoryRatio = perfCounter.GetMemoryRatio();
            StatusData.CPURatio = perfCounter.GetCPURatio();
            StatusData.LastUpdated = DateTime.UtcNow;
            StatusData.Remark = "Uptime: " + (StatusData.LastUpdated - StatusData.StartTime).ToString();

            await daemonStatusStore.UpdateEntity(StatusData.InstanceName, StatusData);


            //sync robot resouce plan here from central admin.

		}

        public async Task Stop()
		{
			throw new System.NotImplementedException();
		}

        public async Task CreateTaskRobot()
		{
			throw new System.NotImplementedException();
		}

        public async Task DisposeTaskRobot()
		{
			throw new System.NotImplementedException();
		}

	}
}

