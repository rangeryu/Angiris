using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.Core.Models
{
    public enum TaskStatus
    {
        New = 0,
        Queueing = 1,
        Processing = 2,
        Completed = 3,
        Failed = 4,
        InDeadletter = 5,
        
    }
}
