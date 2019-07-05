namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using Unosquare.RaspberryIO.Abstractions;
    using Unosquare.RaspberryIO.Peripherals;
    using Unosquare.Swan;

    public partial class Program
    {
        public static void TestButton()
        {
            Console.Clear();

            "Testing Button".Info();
            var inputPin = Pi.Gpio[BcmPin.Gpio12];
            var button = new Button(inputPin);

            button.Pressed += (s, e) => LogMessageOnEvent("Pressed");
            button.Released += (s, e) => LogMessageOnEvent("Realeased");

            Console.ReadKey(true);
        }

        private static void LogMessageOnEvent(string message)
        {
            Console.Clear();
            message.Info();
            "Press any key to continue . . .".WriteLine();
        }
    }
}
