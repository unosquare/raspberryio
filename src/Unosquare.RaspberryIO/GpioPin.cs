using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unosquare.RaspberryIO
{
    public class GpioPin
    {
        protected GpioPin(int wiringPiPinNumber, )
        {

        }

        public int WiringPiPinNumber { get; protected set; }
        public int BcmPinNumber { get; protected set; }
        public int HeaderPinNumber { get; protected set; }
        public GpioHeader Header { get; protected set; }
        public string Name { get; protected set; }
        public PinCapability[] Capabilities { get; protected set; }
        public bool IsLocked { get; set; } = false;

        static private readonly GpioPin Pin08 = new GpioPin()
        {
            
        }
    }
}
