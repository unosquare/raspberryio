namespace Unosquare.RaspberryIO
{
    using System;

    /// <summary>
    /// Defines the different drive modes of a GPIO pin
    /// </summary>
    public enum GpioPinDriveMode : int
    {
        /// <summary>
        /// Input drive mode (perform reads)
        /// </summary>
        Input = 0,
        /// <summary>
        /// Output drive mode (perform writes)
        /// </summary>
        Output = 1,
        /// <summary>
        /// PWM output mode (only certain pins support this -- 2 of them at the moment)
        /// </summary>
        PwmOutput = 2,
        /// <summary>
        /// GPIO Clock output mode (only a pin supports this at this time)
        /// </summary>
        GpioClock = 3
    }

    /// <summary>
    /// The GPIO pin resistor mode. This is used on input pins so that their
    /// lines are not floating
    /// </summary>
    public enum GpioPinResistorPullMode : int
    {
        /// <summary>
        /// Pull resistor not active. Line floating
        /// </summary>
        Off = 0,
        /// <summary>
        /// Pull resistor sets a default value of 0 on no-connects
        /// </summary>
        PullDown = 1,
        /// <summary>
        /// Pull resistor sets a default value of 1 on no-connects
        /// </summary>
        PullUp = 2,
    }

    /// <summary>
    /// The PWM mode.
    /// </summary>
    public enum PwmMode : int
    {
        /// <summary>
        /// PWM pulses are sent using mark-sign patterns (old school)
        /// </summary>
        MarkSign = 0,
        /// <summary>
        /// PWM pulses are sent as a balanced signal (default, newer mode)
        /// </summary>
        Balanced = 1,
    }

    /// <summary>
    /// Defines the different threading locking keys
    /// </summary>
    public enum LockKey : int
    {
        Lock0 = 0,
        Lock1 = 1,
        Lock2 = 2,
        Lock3 = 3,
    }

    /// <summary>
    /// Defines the different edge detection modes  for pin interrupts
    /// </summary>
    public enum EdgeDetection : int
    {
        /// <summary>
        /// Assumes edge detection was already setup externally
        /// </summary>
        ExternalSetup = 0,
        /// <summary>
        /// Falling Edge
        /// </summary>
        FallingEdge = 1,
        /// <summary>
        /// Rising edge
        /// </summary>
        RisingEdge = 2,
        /// <summary>
        /// Both, rising and falling edges
        /// </summary>
        RisingAndFallingEdges = 3
    }

    /// <summary>
    /// Defines the GPIO Pin values 0 for low, 1 for High
    /// </summary>
    public enum GpioPinValue : int
    {
        /// <summary>
        /// Digital high
        /// </summary>
        High = 1,
        /// <summary>
        /// Digital low
        /// </summary>
        Low = 0
    }

    /// <summary>
    /// Defines the SPI channel numbers
    /// </summary>
    public enum SpiChannelNumber
    {
        /// <summary>
        /// The channel 0
        /// </summary>
        Channel0 = 0,
        /// <summary>
        /// The channel 1
        /// </summary>
        Channel1 = 1,
    }

    /// <summary>
    /// Defines GPIO controller initialization modes
    /// </summary>
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

    /// <summary>
    /// Defines the available encoding formats for the Raspberry Pi camera module
    /// </summary>
    public enum CameraImageEncodingFormat
    {
        Jpg,
        Bmp,
        Gif,
        Png,
    }

    /// <summary>
    /// Defines the different exposure modes for the Raspberry Pi's camera module
    /// </summary>
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

    /// <summary>
    /// Defines the different AWB (Auto White Balance) modes for the Raspberry Pi's camera module
    /// </summary>
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

    /// <summary>
    /// Defines the available image effects for the Raspberry Pi's camera module
    /// </summary>
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

    /// <summary>
    /// Defines the different metering modes for the Raspberry Pi's camera module
    /// </summary>
    public enum CameraMeteringMode
    {
        Average,
        Spot,
        Backlit,
        Matrix,
    }

    /// <summary>
    /// Defines the different image rotation modes for the Raspberry Pi's camera module
    /// </summary>
    public enum CameraImageRotation
    {
        None = 0,
        Degrees90 = 90,
        Degrees180 = 180,
        Degrees270 = 270
    }

    /// <summary>
    /// Defines the different DRC (Dynamic Range Compensation)  modes for the Raspberry Pi's camera module
    /// Helpful for low light photos
    /// </summary>
    public enum CameraDynamicRangeCompensation
    {
        Off,
        Low,
        Medium,
        High
    }

    /// <summary>
    /// Defines the bit-wise mask flags for the available annotation elements for the Raspberry Pi's camera module
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

    /// <summary>
    /// Defines the different H.264 encoding profiles to be used when capturing video.
    /// </summary>
    public enum CameraH264Profile
    {
        /// <summary>
        /// BP:  Primarily for lower-cost applications with limited computing resources, 
        /// this profile is used widely in videoconferencing and mobile applications.
        /// </summary>
        Baseline,
        /// <summary>
        /// MP: Originally intended as the mainstream consumer profile for broadcast 
        /// and storage applications, the importance of this profile faded when the High profile was developed for those applications.
        /// </summary>
        Main,
        /// <summary>
        /// HiP: The primary profile for broadcast and disc storage applications, particularly 
        /// for high-definition television applications (this is the profile adopted into HD DVD and Blu-ray Disc, for example).
        /// </summary>
        High
    }
}
