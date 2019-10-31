namespace Unosquare.RaspberryIO.Peripherals
{
    using Abstractions;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Ultrasonic sensor HC-SR04.
    /// </summary>
    /// <seealso cref="IDisposable" />
    public class UltrasonicHcsr04 : IDisposable
    {
        /// <summary>
        /// This value is reported when no obstacle is detected.
        /// </summary>
        public const int NoObstacleDistance = -1;

        private const long NoObstaclePulseMicroseconds = 32000;
        private static readonly int[] AllowedPinNumbers = { 7, 11, 12, 13, 15, 16, 18, 22, 29, 31, 32, 33, 35, 36, 37, 38, 40 };

        private bool _disposedValue;
        private IGpioPin? _triggerPin;
        private IGpioPin? _echoPin;
        private Thread _readWorker;
        private Swan.Diagnostics.HighResolutionTimer? _measurementTimer;

        static UltrasonicHcsr04()
        {
            AllowedPins = new ReadOnlyCollection<IGpioPin>(
                Pi.Gpio
                    .Where(g => g.Header == GpioHeader.P1 &&
                                AllowedPinNumbers.Contains(g.PhysicalPinNumber))
                    .ToArray());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UltrasonicHcsr04"/> class.
        /// </summary>
        /// <param name="triggerPin">The trigger pin.</param>
        /// <param name="echoPin">The echo pin.</param>
        /// <exception cref="ArgumentException">When it is provided invalid trigger or echo pins.</exception>
        public UltrasonicHcsr04(IGpioPin triggerPin, IGpioPin echoPin)
        {
            if (AllowedPins.Contains(triggerPin) == false)
                throw new ArgumentException($"{nameof(triggerPin)}, {triggerPin} is not available to service this driver.");

            if (AllowedPins.Contains(echoPin) == false)
                throw new ArgumentException($"{nameof(echoPin)}, {echoPin} is not available to service this driver.");

            _triggerPin = triggerPin;
            _echoPin = echoPin;
            _readWorker = new Thread(PerformContinuousReads);
        }

        /// <summary>
        /// Occurs when data from sensor is available.
        /// </summary>
        public event EventHandler<UltrasonicReadEventArgs> OnDataAvailable;

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        private static ReadOnlyCollection<IGpioPin> AllowedPins { get; }

        /// <summary>
        /// Starts sensor reading.
        /// </summary>
        public void Start()
        {
            _triggerPin.PinMode = GpioPinDriveMode.Output;
            _echoPin.PinMode = GpioPinDriveMode.Input;

            _measurementTimer = new Swan.Diagnostics.HighResolutionTimer();
            IsRunning = true;
            _readWorker.Start();
        }

        /// <summary>
        /// Stops sensor reading.
        /// </summary>
        public void Stop() =>
            IsRunning = false;
        
        /// <inheritdoc />
        public void Dispose() =>
            Stop();
        
        private void PerformContinuousReads(object state)
        {
            while (IsRunning)
            {
                // Acquire measure
                var sensorData = RetrieveSensorData();

                if (!IsRunning) continue;
                OnDataAvailable?.Invoke(this, sensorData);
                Thread.Sleep(200);
            }

            Dispose(true);
        }

        private UltrasonicReadEventArgs RetrieveSensorData()
        {
            try
            {
                // Send trigger pulse
                _triggerPin.Write(GpioPinValue.Low);
                Pi.Timing.SleepMicroseconds(2);
                _triggerPin.Write(GpioPinValue.High);
                Pi.Timing.SleepMicroseconds(12);
                _triggerPin.Write(GpioPinValue.Low);

                if (!_echoPin.WaitForValue(GpioPinValue.High, 50))
                    throw new TimeoutException();

                _measurementTimer.Start();
                if (!_echoPin.WaitForValue(GpioPinValue.Low, 50))
                    throw new TimeoutException();

                _measurementTimer.Stop();
                var elapsedTime = _measurementTimer.ElapsedMicroseconds;
                _measurementTimer.Reset();

                var distance = elapsedTime / 58.0;
                if (elapsedTime > NoObstaclePulseMicroseconds)
                    distance = NoObstacleDistance;

                return new UltrasonicReadEventArgs(distance);
            }
            catch
            {
                return UltrasonicReadEventArgs.CreateInvalidReading();
            }
        }
        
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                _triggerPin = null;
                _echoPin = null;
                _readWorker = null;
            }

            _disposedValue = true;
        }
    }
}
