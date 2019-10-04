namespace Unosquare.RaspberryIO.Peripherals
{
    /// <summary>
    /// ADS1115 Device.
    /// </summary>
    public class ADS1115 : ADS1x15
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ADS1115"/> class.
        /// </summary>
        /// <param name="address">The I2C Address for this device.</param>
        public ADS1115(byte address = ADS1x15ADDRESS)
            : base(ADS1115CONVERSIONDELAY, 0, address)
        {
        }
    }
}
