namespace Unosquare.RaspberryIO.Playground
{
    using Abstractions;
    using System;
    using Swan;
    using Unosquare.RaspberryIO.Peripherals;

    public static partial class Extra
    {
        public static void TestButton()
        {
            Console.Clear();

            "Testing Button".Info();
            var inputPin = Pi.Gpio[BcmPin.Gpio12];
            var button = new Button(inputPin, GpioPinResistorPullMode.PullUp);

            button.Pressed += (s, e) => LogMessageOnEvent("Pressed");
            button.Released += (s, e) => LogMessageOnEvent("Realeased");

            while (true)
            {
                var input = Console.ReadKey(true).Key;

                if (input == ConsoleKey.Escape)
                {
                    break;
                }
            }
        }

        private static void LogMessageOnEvent(string message)
        {
            Console.Clear();
            message.Info();
            "Press Esc key to continue . . .".WriteLine();
        }
    }
}
