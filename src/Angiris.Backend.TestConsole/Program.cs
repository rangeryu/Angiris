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
            RobotDaemon daemon = new RobotDaemon();

            Console.WriteLine("Starting");

            Task daemonStartTask = daemon.Start();

            Console.WriteLine("press any key to exit.");

            Console.ReadLine();

            daemon.Stop().Wait();

        }
    }
}
