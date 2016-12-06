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
        /// <summary>
        /// The lock 0
        /// </summary>
        Lock0 = 0,
        /// <summary>
        /// The lock 1
        /// </summary>
        Lock1 = 1,
        /// <summary>
        /// The lock 2
        /// </summary>
        Lock2 = 2,
        /// <summary>
        /// The lock 3
        /// </summary>
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
        /// <summary>
        /// The unknown
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// The pin00
        /// </summary>
        Pin00 = 0,
        /// <summary>
        /// The pin01
        /// </summary>
        Pin01 = 1,
        /// <summary>
        /// The pin02
        /// </summary>
        Pin02 = 2,
        /// <summary>
        /// The pin03
        /// </summary>
        Pin03 = 3,
        /// <summary>
        /// The pin04
        /// </summary>
        Pin04 = 4,
        /// <summary>
        /// The pin05
        /// </summary>
        Pin05 = 5,
        /// <summary>
        /// The pin06
        /// </summary>
        Pin06 = 6,
        /// <summary>
        /// The pin07
        /// </summary>
        Pin07 = 7,
        /// <summary>
        /// The pin08
        /// </summary>
        Pin08 = 8,
        /// <summary>
        /// The pin09
        /// </summary>
        Pin09 = 9,
        /// <summary>
        /// The pin10
        /// </summary>
        Pin10 = 10,
        /// <summary>
        /// The pin11
        /// </summary>
        Pin11 = 11,
        /// <summary>
        /// The pin12
        /// </summary>
        Pin12 = 12,
        /// <summary>
        /// The pin13
        /// </summary>
        Pin13 = 13,
        /// <summary>
        /// The pin14
        /// </summary>
        Pin14 = 14,
        /// <summary>
        /// The pin15
        /// </summary>
        Pin15 = 15,
        /// <summary>
        /// The pin16
        /// </summary>
        Pin16 = 16,
        /// <summary>
        /// The pin17
        /// </summary>
        Pin17 = 17,
        /// <summary>
        /// The pin18
        /// </summary>
        Pin18 = 18,
        /// <summary>
        /// The pin19
        /// </summary>
        Pin19 = 19,
        /// <summary>
        /// The pin20
        /// </summary>
        Pin20 = 20,
        /// <summary>
        /// The pin21
        /// </summary>
        Pin21 = 21,
        /// <summary>
        /// The pin22
        /// </summary>
        Pin22 = 22,
        /// <summary>
        /// The pin23
        /// </summary>
        Pin23 = 23,
        /// <summary>
        /// The pin24
        /// </summary>
        Pin24 = 24,
        /// <summary>
        /// The pin25
        /// </summary>
        Pin25 = 25,
        /// <summary>
        /// The pin26
        /// </summary>
        Pin26 = 26,
        /// <summary>
        /// The pin27
        /// </summary>
        Pin27 = 27,
        /// <summary>
        /// The pin28
        /// </summary>
        Pin28 = 28,
        /// <summary>
        /// The pin29
        /// </summary>
        Pin29 = 29,
        /// <summary>
        /// The pin30
        /// </summary>
        Pin30 = 30,
        /// <summary>
        /// The pin31
        /// </summary>
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
        /// <summary>
        /// The unknown version
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// The model b rev1
        /// </summary>
        ModelBRev1 = 0x0002,
        /// <summary>
        /// The model b rev1 ec N0001
        /// </summary>
        ModelBRev1ECN0001 = 0x0003,
        /// <summary>
        /// The model b rev2x04
        /// </summary>
        ModelBRev2x04 = 0x0004,
        /// <summary>
        /// The model b rev2x05
        /// </summary>
        ModelBRev2x05 = 0x0005,
        /// <summary>
        /// The model b rev2x06
        /// </summary>
        ModelBRev2x06 = 0x0006,
        /// <summary>
        /// The model ax07
        /// </summary>
        ModelAx07 = 0x0007,
        /// <summary>
        /// The model ax08
        /// </summary>
        ModelAx08 = 0x0008,
        /// <summary>
        /// The model ax09
        /// </summary>
        ModelAx09 = 0x0009,
        /// <summary>
        /// The model b rev2x0d
        /// </summary>
        ModelBRev2x0d,
        /// <summary>
        /// The model b rev2x0e
        /// </summary>
        ModelBRev2x0e,
        /// <summary>
        /// The model b rev2x0f
        /// </summary>
        ModelBRev2x0f = 0x000f,
        /// <summary>
        /// The model b plus0x10
        /// </summary>
        ModelBPlus0x10 = 0x0010,
        /// <summary>
        /// The model b plus0x13
        /// </summary>
        ModelBPlus0x13 = 0x0013,
        /// <summary>
        /// The compute module0x11
        /// </summary>
        ComputeModule0x11 = 0x0011,
        /// <summary>
        /// The compute module0x14
        /// </summary>
        ComputeModule0x14 = 0x0014,
        /// <summary>
        /// The model a plus0x12
        /// </summary>
        ModelAPlus0x12 = 0x0012,
        /// <summary>
        /// The model a plus0x15
        /// </summary>
        ModelAPlus0x15 = 0x0015,
        /// <summary>
        /// The pi2 model B1V1 sony
        /// </summary>
        Pi2ModelB1v1Sony = 0xa01041,
        /// <summary>
        /// The pi2 model B1V1 embest
        /// </summary>
        Pi2ModelB1v1Embest = 0xa21041,
        /// <summary>
        /// The pi2 model B1V2
        /// </summary>
        Pi2ModelB1v2 = 0xa22042,
        /// <summary>
        /// The pi zero1v2
        /// </summary>
        PiZero1v2 = 0x900092,
        /// <summary>
        /// The pi zero1v3
        /// </summary>
        PiZero1v3 = 0x900093,
        /// <summary>
        /// The pi3 model b sony
        /// </summary>
        Pi3ModelBSony = 0xa02082,
        /// <summary>
        /// The pi3 model b embest
        /// </summary>
        Pi3ModelBEmbest = 0xa22082,
    }

    /// <summary>
    /// Defines the available encoding formats for the Raspberry Pi camera module
    /// </summary>
    public enum CameraImageEncodingFormat
    {
        /// <summary>
        /// The JPG
        /// </summary>
        Jpg,
        /// <summary>
        /// The BMP
        /// </summary>
        Bmp,
        /// <summary>
        /// The GIF
        /// </summary>
        Gif,
        /// <summary>
        /// The PNG
        /// </summary>
        Png,
    }

    /// <summary>
    /// Defines the different exposure modes for the Raspberry Pi's camera module
    /// </summary>
    public enum CameraExposureMode
    {
        /// <summary>
        /// The automatic
        /// </summary>
        Auto,
        /// <summary>
        /// The night
        /// </summary>
        Night,
        /// <summary>
        /// The night preview
        /// </summary>
        NightPreview,
        /// <summary>
        /// The backlight
        /// </summary>
        Backlight,
        /// <summary>
        /// The spotlight
        /// </summary>
        Spotlight,
        /// <summary>
        /// The sports
        /// </summary>
        Sports,
        /// <summary>
        /// The snow
        /// </summary>
        Snow,
        /// <summary>
        /// The beach
        /// </summary>
        Beach,
        /// <summary>
        /// The very long
        /// </summary>
        VeryLong,
        /// <summary>
        /// The fixed FPS
        /// </summary>
        FixedFps,
        /// <summary>
        /// The anti shake
        /// </summary>
        AntiShake,
        /// <summary>
        /// The fireworks
        /// </summary>
        Fireworks,
    }

    /// <summary>
    /// Defines the different AWB (Auto White Balance) modes for the Raspberry Pi's camera module
    /// </summary>
    public enum CameraWhiteBalanceMode
    {
        /// <summary>
        /// No white balance
        /// </summary>
        Off,
        /// <summary>
        /// The automatic
        /// </summary>
        Auto,
        /// <summary>
        /// The sun
        /// </summary>
        Sun,
        /// <summary>
        /// The cloud
        /// </summary>
        Cloud,
        /// <summary>
        /// The shade
        /// </summary>
        Shade,
        /// <summary>
        /// The tungsten
        /// </summary>
        Tungsten,
        /// <summary>
        /// The fluorescent
        /// </summary>
        Fluorescent,
        /// <summary>
        /// The incandescent
        /// </summary>
        Incandescent,
        /// <summary>
        /// The flash
        /// </summary>
        Flash,
        /// <summary>
        /// The horizon
        /// </summary>
        Horizon
    }

    /// <summary>
    /// Defines the available image effects for the Raspberry Pi's camera module
    /// </summary>
    public enum CameraImageEffect
    {
        /// <summary>
        /// No effect
        /// </summary>
        None,
        /// <summary>
        /// The negative
        /// </summary>
        Negative,
        /// <summary>
        /// The solarise
        /// </summary>
        Solarise,
        /// <summary>
        /// The whiteboard
        /// </summary>
        Whiteboard,
        /// <summary>
        /// The blackboard
        /// </summary>
        Blackboard,
        /// <summary>
        /// The sketch
        /// </summary>
        Sketch,
        /// <summary>
        /// The denoise
        /// </summary>
        Denoise,
        /// <summary>
        /// The emboss
        /// </summary>
        Emboss,
        /// <summary>
        /// The oil paint
        /// </summary>
        OilPaint,
        /// <summary>
        /// The hatch
        /// </summary>
        Hatch,
        /// <summary>
        /// Graphite Pen
        /// </summary>
        GPen,
        /// <summary>
        /// The pastel
        /// </summary>
        Pastel,
        /// <summary>
        /// The water colour
        /// </summary>
        WaterColour,
        /// <summary>
        /// The film
        /// </summary>
        Film,
        /// <summary>
        /// The blur
        /// </summary>
        Blur,
        /// <summary>
        /// The saturation
        /// </summary>
        Saturation,
        /// <summary>
        /// The solour swap
        /// </summary>
        SolourSwap,
        /// <summary>
        /// The washed out
        /// </summary>
        WashedOut,
        /// <summary>
        /// The colour point
        /// </summary>
        ColourPoint,
        /// <summary>
        /// The colour balance
        /// </summary>
        ColourBalance,
        /// <summary>
        /// The cartoon
        /// </summary>
        Cartoon
    }

    /// <summary>
    /// Defines the different metering modes for the Raspberry Pi's camera module
    /// </summary>
    public enum CameraMeteringMode
    {
        /// <summary>
        /// The average
        /// </summary>
        Average,
        /// <summary>
        /// The spot
        /// </summary>
        Spot,
        /// <summary>
        /// The backlit
        /// </summary>
        Backlit,
        /// <summary>
        /// The matrix
        /// </summary>
        Matrix,
    }

    /// <summary>
    /// Defines the different image rotation modes for the Raspberry Pi's camera module
    /// </summary>
    public enum CameraImageRotation
    {
        /// <summary>
        /// No rerotation
        /// </summary>
        None = 0,
        /// <summary>
        /// 90 Degrees
        /// </summary>
        Degrees90 = 90,
        /// <summary>
        /// 180 Degrees
        /// </summary>
        Degrees180 = 180,
        /// <summary>
        /// 270 degrees
        /// </summary>
        Degrees270 = 270
    }

    /// <summary>
    /// Defines the different DRC (Dynamic Range Compensation)  modes for the Raspberry Pi's camera module
    /// Helpful for low light photos
    /// </summary>
    public enum CameraDynamicRangeCompensation
    {
        /// <summary>
        /// The off setting
        /// </summary>
        Off,
        /// <summary>
        /// The low
        /// </summary>
        Low,
        /// <summary>
        /// The medium
        /// </summary>
        Medium,
        /// <summary>
        /// The high
        /// </summary>
        High
    }

    /// <summary>
    /// Defines the bit-wise mask flags for the available annotation elements for the Raspberry Pi's camera module
    /// </summary>
    [Flags]
    public enum CameraAnnotation
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,
        /// <summary>
        /// The time
        /// </summary>
        Time = 4,
        /// <summary>
        /// The date
        /// </summary>
        Date = 8,
        /// <summary>
        /// The shutter settings
        /// </summary>
        ShutterSettings = 16,
        /// <summary>
        /// The caf settings
        /// </summary>
        CafSettings = 32,
        /// <summary>
        /// The gain settings
        /// </summary>
        GainSettings = 64,
        /// <summary>
        /// The lens settings
        /// </summary>
        LensSettings = 128,
        /// <summary>
        /// The motion settings
        /// </summary>
        MotionSettings = 256,
        /// <summary>
        /// The frame number
        /// </summary>
        FrameNumber = 512,
        /// <summary>
        /// The solid background
        /// </summary>
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
