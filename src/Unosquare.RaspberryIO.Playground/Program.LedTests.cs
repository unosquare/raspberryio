namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Threading;
    using Unosquare.RaspberryIO.Abstractions;
    using Unosquare.Swan;
    using Unosquare.WiringPi;

    public partial class Program
    {
        public static void TestLedBlinking()
        {
            Console.Clear();

            // Get a reference to the pin you need to use.
            // All methods below are exactly equivalent and reference the same pin
            var blinkingPin = Pi.Gpio[16];
            blinkingPin = Pi.Gpio[BcmPin.Gpio26];
            blinkingPin = Pi.Gpio[P1.Pin37];
            blinkingPin = ((GpioController)Pi.Gpio)[WiringPiPin.Pin26];
            blinkingPin = ((GpioController)Pi.Gpio).Pin26;
            blinkingPin = ((GpioController)Pi.Gpio).HeaderP1[37];

            // Configure the pin as an output
            blinkingPin.PinMode = GpioPinDriveMode.Output;

            // perform writes to the pin by toggling the isOn variable
            var isOn = false;
            for (var i = 0; i < 20; i++)
            {
                isOn = !isOn;
                blinkingPin.Write(isOn);
                Thread.Sleep(500);
                "Press any key to continue . . .".ReadKey(true);
            }
        }
    }
}
