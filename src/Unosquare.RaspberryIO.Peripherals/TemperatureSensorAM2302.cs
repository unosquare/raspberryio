namespace Unosquare.RaspberryIO.Peripherals
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using Unosquare.RaspberryIO.Abstractions;
    using Unosquare.RaspberryIO.Abstractions.Native;

    /// <summary>
    /// Provides logic to read from the AM2302 sensor, also known as the DHT22 sensor.
    /// This is an inexpensive sensor that reads temperature and humidity constantly.
    /// </summary>
    public class TemperatureSensorAM2302 : IDisposable
    {
        private static readonly int[] AllowedPinNumbers = new[] { 7, 11, 12, 13, 15, 16, 18, 22, 29, 31, 32, 33, 35, 36, 37, 38, 40 };
        private static readonly TimeSpan ReadInterval = TimeSpan.FromSeconds(2);
        private static readonly long BitPulseMidMicroseconds = 50; // (26 ... 50)µs for false; (51 ... 76)µs for true

        private readonly IGpioPin DataPin;
        private readonly Thread ReadWorker;

        /// <summary>
        /// Initializes static members of the <see cref="TemperatureSensorAM2302"/> class.
        /// </summary>
        static TemperatureSensorAM2302()
        {
            AllowedPins = new ReadOnlyCollection<IGpioPin>(
                Pi.Gpio
                    .Where(g => g.Header == GpioHeader.P1 &&
                                AllowedPinNumbers.Contains(g.PhysicalPinNumber))
                    .ToArray());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemperatureSensorAM2302" /> class.
        /// </summary>
        /// <param name="dataPin">The data pin. Must be a GPIO-only pin on the P1 Header of the Pi.</param>
        /// <exception cref="ArgumentException">dataPin When it is invalid.</exception>
        public TemperatureSensorAM2302(IGpioPin dataPin)
        {
            if (AllowedPins.Contains(dataPin) == false)
                throw new ArgumentException($"{nameof(dataPin)}, {dataPin} is not available to service this driver.");

            DataPin = dataPin;
            ReadWorker = new Thread(PerformContinuousReads);
        }

        /// <summary>
        /// Occurs when data from the sensor becomes available
        /// </summary>
        public event EventHandler<AM2302DataReadEventArgs> OnDataAvailable;

        /// <summary>
        /// Gets a collection of pins that are allowed to run this sensor.
        /// </summary>
        public static ReadOnlyCollection<IGpioPin> AllowedPins { get; }

        /// <summary>
        /// Gets a value indicating whether the sensor is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Starts the listener.
        /// </summary>
        public void Start()
        {
            IsRunning = true;
            ReadWorker.Start();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Avoid calling this multiple times
            if (IsRunning == false)
                return;

            // Abort
            StopContinuousReads();
        }

        /// <summary>
        /// Performs the continuous reads of the sensor.
        /// This method represents the body of the worker.
        /// </summary>
        private void PerformContinuousReads()
        {
            var stopwatch = new HighResolutionTimer();
            var lastElapsedTime = TimeSpan.FromSeconds(0);
            while (IsRunning)
            {
                try
                {
                    // Start to comunicate with sensor
                    // Inform sensor that must finish last execution and put it's state in idle
                    DataPin.PinMode = GpioPinDriveMode.Output;

                    // Waiting for sensor init
                    DataPin.Write(GpioPinValue.High);
                    if (lastElapsedTime < ReadInterval)
                        Thread.Sleep(ReadInterval - lastElapsedTime);

                    // Start to counter measure time
                    stopwatch.Start();

                    // Send request to trasmission from board to sensor
                    DataPin.Write(GpioPinValue.Low);
                    Pi.Timing.SleepMicroseconds(1000);
                    DataPin.Write(GpioPinValue.High);
                    Pi.Timing.SleepMicroseconds(20);
                    DataPin.Write(GpioPinValue.Low);

                    // Acquire measure
                    var sensorData = RetrieveSensorData();
                    OnDataAvailable?.Invoke(this, sensorData);
                }
                catch
                {
                    // swallow
                }

                lastElapsedTime = stopwatch.Elapsed;
                if (stopwatch.IsRunning)
                    stopwatch.Stop();

                stopwatch.Reset();
            }
        }

        /// <summary>
        /// Retrieves the sensor data.
        /// </summary>
        /// <returns>The event arguments that will be read from the sensor.</returns>
        private AM2302DataReadEventArgs RetrieveSensorData()
        {
            // Prepare buffer to store measure and checksum
            var data = new byte[5];
            for (var i = 0; i < 5; i++)
                data[i] = 0;

            // Wait for sensor response
            DataPin.PinMode = GpioPinDriveMode.Input;

            // Read acknowledgement from sensor
            DataPin.WaitForValue(GpioPinValue.High, 100);
            DataPin.WaitForValue(GpioPinValue.Low, 100);

            // Read 40 bits to acquire:
            //   16 bit -> Humidity
            //   16 bit -> Temperature
            //   8 bit -> Checksum
            var remainingBitCount = 7;
            var currentByteIndex = 0;

            var stopwatch = new HighResolutionTimer();
            for (var i = 0; i < 40; i++)
            {
                stopwatch.Reset();
                DataPin.WaitForValue(GpioPinValue.High, 100);

                stopwatch.Start();
                DataPin.WaitForValue(GpioPinValue.Low, 100);

                stopwatch.Stop();

                // Check if signal is 1 or 0
                if (stopwatch.ElapsedMicroseconds > BitPulseMidMicroseconds)
                    data[currentByteIndex] |= (byte)(1 << remainingBitCount);

                if (remainingBitCount == 0)
                {
                    currentByteIndex++;

                    // restart the remaining count
                    // for the next incoming byte
                    remainingBitCount = 7;
                }
                else
                {
                    remainingBitCount--;
                }
            }

            // Compute the checksum
            var checkSum = data[0] + data[1] + data[2] + data[3];
            if ((checkSum & 0xff) != data[4])
                return null;

            var sign = 1;

            // Check negative temperature
            if ((data[2] & 0x80) != 0)
            {
                data[2] = (byte)(data[2] & 0x7F);
                sign = -1;
            }

            return new AM2302DataReadEventArgs(
                temperatureCelsius: (sign * ((data[2] << 8) + data[3])) / 10m,
                humidityPercentage: ((data[0] << 8) + data[1]) / 10m);
        }

        /// <summary>
        /// Aborts the read thread.
        /// </summary>
        private void StopContinuousReads() =>
            IsRunning = false;

        /// <summary>
        /// Represents the sensor data that was read.
        /// </summary>
        public sealed class AM2302DataReadEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AM2302DataReadEventArgs"/> class.
            /// </summary>
            /// <param name="temperatureCelsius">The temperature celsius.</param>
            /// <param name="humidityPercentage">The humidity percentage.</param>
            internal AM2302DataReadEventArgs(decimal temperatureCelsius, decimal humidityPercentage)
            {
                TemperatureCelsius = temperatureCelsius;
                HumidityPercentage = humidityPercentage;
            }

            /// <summary>
            /// Prevents a default instance of the <see cref="AM2302DataReadEventArgs"/> class from being created.
            /// </summary>
            private AM2302DataReadEventArgs()
            {
                // placeholder
            }

            /// <summary>
            /// Gets the temperature celsius.
            /// </summary>
            public decimal TemperatureCelsius { get; }

            /// <summary>
            /// Gets the humidity percentage.
            /// </summary>
            public decimal HumidityPercentage { get; }
        }
    }
}
