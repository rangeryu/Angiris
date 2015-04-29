using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Angiris.Core.Utility
{
    /// <summary>
    /// http://stackoverflow.com/questions/13252520/system-random-default-constructor-system-clock-resolution
    /// </summary>
    public static class RandomHelper
    {
        private static int seedCounter = new Random().Next();

        [ThreadStatic]
        private static Random _rng;

        public static Random Instance
        {
            get
            {
                if (_rng == null)
                {
                    int seed = Interlocked.Increment(ref seedCounter);
                    _rng = new Random(seed);
                }
                return _rng;
            }
        }
    }
}
