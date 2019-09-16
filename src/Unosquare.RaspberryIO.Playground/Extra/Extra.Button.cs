namespace Unosquare.RaspberryIO.Playground.Extra
{
    using Abstractions;
    using System;
    using Unosquare.RaspberryIO.Peripherals;

    public static partial class Extra
    {
        public static void TestButton()
        {
            Console.Clear();

            var inputPin = Pi.Gpio[BcmPin.Gpio24];
            var button = new Button(inputPin, GpioPinResistorPullMode.PullUp);

            button.Pressed += (s, e) => LogMessageOnEvent("Pressed");
            button.Released += (s, e) => LogMessageOnEvent("Released");

            while (true)
            {
                var input = Console.ReadKey(true).Key;

                if (input != ConsoleKey.Escape) continue;

                break;
            }
        }

        private static void LogMessageOnEvent(string message)
        {
            Console.Clear();
            Console.WriteLine(message);
            Console.WriteLine(ExitMessage);
        }
    }
}
