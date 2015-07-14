using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Angiris.Backend.Core;
using Microsoft.Azure;
using Newtonsoft.Json;


namespace BackendWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        //private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);
        private RobotDaemon _daemonInstance;

        public override void Run()
        {
            Trace.TraceInformation("BackendWorkerRole is running");

            try
            {
                //TODO retrive config from central admin

                // xml encoded string: 
                //raw in cfg:  { &quot;FlightCrawlRobotP0&quot;:3,&quot;FlightCrawlRobotP1&quot;:2 } 
                //read by API: { "FlightCrawlRobotP0":3,"FlightCrawlRobotP1":2 }
                var daemonCfgStr = CloudConfigurationManager.GetSetting("Angiris.DaemonConfig");

                var daemonCfg = JsonConvert.DeserializeObject<Angiris.Core.Models.DaemonConfig>(daemonCfgStr);

                //var daemonCfg = new Angiris.Core.Models.DaemonConfig
                //{
                //    FlightCrawlRobotP0 = 3,
                //    FlightCrawlRobotP1 = 2
                //};

                _daemonInstance.Start(daemonCfg).Wait();
            }
            finally
            {
                this._runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            //ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            _daemonInstance = new RobotDaemon();

            bool result = base.OnStart();
            
            Trace.TraceInformation("BackendWorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("BackendWorkerRole is stopping");

            _daemonInstance.Stop().Wait();
            //this.cancellationTokenSource.Cancel();
            this._runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("BackendWorkerRole has stopped");
        }

 
    }
}
