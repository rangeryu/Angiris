using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.Backend.Core
{
    public class PerfCounter
    {
        //for counter list
        //https://technet.microsoft.com/en-us/library/cc780836(WS.10).aspx

        PerformanceCounter ratio_cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        PerformanceCounter ratio_memory = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        //PerformanceCounter ratio_network = new PerformanceCounter("Memory", "% Committed Bytes In Use");

        public PerfCounter()
        {
            //it will always return 0 at first call. so invoke it during constructor.
            ratio_cpu.NextValue();
            ratio_memory.NextValue();
        }

        public int GetCPURatio()
        {
            return (int)ratio_cpu.NextValue();
        }

        public int GetMemoryRatio()
        {
            return (int)ratio_memory.NextValue();
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <returns></returns>
        public int GetNetworkRatio()
        {
            return 0;
        }

    }
    
}
