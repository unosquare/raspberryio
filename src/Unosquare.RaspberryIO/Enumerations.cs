namespace Unosquare.RaspberryIO
{
    using System;

    public enum GpioPinDriveMode : int
    {
        Input = 0,
        Output = 1,
        PwmOutput = 2,
        GpioClock = 3
    }

    public enum GpioPinResistorPullMode : int
    {
        Off = 0,
        PullDown = 1,
        PullUp = 2,
    }

    public enum PwmMode : int
    {
        MarkSign = 0,
        Balanced = 1,
    }

    public enum LockKey : int
    {
        Lock0,
        Lock1,
        Lock2,
        Lock3,
    }

    public enum EdgeDetection : int
    {
        EdgeSetup = 0,
        EdgeFalling = 1,
        EdgeRising = 2,
        EdgeBoth = 3
    }

    public enum GpioPinValue : int
    {
        High = 1,
        Low = 0
    }

    public enum SpiChannelNumber
    {
        Channel0 = 0,
        Channel1 = 1,
    }

    internal enum ControllerMode
    {
        NotInitialized,
        DirectWithWiringPiPins,
        DirectWithBcmPins,
        DirectWithHeaderPins,
        FileStreamWithHardwarePins,
    }

    /// <summary>
    /// Defines the Header connectors available
    /// </summary>
    public enum GpioHeader
    {
        /// <summary>
        /// Not defined
        /// </summary>
        None,
        /// <summary>
        /// The P1 connector (main connector)
        /// </summary>
        P1,
        /// <summary>
        /// The P5 connector (auxiliary, not commenly used)
        /// </summary>
        P5,
    }

    /// <summary>
    /// Defines all the available Wiring Pi Pin Numbers
    /// </summary>
    public enum WiringPiPin
    {
        Unknown = -1,
        Pin00 = 0,
        Pin01 = 1,
        Pin02 = 2,
        Pin03 = 3,
        Pin04 = 4,
        Pin05 = 5,
        Pin06 = 6,
        Pin07 = 7,
        Pin08 = 8,
        Pin09 = 9,
        Pin10 = 10,
        Pin11 = 11,
        Pin12 = 12,
        Pin13 = 13,
        Pin14 = 14,
        Pin15 = 15,
        Pin16 = 16,
        Pin17 = 17,
        Pin18 = 18,
        Pin19 = 19,
        Pin20 = 20,
        Pin21 = 21,
        Pin22 = 22,
        Pin23 = 23,
        Pin24 = 24,
        Pin25 = 25,
        Pin26 = 26,
        Pin27 = 27,
        Pin28 = 28,
        Pin29 = 29,
        Pin30 = 30,
        Pin31 = 31,
    }

    /// <summary>
    /// Defines the different pin capabilities
    /// </summary>
    public enum PinCapability
    {
        /// <summary>
        /// General Purpose capability: Digitala nd Analog Read/Write
        /// </summary>
        GP,
        /// <summary>
        /// General Purpose Clock (not PWM)
        /// </summary>
        GPCLK,
        /// <summary>
        /// i2c data channel
        /// </summary>
        I2CSDA,
        /// <summary>
        /// i2c clock channel
        /// </summary>
        I2CSCL,
        /// <summary>
        /// SPI Master Out, Slave In channel
        /// </summary>
        SPIMOSI,
        /// <summary>
        /// SPI Master In, Slave Out channel
        /// </summary>
        SPIMISO,
        /// <summary>
        /// SPI Clock channel
        /// </summary>
        SPICLK,
        /// <summary>
        /// SPI Chip Select Channel
        /// </summary>
        SPICS,
        /// <summary>
        /// UART Request to Send Channel
        /// </summary>
        UARTRTS,
        /// <summary>
        /// UART Transmit Channel
        /// </summary>
        UARTTXD,
        /// <summary>
        /// UART Receive Channel
        /// </summary>
        UARTRXD,
        /// <summary>
        /// Hardware Pule Width Modulation
        /// </summary>
        PWM
    }

    /// <summary>
    /// Defines the board revision codes of the different versions of the Raspberry Pi
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

    public enum CameraImageEncodingFormat
    {
        Jpg,
        Bmp,
        Gif,
        Png,
    }

    public enum CameraExposureMode
    {
        Auto,
        Night,
        NightPreview,
        Backlight,
        Spotlight,
        Sports,
        Snow,
        Beach,
        VeryLong,
        FixedFps,
        AntiShake,
        Fireworks,
    }

    public enum CameraWhiteBalanceMode
    {
        Off,
        Auto,
        Sun,
        Cloud,
        Shade,
        Tungsten,
        Fluorescent,
        Incandescent,
        Flash,
        Horizon
    }

    public enum CameraImageEffect
    {
        None,
        Negative,
        Solarise,
        Whiteboard,
        Blackboard,
        Sketch,
        Denoise,
        Emboss,
        OilPaint,
        Hatch,
        GPen,
        Pastel,
        WaterColour,
        Film,
        Blur,
        Saturation,
        SolourSwap,
        WashedOut,
        ColourPoint,
        ColourBalance,
        Cartoon
    }

    public enum CameraMeteringMode
    {
        Average,
        Spot,
        Backlit,
        Matrix,
    }

    public enum CameraImageRotation
    {
        None = 0,
        Degrees90 = 90,
        Degrees180 = 180,
        Degrees270 = 270
    }

    public enum CameraDynamicRangeCompensation
    {
        Off,
        Low,
        Medium,
        High
    }

    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum CameraAnnotation
    {
        None = 0,
        Time = 4,
        Date = 8,
        ShutterSettings = 16,
        CafSettings = 32,
        GainSettings = 64,
        LensSettings = 128,
        MotionSettings = 256,
        FrameNumber = 512,
        SolidBackground = 1024,
    }

    public enum CameraH264Profile
    {
        Baseline,
        Main,
        High
    }
}
