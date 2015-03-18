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

        public List<RobotStatus> RobotStatusList { get; set; }


        public override string ToString()
        {
           string output= string.Format("Instance:{0}\r\n"
               + "StartTime:{1}\r\n"
               + "CPURatio:{2}%\r\n"
               + "MemoryRatio:{3}%\r\n"
               + "CrawlerCount:{4}\r\n"
               + "LastUpdated:{5}\r\n"
               + "Remark:{6}\r\n"
               + "IsStarted:{7}\r\n",
               this.InstanceName, this.StartTime, this.CPURatio, this.MemoryRatio, this.CrawlerCount, this.LastUpdated, this.Remark, this.IsStarted
                );

           return output;
        }
    }
}
