namespace Unosquare.RaspberryIO.Peripherals
{
    using Gpio;
    using Native;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using Unosquare.Swan;

    /// <summary>
    /// Provides logic to read from the AM2302 sensor, also known as the DHT22 sensor.
    /// This is an inexpensive sensor that reads temperature and humidity constantly
    /// </summary>
    public class TemperatureSensorAM2302 : IDisposable
    {
        private static readonly int[] AllowedPinNumbers = new int[] { 7, 11, 12, 13, 15, 16, 18, 22, 29, 31, 32, 33, 35, 36, 37, 38, 40 };
        private static readonly TimeSpan ReadInterval = TimeSpan.FromSeconds(5);
        private readonly Timing _systemTiming;

        private readonly GpioPin DataPin;
        private readonly Thread ReadWorker;
        private bool _isRunning;

        /// <summary>
        /// Initializes static members of the <see cref="TemperatureSensorAM2302"/> class.
        /// </summary>
        static TemperatureSensorAM2302()
        {
            AllowedPins = new ReadOnlyCollection<GpioPin>(
                Pi.Gpio.HeaderP1
                    .Where(kvp => AllowedPinNumbers.Contains(kvp.Key))
                    .Select(kvp => kvp.Value)
                    .ToArray());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemperatureSensorAM2302" /> class.
        /// </summary>
        /// <param name="dataPin">The data pin. Must be a GPIO-only pin on the P1 Header of the Pi</param>
        /// <exception cref="ArgumentException">dataPin When it is invalid</exception>
        public TemperatureSensorAM2302(GpioPin dataPin)
        {
            if (AllowedPins.Contains(dataPin) == false)
                throw new ArgumentException($"{nameof(dataPin)}, {dataPin} is not available to service this driver.");

            _systemTiming = Timing.Instance;
            DataPin = dataPin;
            ReadWorker = new Thread(PerformContinuousReads)
            {
                IsBackground = true,
                Name = nameof(TemperatureSensorAM2302),
                Priority = ThreadPriority.AboveNormal
            };
        }

        /// <summary>
        /// Occurs when data from the sensor becomes available
        /// </summary>
        public event EventHandler<AM2302DataReadEventArgs> OnDataAvailable;

        /// <summary>
        /// Gets a collection of pins that are allowed to run this sensor.
        /// </summary>
        public static ReadOnlyCollection<GpioPin> AllowedPins { get; }

        /// <summary>
        /// Gets a value indicating whether the sensor is running.
        /// </summary>
        public bool IsRunning => _isRunning;

        /// <summary>
        /// Starts the listener.
        /// </summary>
        public void Start()
        {
            _isRunning = true;
            ReadWorker.Start();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Avoid calling this multiple times
            if (_isRunning == false)
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
            while (_isRunning)
            {
                try
                {
                    // Start to comunicate with sensor
                    // Inform sensor that must finish last execution and put it's state in idle
                    DataPin.PinMode = GpioPinDriveMode.Output;

                    // Send request to trasmission from board to sensor
                    DataPin.Write(GpioPinValue.Low);
                    _systemTiming.SleepMicroseconds(5000);
                    DataPin.Write(GpioPinValue.High);
                    _systemTiming.SleepMicroseconds(30);
                    DataPin.Write(GpioPinValue.Low);

                    // Acquire measure
                    var sensorData = RetrieveSensorData();
                    if (sensorData != null)
                        OnDataAvailable?.Invoke(this, sensorData);

                    DataPin.PinMode = GpioPinDriveMode.Output;
                    DataPin.Write(GpioPinValue.High);
                }
                catch (Exception ex)
                {
                    ex.Error(nameof(TemperatureSensorAM2302), ex.Message);
                }

                // Waiting for sensor init
                Thread.Sleep(ReadInterval);
            }
        }

        /// <summary>
        /// Retrieves the sensor data.
        /// </summary>
        /// <returns>The event arguments that will be read from the sensor</returns>
        private AM2302DataReadEventArgs RetrieveSensorData()
        {
            // Wait for sensor response
            DataPin.PinMode = GpioPinDriveMode.Input;
            DataPin.InputPullMode = GpioPinResistorPullMode.PullUp;

            var changeElapsed = new HighResolutionTimer();
            var lastValue = DataPin.Read();
            var currentValue = lastValue;
            var lastElapsed = 0L;
            var pulses = new List<Tuple<bool, long>>(128);

            changeElapsed.Start();
            while (true)
            {
                lastElapsed = changeElapsed.ElapsedMicroseconds;
                currentValue = DataPin.Read();

                if (lastElapsed >= 5000)
                    break;

                if (currentValue == lastValue)
                    continue;
                else
                    changeElapsed.Restart();

                var pulse = new Tuple<bool, long>(lastValue, lastElapsed);
                pulses.Add(pulse);
                lastValue = currentValue;
            }

            var startPulseIndex = -1;
            for (var pulseIndex = 0; pulseIndex < pulses.Count - 80 - 1; pulseIndex++)
            {
                var p0 = pulses[pulseIndex + 0];
                var p1 = pulses[pulseIndex + 1];

                if (p0.Item1 == true && p0.Item2.IsBetween(70, 90) && p1.Item1 == false && p1.Item2.IsBetween(40, 60))
                {
                    startPulseIndex = pulseIndex + 1;
                    break;
                }
            }

            if (startPulseIndex < 0 || pulses.Count - startPulseIndex < 80)
                return null;

            var dataBits = new BitArray(5 * 8); // 40 bit is 8 bytes
            var dataBitIndex = 0;
            $"Start Pulse Index: {startPulseIndex}".Info();
            for (var pulseIndex = startPulseIndex; pulseIndex < pulses.Count; pulseIndex += 2)
            {
                var p0 = pulses[pulseIndex + 0];
                var p1 = pulses[pulseIndex + 1];
                dataBits[dataBitIndex] = p1.Item2 < 32 ? false : true;
                $"{(dataBits[dataBitIndex] ? "1" : "0")} = {(p0.Item1 ? "H" : "L")}: {p0.Item2,4} | {(p1.Item1 ? "H" : "L")}: {p1.Item2,4}".Warn(nameof(TemperatureSensorAM2302));
                dataBitIndex++;
                if (dataBitIndex >= dataBits.Length)
                    break;
            }

            // Compute the checksum
            var data = new byte[dataBits.Length / 8];
            dataBits.CopyTo(data, 0);
            var checkSum = BitConverter.GetBytes(data[0] + data[1] + data[2] + data[3]);

            $"Checksum: {BitConverter.ToString(checkSum, 0)}; Data: {BitConverter.ToString(data)}".Warn(nameof(TemperatureSensorAM2302));
            if (checkSum[0] != data[4])
                $"BAD CHECKSUM: Expected {checkSum[0]:X}; Was: {data[4]:X}".Error(nameof(TemperatureSensorAM2302)); // return null;

            var sign = 0.1M;

            // Check negative temperature
            if ((data[2] & 0x80) != 0)
            {
                data[2] = (byte)(data[2] & 0x7F);
                sign *= -1;
            }

            return new AM2302DataReadEventArgs(
                temperatureCelsius: sign * (BitConverter.IsLittleEndian ?
                    BitConverter.ToUInt16(new byte[] { data[3], data[2] }, 0) :
                    BitConverter.ToUInt16(new byte[] { data[2], data[3] }, 0)),
                humidityPercentage: 0.1M * (BitConverter.IsLittleEndian ?
                    BitConverter.ToUInt16(new byte[] { data[1], data[0] }, 0) :
                    BitConverter.ToUInt16(new byte[] { data[0], data[1] }, 0)));
        }

        /// <summary>
        /// Aborts the read thread.
        /// </summary>
        private void StopContinuousReads()
        {
            _isRunning = false;
            ReadWorker.Abort();
        }

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
