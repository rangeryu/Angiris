using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Angiris.AppConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            { 
                var a = Convert.ToInt32(DateTime.Now.Ticks);
                Console.WriteLine(a);
                Console.ReadLine();
            }
        }
    }
}
