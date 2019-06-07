namespace Unosquare.RaspberryIO.Peripherals
{
    using Abstractions;
    using Abstractions.Native;
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

        private IGpioPin TriggerPin;
        private IGpioPin EchoPin;
        private Thread ReadWorker;
        private HighResolutionTimer measurementTimer;

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

            TriggerPin = triggerPin;
            EchoPin = echoPin;
            ReadWorker = new Thread(PerformContinuousReads);
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
            TriggerPin.PinMode = GpioPinDriveMode.Output;
            EchoPin.PinMode = GpioPinDriveMode.Input;

            measurementTimer = new HighResolutionTimer();
            IsRunning = true;
            ReadWorker.Start();
        }

        /// <summary>
        /// Stops sensor reading.
        /// </summary>
        public void Stop() =>
            StopContinuousReads();

        private void StopContinuousReads() =>
            IsRunning = false;

        private void PerformContinuousReads(object state)
        {
            while (IsRunning)
            {
                // Acquire measure
                var sensorData = RetrieveSensorData();

                if (IsRunning)
                {
                    OnDataAvailable?.Invoke(this, sensorData);
                    Thread.Sleep(200);
                }
            }

            Dispose(true);
        }

        private UltrasonicReadEventArgs RetrieveSensorData()
        {
            try
            {
                // Send trigger pulse
                TriggerPin.Write(GpioPinValue.Low);
                Pi.Timing.SleepMicroseconds(2);
                TriggerPin.Write(GpioPinValue.High);
                Pi.Timing.SleepMicroseconds(12);
                TriggerPin.Write(GpioPinValue.Low);

                if (!EchoPin.WaitForValue(GpioPinValue.High, 50))
                    throw new TimeoutException();

                measurementTimer.Start();
                if (!EchoPin.WaitForValue(GpioPinValue.Low, 50))
                    throw new TimeoutException();

                measurementTimer.Stop();
                var elapsedTime = measurementTimer.ElapsedMicroseconds;
                measurementTimer.Reset();

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

        #region IDisposable Support

        private bool disposedValue;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() =>
            StopContinuousReads();

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    TriggerPin = null;
                    EchoPin = null;
                    ReadWorker = null;
                }

                disposedValue = true;
            }
        }

#endregion
    }
}
