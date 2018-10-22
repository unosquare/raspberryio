namespace Unosquare.WiringPI
{
    using Native;
    using RaspberryIO.Abstractions;
    using Swan;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Unosquare.RaspberryIO.Abstractions.Native;

    /// <summary>
    /// Provides access to using the SPI buses on the GPIO.
    /// SPI is a bus that works like a ring shift register
    /// The number of bytes pushed is equal to the number of bytes received.
    /// </summary>
    public sealed class SpiChannel : ISpiChannel
    {
        /// <summary>
        /// The minimum frequency of an SPI Channel.
        /// </summary>
        public const int MinFrequency = 500000;

        /// <summary>
        /// The maximum frequency of an SPI channel.
        /// </summary>
        public const int MaxFrequency = 32000000;

        private static readonly object SyncRoot = new object();
        private static readonly Dictionary<SpiChannelNumber, SpiChannel> Buses = new Dictionary<SpiChannelNumber, SpiChannel>();
        private readonly object _syncLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="SpiChannel"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="frequency">The frequency.</param>
        private SpiChannel(SpiChannelNumber channel, int frequency)
        {
            lock (SyncRoot)
            {
                Frequency = frequency.Clamp(MinFrequency, MaxFrequency);
                Channel = (int)channel;
                FileDescriptor = WiringPi.WiringPiSPISetup((int)channel, Frequency);

                if (FileDescriptor < 0)
                {
                    HardwareException.Throw(nameof(SpiChannel), channel.ToString());
                }
            }
        }

        /// <inheritdoc />
        public int FileDescriptor { get; }

        /// <inheritdoc />
        public int Channel { get; }

        /// <inheritdoc />
        public int Frequency { get; }

        /// <inheritdoc />
        public byte[] SendReceive(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                return null;

            lock (_syncLock)
            {
                var spiBuffer = new byte[buffer.Length];
                Array.Copy(buffer, spiBuffer, buffer.Length);

                var result = WiringPi.WiringPiSPIDataRW(Channel, spiBuffer, spiBuffer.Length);
                if (result < 0) HardwareException.Throw(nameof(SpiChannel), nameof(SendReceive));

                return spiBuffer;
            }
        }

        /// <summary>
        /// Sends data and simultaneously receives the data in the return buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>
        /// The read bytes from the ring-style bus.
        /// </returns>
        public Task<byte[]> SendReceiveAsync(byte[] buffer) => Task.Run(() => SendReceive(buffer));

        /// <inheritdoc />
        public void Write(byte[] buffer)
        {
            lock (_syncLock)
            {
                var result = Standard.Write(FileDescriptor, buffer, buffer.Length);

                if (result < 0)
                    HardwareException.Throw(nameof(SpiChannel), nameof(Write));
            }
        }

        /// <summary>
        /// Writes the specified buffer the the underlying FileDescriptor.
        /// Do not use this method if you expect data back.
        /// This method is efficient if used in a fire-and-forget scenario
        /// like sending data over to those long RGB LED strips.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>The awaitable task.</returns>
        public Task WriteAsync(byte[] buffer) => Task.Run(() => { Write(buffer); });

        /// <summary>
        /// Retrieves the spi bus. If the bus channel is not registered it sets it up automatically.
        /// If it had been previously registered, then the bus is simply returned.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="frequency">The frequency.</param>
        /// <returns>The usable SPI channel.</returns>
        internal static ISpiChannel Retrieve(SpiChannelNumber channel, int frequency)
        {
            lock (SyncRoot)
            {
                if (Buses.ContainsKey(channel))
                    return Buses[channel];

                var newBus = new SpiChannel(channel, frequency);
                Buses[channel] = newBus;
                return newBus;
            }
        }
    }
}
