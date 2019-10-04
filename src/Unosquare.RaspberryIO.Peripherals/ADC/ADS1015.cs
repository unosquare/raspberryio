namespace Unosquare.RaspberryIO.Peripherals
{
    /// <summary>
    /// ADS1015 Device.
    /// </summary>
    public class ADS1015 : ADS1x15
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ADS1015"/> class.
        /// </summary>
        /// <param name="address">The I2C Address for this device.</param>
        public ADS1015(byte address = ADS1x15ADDRESS)
            : base(ADS1015CONVERSIONDELAY, 4, address)
        {
        }
    }
}
