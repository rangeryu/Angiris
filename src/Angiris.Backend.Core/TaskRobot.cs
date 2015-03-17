namespace Angiris.Backend.Core
{
    using Angiris.Core.Messaging;
    using Angiris.Core.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public abstract class TaskRobot
	{
        public event EventHandler TaskCompleted;		 

		public event EventHandler TaskFailed;

        public event EventHandler TimeOut;
		 


 
		public abstract void Start();


        public abstract void Stop();

        public abstract void Dispose();

        public abstract void ExecuteTask();

	}
}

