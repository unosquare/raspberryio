using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unosquare.RaspberryIO
{
    public enum PinMode : int
    {
        Input = 0,
        Output = 1,
        PwmOutput = 2,
        GpioClock = 3
    }

    public enum InterruptLevels : int
    {
        EdgeSetup = 0,
        EdgeFalling = 1,
        EdgeRising = 2,
        EdgeBoth = 3
    }

    internal enum PinValue : int
    {
        High = 1,
        Low = 0
    }

    public enum ControllerMode
    {
        NotInitialized,
        DirectWithMappedPins,
        DirectWithHardwarePins,
        DirectWithNamedPins,
        FileStreamWithHardwarePins,
    }


}
