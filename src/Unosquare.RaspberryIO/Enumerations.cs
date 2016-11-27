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

    public enum GpioHeader
    {
        None,
        P1,
        P2,
    }

    public enum PinCapability
    {
        DigitalRead,
        DigitalWrite,
        AnalogRead,
        AnalogWrite,
    }

    /// <summary>
    /// http://www.raspberrypi-spy.co.uk/2012/09/checking-your-raspberry-pi-board-version/
    /// </summary>
    public enum RaspberryPiVersion
    {
        Unknown = 0,
        ModelBRev1 = 0x0002,
        ModelBRev1ECN0001 = 0x0003,
        ModelBRev2x04 = 0x0004,
        ModelBRev2x05 = 0x0005,
        ModelBRev2x06 = 0x0006,
        ModelAx07 = 0x0007,
        ModelAx08 = 0x0008,
        ModelAx09 = 0x0009,
        ModelBRev2x0d,
        ModelBRev2x0e,
        ModelBRev2x0f = 0x000f,
        ModelBPlus0x10 = 0x0010,
        ModelBPlus0x13 = 0x0013,
        ComputeModule0x11 = 0x0011,
        ComputeModule0x14 = 0x0014,
        ModelAPlus0x12 = 0x0012,
        ModelAPlus0x15 = 0x0015,
        Pi2ModelB1v1Sony = 0xa01041,
        Pi2ModelB1v1Embest = 0xa21041,
        Pi2ModelB1v2 = 0xa22042,
        PiZero1v2 = 0x900092,
        PiZero1v3 = 0x900093,
        Pi3ModelBSony = 0xa02082,
        Pi3ModelBEmbest = 0xa22082,
    }
}
