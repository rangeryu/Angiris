using Angiris.Backend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Angiris.Backend.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            PerfCounter counter = new PerfCounter();

            while(true)
            {
                Console.WriteLine("CPU " + counter.GetCPURatio());
                Console.WriteLine("Memory " + counter.GetMemoryRatio());
                Thread.Sleep(1000);
            }
        }
    }
}
