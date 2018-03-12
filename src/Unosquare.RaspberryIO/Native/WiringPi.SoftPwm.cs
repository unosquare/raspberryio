﻿namespace Unosquare.RaspberryIO.Native
{
    using System.Runtime.InteropServices;

    public partial class WiringPi
    {
        #region WiringPi - Soft PWM (https://github.com/WiringPi/WiringPi/blob/master/wiringPi/softPwm.h)

        /// <summary>
        /// This creates a software controlled PWM pin. You can use any GPIO pin and the pin numbering will be that of the wiringPiSetup()
        /// function you used. Use 100 for the pwmRange, then the value can be anything from 0 (off) to 100 (fully on) for the given pin.
        /// The return value is 0 for success. Anything else and you should check the global errno variable to see what went wrong.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="pwmRange">The PWM range.</param>
        /// <returns>The result</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "softPwmCreate", SetLastError = true)]
        public static extern int SoftPwmCreate(int pin, int initialValue, int pwmRange);

        /// <summary>
        /// This updates the PWM value on the given pin. The value is checked to be in-range and pins that haven’t previously
        /// been initialized via softPwmCreate will be silently ignored.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="value">The value.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "softPwmWrite", SetLastError = true)]
        public static extern void SoftPwmWrite(int pin, int value);

        /// <summary>
        /// This function is undocumented
        /// </summary>
        /// <param name="pin">The pin.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "softPwmStop", SetLastError = true)]
        public static extern void SoftPwmStop(int pin);

        /// <summary>
        /// This creates a software controlled tone pin. You can use any GPIO pin and the pin numbering will be that of the wiringPiSetup() function you used.
        /// The return value is 0 for success. Anything else and you should check the global errno variable to see what went wrong.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>The result</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "softToneCreate", SetLastError = true)]
        public static extern int SoftToneCreate(int pin);

        /// <summary>
        /// This function is undocumented
        /// </summary>
        /// <param name="pin">The pin.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "softToneStop", SetLastError = true)]
        public static extern void SoftToneStop(int pin);

        /// <summary>
        /// This updates the tone frequency value on the given pin. The tone will be played until you set the frequency to 0.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="freq">The freq.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "softToneWrite", SetLastError = true)]
        public static extern void SoftToneWrite(int pin, int freq);

        #endregion

    }
}
