namespace Unosquare.RaspberryIO.Peripherals
{
    using Gpio;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A class to send infrared signals using an IR LED.
    /// It uses a carrier frequency of ~38kHz
    /// </summary>
    public sealed class InfraredEmitter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfraredEmitter"/> class.
        /// </summary>
        /// <param name="outputPin">The output pin.</param>
        public InfraredEmitter(GpioPin outputPin)
        {
            if (outputPin == null || outputPin.Capabilities.Contains(PinCapability.PWM) == false)
                throw new ArgumentException("Pin does not support PWM", nameof(outputPin));

            OutputPin = outputPin;
            OutputPin.PinMode = GpioPinDriveMode.PwmOutput;
            OutputPin.PwmMode = PwmMode.MarkSign;
            OutputPin.PwmClockDivisor = 5;
            OutputPin.PwmRange = 101;
            OutputPin.PwmRegister = 0;
        }

        /// <summary>
        /// Gets the output pin.
        /// </summary>
        public GpioPin OutputPin { get; }

        /// <summary>
        /// Snaps the pulse lengths to their valid equivalents.
        /// </summary>
        /// <param name="pulses">The pulses.</param>
        /// <param name="validPulseLengths">The valid pulse lengths.</param>
        /// <returns>An aray of pulses with exact lengths</returns>
        public static InfraredSensor.InfraredPulse[] SnapPulseLengths(
            IEnumerable<InfraredSensor.InfraredPulse> pulses, IEnumerable<long> validPulseLengths)
        {
            var result = new List<InfraredSensor.InfraredPulse>(pulses.Count());
            foreach (var p in pulses)
            {
                var pulseLength = validPulseLengths.OrderBy(v => Math.Abs(p.DurationUsecs - v)).FirstOrDefault();
                result.Add(new InfraredSensor.InfraredPulse(p.Value, pulseLength));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Sends the specified pulses.
        /// </summary>
        /// <param name="pulses">The pulses.</param>
        public void Send(IEnumerable<InfraredSensor.InfraredPulse> pulses)
        {
            foreach (var p in pulses)
            {
                OutputPin.PwmRegister = p.Value ? 51 : 0;
                Pi.Timing.SleepMicroseconds((uint)p.DurationUsecs);
            }

            OutputPin.PwmRegister = 0;
        }
    }
}
