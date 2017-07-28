namespace Unosquare.RaspberryIO.Camera
{
    using System;

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
        Fireworks
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
