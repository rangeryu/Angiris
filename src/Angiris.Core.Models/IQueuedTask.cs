using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.Core.Models
{
    public interface IQueuedTask
    {
        [JsonProperty(PropertyName = "id")]
        string TaskId { get; set; }
        List<string> LogData { get; set; }

        DateEpoch CreateTime { get; set; }

        DateEpoch FinishTime { get; set; }

        DateEpoch LastModifiedTime { get; set; }

        TaskStatus Status { get; set; }

        int MaxExecutionTimeInMs { get; set; }
    }
}
