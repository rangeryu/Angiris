namespace Angiris.Backend.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public abstract class TaskRobot
	{
		public virtual object TaskCompleted
		{
			get;
			set;
		}

		public virtual object TaskFailed
		{
			get;
			set;
		}

		public virtual object TimeOut
		{
			get;
			set;
		}

		public virtual object Status
		{
			get;
			set;
		}

		public virtual object QueueTopicMgr
		{
			get;
			set;
		}

		public virtual void Start()
		{
			throw new System.NotImplementedException();
		}

		public virtual void Stop()
		{
			throw new System.NotImplementedException();
		}

		public virtual void Dispose()
		{
			throw new System.NotImplementedException();
		}

		public virtual void ExecuteTask()
		{
			throw new System.NotImplementedException();
		}

	}
}

