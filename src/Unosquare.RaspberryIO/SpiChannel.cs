namespace Unosquare.RaspberryIO
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides access to using the SPI buses on the GPIO.
    /// SPI is a bus that works like a ring shift register 
    /// The number of bytes pushed is equal to the number of bytes received.
    /// </summary>
    public sealed class SpiChannel
    {
        private static readonly Dictionary<SpiChannelNumber, SpiChannel> Buses = new Dictionary<SpiChannelNumber, SpiChannel>();

        public const int MinFrequency = 500000;
        public const int MaxFrequency = 32000000;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpiChannel"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="frequency">The frequency.</param>
        /// <exception cref="System.SystemException"></exception>
        private SpiChannel(SpiChannelNumber channel, int frequency)
        {
            if (frequency > MaxFrequency) frequency = MaxFrequency;
            if (frequency < MinFrequency) frequency = MinFrequency;

            var busResult = Interop.wiringPiSPISetup((int)channel, frequency);
            Channel = channel;
            Frequency = frequency;

            if (busResult != 0)
            {
                throw new SystemException($"Could not register SPI bus on channel {channel} at {frequency} Hz.");
            }
        }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        public SpiChannelNumber Channel { get; }

        /// <summary>
        /// Gets the frequency.
        /// </summary>
        public int Frequency { get; private set; }

        /// <summary>
        /// Retrieves the spi bus. If the bus channel is not registered it sets it up automatically.
        /// If it had been previously registered, then the bus is simply returned.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="frequency">The frequency.</param>
        /// <returns></returns>
        internal static SpiChannel Retrieve(SpiChannelNumber channel, int frequency)
        {
            lock (Pi.SyncLock)
            {
                if (Buses.ContainsKey(channel))
                    return Buses[channel];
                var newBus = new SpiChannel(channel, frequency);
                Buses[channel] = newBus;
                return newBus;
            }
        }

        /// <summary>
        /// Sends data and simultaneously receives the data in the return buffer
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns></returns>
        public byte[] SendReceive(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                return null;

            lock (Pi.SyncLock)
            {
                var spiBuffer = new byte[buffer.Length];
                Array.Copy(buffer, spiBuffer, buffer.Length);

                Interop.wiringPiSPIDataRW((int)Channel, spiBuffer, spiBuffer.Length);
                return spiBuffer;
            }
        }
    }
}
