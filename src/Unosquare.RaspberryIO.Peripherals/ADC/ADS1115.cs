using Unosquare.RaspberryIO.Abstractions;

namespace Unosquare.RaspberryIO.Peripherals
{
    /// <summary>
    /// ADS1115 Device.
    /// Also available pre-packaged on an experimental board as KY-053. 
    /// </summary>
    public class ADS1115 : ADS1x15
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ADS1115"/> class.
        /// </summary>
        /// <param name="address">The I2C Address for this device.</param>
        public ADS1115(II2CDevice device)
            : base(device, ADS1015CONVERSIONDELAY, 0)
        {
        }
    }
}
