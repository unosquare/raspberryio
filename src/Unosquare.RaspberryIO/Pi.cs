using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unosquare.RaspberryIO
{
    /// <summary>
    /// Static class that provides access to the Raspberry Pi's GPIO, system information and Camera
    /// </summary>
    public static class Pi
    {
        /// <summary>
        /// Provides access to the Raspberry Pi's GPIO as a collection of GPIO Pins.
        /// </summary>
        static public GpioController Gpio { get { return GpioController.Instance; } }
        
        /// <summary>
        /// Provides information on this Raspberry Pi's CPU and form factor.
        /// </summary>
        static public SystemInfo Info { get { return SystemInfo.Instance; } }
    }
}
