namespace Unosquare.RaspberryIO.Peripherals
{
    using System;
    using System.Threading;
    using Unosquare.RaspberryIO;
    using Unosquare.RaspberryIO.Abstractions;

#pragma warning disable SA1602 // Enumeration items should be documented
    /// <summary>
    /// Abstract base class for both ADC varients, new the specific varient to ensure it's configured correctly at construction.
    /// </summary>
    public abstract class ADS1x15
    {
        /// <summary>
        /// Register a device on the bus.
        /// </summary>
        private II2CDevice device;

        private byte i2cAddress;
        private byte conversionDelay;
        private byte bitShift;

        /// <summary>
        /// Initializes a new instance of the <see cref="ADS1x15"/> class.
        /// </summary>
        /// <param name="delay">amount of time to delay between write/read.</param>
        /// <param name="shift">bits to shift for sign correction.</param>
        /// <param name="address">I2C Address.</param>
        protected ADS1x15(byte delay, byte shift, byte address = ADS1x15ADDRESS)
        {
            device = Pi.I2C.AddDevice(i2cAddress = address);
            conversionDelay = delay;
            bitShift = shift;

            Console.WriteLine($"did: {device.DeviceId} desc:{device.FileDescriptor}");
            Gain = AdsGaint.GAINTWOTHIRDS; /* +/- 6.144V range (limited to VDD +0.3V max!) */
        }

#pragma warning disable CS1591 // Missing XML Documentation
        /// <summary>
        /// Gain of internal converter.
        /// </summary>
        public enum AdsGaint
        {
            GAINTWOTHIRDS = ADS1015REGCONFIGPGA6144V,
            GAINONE = ADS1015REGCONFIGPGA4096V,
            GAINTWO = ADS1015REGCONFIGPGA2048V,
            GAINFOUR = ADS1015REGCONFIGPGA1024V,
            GAINEIGHT = ADS1015REGCONFIGPGA0512V,
            GAINSIXTEEN = ADS1015REGCONFIGPGA0256V,
        }
#pragma warning restore SA1602 // Enumeration items should be documented

        /// <summary>
        /// Internal gain of hardware converter.
        /// </summary>
        public AdsGaint Gain { get; set; }

        /// <summary>
        /// Gets a single-ended ADC reading from the specified channel.
        /// </summary>
        /// <param name="channel">the channel to interogate.</param>
        /// <returns>unsigned absolute voltage of the ADC channel.</returns>
        public short ReadChannel(byte channel)
        {
            var config = SingleAndDiffConfigBase
                        | ((channel * 0x1000) + ADS1015REGCONFIGMUXSINGLE0)
                        | ADS1015REGCONFIGOSSINGLE
                        ;

            return (channel > 3) ? (short)-1 : ConfigureThenReadADC(config);
        }

        /// <summary>
        /// Read the voltage difference (+/-) between ADC0 and ADC1.
        /// </summary>
        /// <returns>signed voltage difference.</returns>
        public short ReadDifferential01()
        {
            return SignBitCleanup(ConfigureThenReadADC(SingleAndDiffConfigBase | ADS1015REGCONFIGMUXDIFF01));
        }

        /// <summary>
        /// Read the voltage difference (+/-) between ADC2 and ADC3.
        /// </summary>
        /// <returns>signed voltage difference.</returns>
        public short ReadDifferential23()
        {
            return SignBitCleanup(ConfigureThenReadADC(SingleAndDiffConfigBase | ADS1015REGCONFIGMUXDIFF23));
        }

        /// <summary>
        ///  Start ADC in comparitor mode.
        /// </summary>
        /// <param name="channel">the channel to trigger on.</param>
        /// <param name="threshold">the triggering value.</param>
        public void StartComparitor(byte channel, short threshold)
        {
            var config = ComparitorConfigBase
                        | ((channel * 0x1000) + ADS1015REGCONFIGMUXSINGLE0)
                        ;

            // Set the high threshold register
            // Shift 12-bit results left 4 bits for the ADS1015
            WriteRegister(ADS1015REGPOINTERHITHRESH, (ushort)(threshold << bitShift));

            // Write config register to the ADC
            WriteRegister(ADS1015REGPOINTERCONFIG, (ushort)config);
        }
        
        /// <summary>
        /// Fetch results from a thresholded comparitor.
        /// </summary>
        /// <returns>unsigned result.</returns>
        public ushort PollComparitorResult()
        {
            return (ushort)RawReadADC();
        }

        /// <summary>
        /// Fetch results from a thresholded comparitor.
        /// </summary>
        /// <returns>signed result.</returns>
        public short PollComparitorResultSigned()
        {
            return SignBitCleanup(RawReadADC());
        }

        #region Private Members
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CA1823 // Remove unused private members
#pragma warning disable SA1201 // A field should not follow a method

        /// <summary>
        /// I2C ADDRESS/BITS.
        /// </summary>
        protected const byte ADS1x15ADDRESS = 0x48;    // 1001 000 (ADDR = GND)

        /// <summary>
        /// CONVERSION DELAY (in mS).
        /// </summary>
        protected const int ADS1015CONVERSIONDELAY = 1;
        protected const int ADS1115CONVERSIONDELAY = 8;

        // POINTER REGISTER
        private const byte ADS1015REGPOINTERMASK = 0x03;
        private const byte ADS1015REGPOINTERCONVERT = 0x00;
        private const byte ADS1015REGPOINTERCONFIG = 0x01;
        private const byte ADS1015REGPOINTERLOWTHRESH = 0x02;
        private const byte ADS1015REGPOINTERHITHRESH = 0x03;

        // CONFIG REGISTER
        private const ushort ADS1015REGCONFIGOSMASK = 0x8000;
        private const ushort ADS1015REGCONFIGOSSINGLE = 0x8000;  // Write: Set to start a single-conversion
        private const ushort ADS1015REGCONFIGOSBUSY = 0x0000;  // Read: Bit = 0 when conversion is in progress
        private const ushort ADS1015REGCONFIGOSNOTBUSY = 0x8000;  // Read: Bit = 1 when device is not performing a conversion

        private const ushort ADS1015REGCONFIGMUXMASK = 0x7000;
        private const ushort ADS1015REGCONFIGMUXDIFF01 = 0x0000;  // Differential P = AIN0, N = AIN1 (default)
        private const ushort ADS1015REGCONFIGMUXDIFF03 = 0x1000;  // Differential P = AIN0, N = AIN3
        private const ushort ADS1015REGCONFIGMUXDIFF13 = 0x2000;  // Differential P = AIN1, N = AIN3
        private const ushort ADS1015REGCONFIGMUXDIFF23 = 0x3000;  // Differential P = AIN2, N = AIN3
        private const ushort ADS1015REGCONFIGMUXSINGLE0 = 0x4000;  // Single-ended AIN0
        private const ushort ADS1015REGCONFIGMUXSINGLE1 = 0x5000;  // Single-ended AIN1
        private const ushort ADS1015REGCONFIGMUXSINGLE2 = 0x6000;  // Single-ended AIN2
        private const ushort ADS1015REGCONFIGMUXSINGLE3 = 0x7000;  // Single-ended AIN3

        private const ushort ADS1015REGCONFIGPGAMASK = 0x0E00;
        private const ushort ADS1015REGCONFIGPGA6144V = 0x0000;  // +/-6.144V range = Gain 2/3
        private const ushort ADS1015REGCONFIGPGA4096V = 0x0200;  // +/-4.096V range = Gain 1
        private const ushort ADS1015REGCONFIGPGA2048V = 0x0400;  // +/-2.048V range = Gain 2 (default)
        private const ushort ADS1015REGCONFIGPGA1024V = 0x0600;  // +/-1.024V range = Gain 4
        private const ushort ADS1015REGCONFIGPGA0512V = 0x0800;  // +/-0.512V range = Gain 8
        private const ushort ADS1015REGCONFIGPGA0256V = 0x0A00;  // +/-0.256V range = Gain 16

        private const ushort ADS1015REGCONFIGMODEMASK = 0x0100;
        private const ushort ADS1015REGCONFIGMODECONTIN = 0x0000;  // Continuous conversion mode
        private const ushort ADS1015REGCONFIGMODESINGLE = 0x0100;  // Power-down single-shot mode (default)

        private const ushort ADS1015REGCONFIGDRMASK = 0x00E0;
        private const ushort ADS1015REGCONFIGDR128SPS = 0x0000;  // 128 samples per second
        private const ushort ADS1015REGCONFIGDR250SPS = 0x0020;  // 250 samples per second
        private const ushort ADS1015REGCONFIGDR490SPS = 0x0040;  // 490 samples per second
        private const ushort ADS1015REGCONFIGDR920SPS = 0x0060;  // 920 samples per second
        private const ushort ADS1015REGCONFIGDR1600SPS = 0x0080;  // 1600 samples per second (default)
        private const ushort ADS1015REGCONFIGDR2400SPS = 0x00A0;  // 2400 samples per second
        private const ushort ADS1015REGCONFIGDR3300SPS = 0x00C0;  // 3300 samples per second

        private const ushort ADS1015REGCONFIGCMODEMASK = 0x0010;
        private const ushort ADS1015REGCONFIGCMODETRAD = 0x0000;  // Traditional comparator with hysteresis (default)
        private const ushort ADS1015REGCONFIGCMODEWINDOW = 0x0010;  // Window comparator

        private const ushort ADS1015REGCONFIGCPOLMASK = 0x0008;
        private const ushort ADS1015REGCONFIGCPOLACTVLOW = 0x0000;  // ALERT/RDY pin is low when active (default)
        private const ushort ADS1015REGCONFIGCPOLACTVHI = 0x0008;  // ALERT/RDY pin is high when active

        private const ushort ADS1015REGCONFIGCLATMASK = 0x0004;  // Determines if ALERT/RDY pin latches once asserted
        private const ushort ADS1015REGCONFIGCLATNONLAT = 0x0000;  // Non-latching comparator (default)
        private const ushort ADS1015REGCONFIGCLATLATCH = 0x0004;  // Latching comparator

        private const ushort ADS1015REGCONFIGCQUEMASK = 0x0003;
        private const ushort ADS1015REGCONFIGCQUE1CONV = 0x0000;  // Assert ALERT/RDY after one conversions
        private const ushort ADS1015REGCONFIGCQUE2CONV = 0x0001;  // Assert ALERT/RDY after two conversions
        private const ushort ADS1015REGCONFIGCQUE4CONV = 0x0002;  // Assert ALERT/RDY after four conversions
        private const ushort ADS1015REGCONFIGCQUENONE = 0x0003;  // Disable the comparator and put ALERT/RDY in high state (default)

        private const ushort SingleAndDiffConfigBase = ADS1015REGCONFIGCQUENONE // Disable the comparator (default val)
                                            | ADS1015REGCONFIGCLATNONLAT // Non-latching (default val)
                                            | ADS1015REGCONFIGCPOLACTVLOW // Alert/Rdy active low   (default val)
                                            | ADS1015REGCONFIGCMODETRAD // Traditional comparator (default val)
                                            | ADS1015REGCONFIGDR1600SPS // 1600 samples per second (default)
                                            | ADS1015REGCONFIGMODESINGLE // Single-shot mode (default)
                                            ;

        private const ushort ComparitorConfigBase = ADS1015REGCONFIGCQUE1CONV // Comparator enabled and asserts on 1 match
                                        | ADS1015REGCONFIGCLATLATCH // Latching mode
                                        | ADS1015REGCONFIGCPOLACTVLOW // Alert/Rdy active low   (default val)
                                        | ADS1015REGCONFIGCMODETRAD // Traditional comparator (default val)
                                        | ADS1015REGCONFIGDR1600SPS // 1600 samples per second (default)
                                        | ADS1015REGCONFIGMODECONTIN // Continuous conversion mode
                                        | ADS1015REGCONFIGMODECONTIN // Continuous conversion mode
                                        ;

        private short ConfigureThenReadADC(int config)
        {
            // Write config register to the ADC
            WriteRegister(ADS1015REGPOINTERCONFIG, (ushort)((int)Gain | config));

            return RawReadADC();
        }

        private short RawReadADC()
        {
            // Wait for the conversion to complete
            Thread.Sleep(conversionDelay);

            // Read the conversion results
            // Shift 12-bit results right 4 bits for the ADS1015
            return (short)(SwapWord(device.ReadAddressWord(ADS1015REGPOINTERCONVERT)) >> bitShift);
        }

        /// <summary>
        /// The bit shifting wrecks the sign bit, need to get it back in the right spot.
        /// </summary>
        /// <param name="value">the value to cleanup.</param>
        /// <returns>sign-corrected value.</returns>
        private short SignBitCleanup(int value)
        {
            return (short)((bitShift != 0 && value > 0x07FF) ? value | 0xF000 : value);
        }

        /// <summary>
        /// Write to the I2C register, ADS1x15 is byte swapped.
        /// </summary>
        /// <param name="registery">register address to write to</param>
        /// <param name="value">value to set</param>
        private void WriteRegister(byte registery, ushort value)
        {
            device.WriteAddressWord(registery, SwapWord(value));
        }

        /// <summary>
        /// Swap the low and high bytes.
        /// </summary>
        /// <param name="value">value to swap</param>
        /// <returns>16-bit unsigned value with high and low bytes swapped</returns>
        private ushort SwapWord(ushort value)
        {
            return (ushort)((value >> 8) | (value << 8));
        }

#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore CA1823
#pragma warning restore SA1201 // A field should not follow a method

        #endregion
    }

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

#pragma warning restore CS1591 // Missing XML Documentation
}
