namespace Unosquare.RaspberryIO.Playground
{
    using Abstractions;
    using Peripherals;
    using Swan;
    using System;

    public partial class Program
    {
        /// <summary>
        /// Tests the button.
        /// </summary>
        public static void TestButton()
        {
            var relayChannel1 = Pi.Gpio.GetGpioPinByBcmPinNumber(17);
            relayChannel1.PinDriveMode = GpioPinDriveMode.Output;

            var buttonPin = Pi.Gpio.GetGpioPinByBcmPinNumber(26);
            buttonPin.PinDriveMode = GpioPinDriveMode.Input;

            var button = new Button(buttonPin);
            button.Pressed += (s, e) => { "Pressed".Info(); };

            button.Released += (s, e) =>
            {
                "Released".Info();
                var value = relayChannel1.Read();
                relayChannel1.Write(!value);
            };

            Console.ReadKey();
        }
    }
}