using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.Core.Models
{
    public class DaemonStatus
    {
        public string InstanceName { get; set; }
        public DateTime StartTime { get; set; }
        public int CPURatio { get; set; }
        public int MemoryRatio { get; set; }
        public int NetworRatio { get; set; }
        public int CrawlerCount { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<string> LogData { get; set; }
        public string Remark { get; set; }

        public bool IsStarted { get; set; }
    }
}
