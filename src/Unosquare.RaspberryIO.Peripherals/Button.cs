namespace Unosquare.RaspberryIO.Peripherals
{
    using System;
    using Unosquare.RaspberryIO.Abstractions;

    /// <summary>
    /// Implements a generic button attached to the GPIO.
    /// </summary>
    public class Button
    {
        internal const ulong InterruptTime = 500;

        private readonly IGpioPin _gpioPin;
        private ulong _pressedLastInterrupt;
        private ulong _releasedLastInterrupt;

        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class.
        /// </summary>
        /// <param name="gpioPin">The gpio pin.</param>
        public Button(IGpioPin gpioPin)
        {
            _gpioPin = gpioPin;

            _gpioPin.InputPullMode = GpioPinResistorPullMode.PullDown;
            _gpioPin.PinMode = GpioPinDriveMode.Input;
            _gpioPin.RegisterInterruptCallback(EdgeDetection.FallingAndRisingEdge, HandleInterrupt);
        }

        /// <summary>
        /// Occurs when [pressed].
        /// </summary>
        public event EventHandler<EventArgs> Pressed;

        /// <summary>
        /// Occurs when [released].
        /// </summary>
        public event EventHandler<EventArgs> Released;

        private void HandleInterrupt()
        {
            if (_gpioPin.Read())
            {
                HandleButtonPressed();
            }
            else
            {
                HandleButtonReleased();
            }
        }

        private void HandleButtonPressed()
        {
            ulong interruptTime = Pi.Timing.Milliseconds;

            if (interruptTime - _pressedLastInterrupt <= InterruptTime) return;
            _pressedLastInterrupt = interruptTime;
            Pressed?.Invoke(this, new EventArgs());
        }

        private void HandleButtonReleased()
        {
            ulong interruptTime = Pi.Timing.Milliseconds;

            if (interruptTime - _releasedLastInterrupt <= InterruptTime) return;
            _releasedLastInterrupt = interruptTime;
            Released?.Invoke(this, new EventArgs());
        }
    }
}
