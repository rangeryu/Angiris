using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.Core.Messaging
{
    public class QueueMgrProfile
    {
        public bool IsHighPriority { get; set; }
        public int MaxConcurrentCalls { get; set; }
        public int ClientPrefetchCount { get; set; }

        public static QueueMgrProfile Default
        {
            get
            {
                return new QueueMgrProfile { ClientPrefetchCount = 100, IsHighPriority = false, MaxConcurrentCalls = 200 };
            }
        }
        
    }
}
