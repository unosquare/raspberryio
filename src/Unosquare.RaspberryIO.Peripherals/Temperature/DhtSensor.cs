namespace Unosquare.RaspberryIO.Peripherals
{
    using Abstractions;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using Unosquare.RaspberryIO.Abstractions.Native;
    using Unosquare.Swan;

    /// <summary>
    /// Base class for DHT family digital relative humidity and temperature sensors.
    /// </summary>
    /// <seealso cref="IDisposable" />
    public abstract class DhtSensor : IDisposable
    {
        private const long BitPulseMidMicroseconds = 60; // (26 ... 28)µs for false; (29 ... 70)µs for true

        private static readonly int[] AllowedPinNumbers = { 7, 11, 12, 13, 15, 16, 18, 22, 29, 31, 32, 33, 35, 36, 37, 38, 40 };
        private static readonly TimeSpan ReadInterval = TimeSpan.FromSeconds(2);

        private readonly IGpioPin DataPin;
        private readonly Timer ReadTimer;
        private int _period;

        /// <summary>
        /// Initializes static members of the <see cref="DhtSensor"/> class.
        /// </summary>
        static DhtSensor()
        {
            AllowedPins = new ReadOnlyCollection<IGpioPin>(
                Pi.Gpio
                    .Where(g => g.Header == GpioHeader.P1 &&
                                AllowedPinNumbers.Contains(g.PhysicalPinNumber))
                    .ToArray());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dht11" /> class.
        /// </summary>
        /// <param name="dataPin">The data pin. Must be a GPIO-only pin on the P1 Header of the Pi.</param>
        /// <exception cref="ArgumentException">dataPin When it is invalid.</exception>
        protected DhtSensor(IGpioPin dataPin)
        {
            if (AllowedPins.Contains(dataPin) == false)
                throw new ArgumentException($"{nameof(dataPin)}, {dataPin} is not available to service this driver.");

            DataPin = dataPin;
            ReadTimer = new Timer(PerformContinuousReads, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Occurs when data from the sensor becomes available
        /// </summary>
        public event EventHandler<DhtReadEventArgs> OnDataAvailable;

        /// <summary>
        /// Gets a value indicating whether the sensor is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets the pulldown microseconds for start communication.
        /// </summary>
        protected uint PullDownMicroseconds { get; set; } = 20000;

        /// <summary>
        /// Gets a collection of pins that are allowed to run this sensor.
        /// </summary>
        protected static ReadOnlyCollection<IGpioPin> AllowedPins { get; }

        /// <summary>
        /// Creates a specific DHT sensor class.
        /// </summary>
        /// <param name="type">The type of DHT sensor to create.</param>
        /// <param name="dataPin">The data pin. Must be a GPIO-only pin on the P1 Header of the Pi.</param>
        /// <returns>The instance of the specific DHT sensor.</returns>
        public static DhtSensor Create(DhtType type, IGpioPin dataPin)
        {
            switch (type)
            {
                case DhtType.Dht12:
                    return new Dht12(dataPin);
                case DhtType.Dht21:
                case DhtType.AM2301:
                    return new Dht21(dataPin);
                case DhtType.Dht22:
                case DhtType.AM2302:
                    return new Dht22(dataPin);
                default:
                    return new Dht11(dataPin);
            }
        }

        /// <summary>
        /// Starts sensor reading at the min time interval which is 2 seconds.
        /// </summary>
        public void Start() =>
            Start(2);

        /// <summary>
        /// Starts sensor reading.
        /// </summary>
        /// <param name="period">The time interval between reading invocations, in seconds. The min time interval must be 2 seconds.</param>
        /// <exception cref="InvalidOperationException">Period cannot be less than 2 second.</exception>
        public void Start(int period)
        {
            if (period < 2)
                throw new InvalidOperationException("Period cannot be less than 2 second.");

            _period = period * 1000;
            IsRunning = true;
            ReadTimer.Change(0, Timeout.Infinite);
        }

        /// <summary>
        /// Stops sensor reading.
        /// </summary>
        public void Stop() =>
            StopContinuousReads();

        /// <summary>
        /// Decodes the temperature.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A value representing the current temperature.</returns>
        protected abstract double DecodeTemperature(byte[] data);

        /// <summary>
        /// Decodes the humidity.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A value representing the current humidity.</returns>
        protected abstract double DecodeHumidity(byte[] data);

        /// <summary>
        /// Performs the continuous reads of the sensor.
        /// This method represents the body of the worker.
        /// </summary>
        private void PerformContinuousReads(object state)
        {
            if (!IsRunning)
                return;

            try
            {
                // Acquire measure
                var sensorData = RetrieveSensorData();

                if (IsRunning)
                {
                    OnDataAvailable?.Invoke(this, sensorData);
                    ReadTimer.Change(_period, Timeout.Infinite);
                }
            }
            catch
            {
                // swallow
            }
        }

        /// <summary>
        /// Retrieves the sensor data.
        /// </summary>
        /// <returns>The event arguments that will be read from the sensor.</returns>
        private DhtReadEventArgs RetrieveSensorData()
        {
            // Prepare buffer to store measure and checksum
            var data = new byte[5];

            // Start to comunicate with sensor
            // Inform sensor that must finish last execution and put it's state in idle
            DataPin.PinMode = GpioPinDriveMode.Output;

            // Send request to trasmission from board to sensor
            DataPin.Write(GpioPinValue.Low);
            Pi.Timing.SleepMicroseconds(PullDownMicroseconds);
            DataPin.Write(GpioPinValue.High);

            // Wait for sensor response
            DataPin.PinMode = GpioPinDriveMode.Input;

            try
            {
                // Read acknowledgement from sensor
                if (!DataPin.WaitForValue(GpioPinValue.Low, 50))
                    throw new TimeoutException();

                if (!DataPin.WaitForValue(GpioPinValue.High, 50))
                    throw new TimeoutException();

                // Begins data transmission
                if (!DataPin.WaitForValue(GpioPinValue.Low, 50))
                    throw new TimeoutException();

                // Read 40 bits to acquire:
                //   16 bit -> Humidity
                //   16 bit -> Temperature
                //   8 bit -> Checksum

                var stopwatch = new HighResolutionTimer();
                for (var i = 0; i < 40; i++)
                {
                    stopwatch.Reset();
                    if (!DataPin.WaitForValue(GpioPinValue.High, 50))
                        throw new TimeoutException();

                    stopwatch.Start();
                    if (!DataPin.WaitForValue(GpioPinValue.Low, 50))
                        throw new TimeoutException();

                    stopwatch.Stop();

                    data[i / 8] <<= 1;

                    // Check if signal is 1 or 0
                    if (stopwatch.ElapsedMicroseconds > BitPulseMidMicroseconds)
                        data[i / 8] |= 1;
                }

                // End transmission
                if (!DataPin.WaitForValue(GpioPinValue.High, 50))
                    throw new TimeoutException();

                // Compute the checksum
                return IsDataValid(data) ?
                        new DhtReadEventArgs(
                            humidityPercentage: DecodeHumidity(data),
                            temperatureCelsius: DecodeTemperature(data)) :
                        DhtReadEventArgs.CreateInvalidReading();
            }
            catch
            {
                return DhtReadEventArgs.CreateInvalidReading();
            }
        }

        private bool IsDataValid(byte[] data) =>
            ((data[0] + data[1] + data[2] + data[3]) & 0xff) == data[4];

        /// <summary>
        /// Aborts the read thread.
        /// </summary>
        private void StopContinuousReads()
        {
            ReadTimer.Change(Timeout.Infinite, Timeout.Infinite);
            IsRunning = false;
        }

#region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Avoid calling this multiple times
                    if (IsRunning)
                        StopContinuousReads();

                    ReadTimer.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose() =>
            Dispose(true);

#endregion

    }
}
