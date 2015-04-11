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
using Microsoft.WindowsAzure.Storage;
using Angiris.Backend.Core;

namespace BackendWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        //private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private RobotDaemon daemonInstance;

        public override void Run()
        {
            Trace.TraceInformation("BackendWorkerRole is running");

            try
            {
                daemonInstance.Start().Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            //ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            daemonInstance = new RobotDaemon();

            bool result = base.OnStart();
            
            Trace.TraceInformation("BackendWorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("BackendWorkerRole is stopping");

            daemonInstance.Stop().Wait();
            //this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("BackendWorkerRole has stopped");
        }

 
    }
}
