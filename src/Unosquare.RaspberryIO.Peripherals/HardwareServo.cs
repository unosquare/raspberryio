namespace Unosquare.RaspberryIO.Peripherals
{
    using Abstractions;
    using Swan;
    using System;
    using WiringPi;

    /// <summary>
    /// Represents a standard 50hz pulse-controlled servo using Hardware-assited PWM.
    /// Maximum duty cycle is ~25% = 20ms x .25 = 5ms max pulse width.
    /// </summary>
    public sealed class HardwareServo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HardwareServo"/> class.
        /// </summary>
        /// <param name="outputPin">The output pin.</param>
        /// <exception cref="ArgumentException">Pin does not support PWM - outputPin.</exception>
        public HardwareServo(GpioPin outputPin)
        {
            if (outputPin == null || outputPin.HasCapability(PinCapability.PWM) == false)
                throw new ArgumentException("Pin does not support PWM", nameof(outputPin));

            OutputPin = outputPin;
            OutputPin.PinMode = GpioPinDriveMode.PwmOutput;
            OutputPin.PwmMode = PwmMode.MarkSign;

            // Parameters taken from:
            // https://mariodivece.com/blog/2018/03/21/rpi-pwm-demystified
            OutputPin.PwmClockDivisor = 96;
            OutputPin.PwmRange = 4000;
            OutputPin.PwmRegister = 0;

            Frequency = (double)((GpioController)Pi.Gpio).PwmBaseFrequency / OutputPin.PwmClockDivisor / OutputPin.PwmRange;
            PeriodMs = 1d / Frequency * 1000;
            MaxPulseLengthMs = PeriodMs * 1024d / OutputPin.PwmRange;
            PulseLengthMs = 1.0d; // default is 1ms pulses
        }

        /// <summary>
        /// Gets the output pin.
        /// </summary>
        public GpioPin OutputPin { get; }

        /// <summary>
        /// Gets the PWM frequency.
        /// </summary>
        public double Frequency { get; }

        /// <summary>
        /// Gets the period in milliseconds.
        /// </summary>
        public double PeriodMs { get; }

        /// <summary>
        /// Gets the maximum pulse length in milliseconds.
        /// </summary>
        public double MaxPulseLengthMs { get; }

        /// <summary>
        /// Gets or sets the pulse length in milliseconds.
        /// </summary>
        public double PulseLengthMs
        {
            get => PeriodMs * OutputPin.PwmRegister.Clamp(0, 1024) / OutputPin.PwmRange;
            set
            {
                value = value.Clamp(0, MaxPulseLengthMs);
                var register = value / PeriodMs * OutputPin.PwmRange;
                OutputPin.PwmRegister = Convert.ToInt32(register).Clamp(0, 1024);
            }
        }

        /// <summary>
        /// Computes the standard 0 to 180 angle of he current pulse length compared to a minimum and maximum pulse length.
        /// </summary>
        /// <param name="pulseLengthMin">The pulse length minimum.</param>
        /// <param name="pulseLengthMax">The pulse length maximum.</param>
        /// <returns>The angle in degrees.</returns>
        public double ComputeAngle(double pulseLengthMin, double pulseLengthMax)
        {
            var currentPulse = PulseLengthMs.Clamp(pulseLengthMin, pulseLengthMax) - pulseLengthMin;
            var currentRange = pulseLengthMax - pulseLengthMin;
            return currentPulse / currentRange * 180d;
        }

        /// <summary>
        /// Computes the pulse length in milliseconds for the given angle (from 0 to 180).
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="pulseLengthMin">The pulse length minimum.</param>
        /// <param name="pulseLengthMax">The pulse length maximum.</param>
        /// <returns>The pulse length in milliseconds for the given angle (from 0 to 180).</returns>
        public double ComputePulseLength(double angle, double pulseLengthMin, double pulseLengthMax) =>
            (angle.Clamp(0, 180) / 180 * (pulseLengthMax - pulseLengthMin)) + pulseLengthMin;

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString() =>
            $"Period: {PeriodMs,6:0.00} ms. | Frequency {Frequency,6:0.000} Hz. | Pulse Length: {PulseLengthMs,8:0.000} ms.";
    }
}