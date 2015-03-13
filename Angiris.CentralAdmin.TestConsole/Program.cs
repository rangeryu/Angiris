using Angiris.CentralAdmin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.CentralAdmin.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ScheduledFlightCrawlRequestFactory flightCrawlRequestFactory = new ScheduledFlightCrawlRequestFactory();

            var startTime = DateTime.Now;
            var task = Task.Run(async () => {

                try
                {
                    await flightCrawlRequestFactory.StartPushTaskMessages();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message + System.Environment.NewLine + ex.InnerException.Message);
                }
                

            });
            task.Wait();
            var endTime = DateTime.Now;
            Console.WriteLine("End in " + (endTime - startTime).TotalSeconds + " seconds");
            Console.ReadLine();
        }


    }
}
