using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.Core.Models
{
    public interface IQueuedTask
    {
        Guid TaskID { get; set; }
        string LogData { get; set; }

        DateTime CreateTime { get; set; }

        DateTime FinishTime { get; set; }

        DateTime LastModifiedTime { get; set; }

        TaskStatus Status { get; set; }
    }
}
