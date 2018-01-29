using System;
using System.Diagnostics;

namespace Unosquare.RaspberryIO
{
    public class HighResolutionTimer : Stopwatch
    {
        readonly double _microSecPerTick =
            1000000D / Frequency;

        public HighResolutionTimer()
        {
            if (!System.Diagnostics.Stopwatch.IsHighResolution)            
                throw new Exception("On this system the high-resolution " +
                                    "performance counter is not available");            
        }

        public long ElapsedMicroseconds
        {
            get
            {
                return (long)(ElapsedTicks * _microSecPerTick);
            }
        }
    }
}
