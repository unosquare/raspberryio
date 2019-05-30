namespace Unosquare.RaspberryIO.Peripherals
{
    /// <summary>
    /// Enum FSSEL.
    /// </summary>
    public enum FSSEL
    {
        /// <summary>
        /// Gyroscope Full Scale Range 250
        /// </summary>
        FSR250 = 0x00,

        /// <summary>
        /// Gyroscope Full Scale Range 500
        /// </summary>
        FSR500 = 0x01,

        /// <summary>
        /// Gyroscope Full Scale Range 1000
        /// </summary>
        FSR1000 = 0x02,

        /// <summary>
        /// Gyroscope Full Scale Range 2000
        /// </summary>
        FSR2000 = 0x03,
    }

    /// <summary>
    /// Enum AFSSEL.
    /// </summary>
    public enum AFSSEL
    {
        /// <summary>
        /// Accelerometer Full Scale Range 2g
        /// </summary>
        FSR2G = 0x00,

        /// <summary>
        /// Accelerometer Full Scale Range 4g
        /// </summary>
        FSR4G = 0x01,

        /// <summary>
        /// Accelerometer Full Scale Range 8g
        /// </summary>
        FSR8G = 0x02,

        /// <summary>
        /// Accelerometer Full Scale Range 16g
        /// </summary>
        FSR16G = 0x03,
    }
}
