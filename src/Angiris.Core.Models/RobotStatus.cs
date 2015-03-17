using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.Core.Models
{
    public class RobotStatus
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int ConcurrentJobCount { get; set; }
        public int TaskReceivedCount { get; set; }
        public DateTime StartTime { get; set; }
        public List<string> LogData { get; set; }

    }
}
