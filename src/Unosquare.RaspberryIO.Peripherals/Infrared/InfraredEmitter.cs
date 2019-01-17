namespace Unosquare.RaspberryIO.Peripherals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Abstractions;
    using WiringPi;

    /// <summary>
    /// A class to send infrared signals using an IR LED.
    /// It uses a carrier frequency of ~38kHz.
    /// </summary>
    public sealed class InfraredEmitter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfraredEmitter"/> class.
        /// </summary>
        /// <param name="outputPin">The output pin.</param>
        public InfraredEmitter(GpioPin outputPin)
        {
            if (outputPin == null || outputPin.HasCapability(PinCapability.PWM) == false)
                throw new ArgumentException("Pin does not support PWM", nameof(outputPin));

            OutputPin = outputPin;
            OutputPin.PinMode = GpioPinDriveMode.PwmOutput;
            OutputPin.PwmMode = PwmMode.MarkSign;

            // Parameters taken from:
            // https://mariodivece.com/blog/2018/03/21/rpi-pwm-demystified
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
        /// <returns>An aray of pulses with exact lengths.</returns>
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

        /// <summary>
        /// Provides a NEC IR protocol data encoder.
        /// </summary>
        public static class NecEncoder
        {
            private static readonly InfraredSensor.InfraredPulse PreambleMark = new InfraredSensor.InfraredPulse(true, 9000);
            private static readonly InfraredSensor.InfraredPulse DataSpace = new InfraredSensor.InfraredPulse(false, 4500);
            private static readonly InfraredSensor.InfraredPulse RepeatSpace = new InfraredSensor.InfraredPulse(false, 2250);
            private static readonly InfraredSensor.InfraredPulse ShortMark = new InfraredSensor.InfraredPulse(true, 562);
            private static readonly InfraredSensor.InfraredPulse ShortSpace = new InfraredSensor.InfraredPulse(false, 562);
            private static readonly InfraredSensor.InfraredPulse LongSpace = new InfraredSensor.InfraredPulse(false, 1687);

            private static readonly InfraredSensor.InfraredPulse[] RepeatPulses = new[]
            {
                PreambleMark,
                RepeatSpace,
                ShortMark,
            };

            private static readonly InfraredSensor.InfraredPulse[] PreambleData = new[]
            {
                PreambleMark,
                DataSpace,
            };

            private static readonly InfraredSensor.InfraredPulse[] V0 = new[]
            {
                ShortMark,
                ShortSpace,
            };

            private static readonly InfraredSensor.InfraredPulse[] V1 = new[]
            {
                ShortMark,
                LongSpace,
            };

            /// <summary>
            /// Encodes the specified 4-byte data into IR pulses.
            /// </summary>
            /// <param name="data">The data.</param>
            /// <returns>The data encoded as IR pulses.</returns>
            /// <exception cref="ArgumentException">The data has to be 4 bytes long. - data.</exception>
            public static InfraredSensor.InfraredPulse[] Encode(byte[] data)
            {
                if (data == null || data.Length != 4)
                    throw new ArgumentException("The data has to be 4 bytes long.", nameof(data));

                var result = new List<InfraredSensor.InfraredPulse>(67);
                result.AddRange(PreambleData);

                var bits = new BitArray(data);
                for (var bitIndex = 0; bitIndex < bits.Length; bitIndex++)
                {
                    var v = bits[bitIndex] ? V1 : V0;
                    result.AddRange(v);
                }

                result.Add(ShortMark);
                return result.ToArray();
            }
        }
    }
}
