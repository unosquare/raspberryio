namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Unosquare.Swan;

    public static class PeripheralsTests
    {
        private static readonly Dictionary<ConsoleKey, string> MainOptions = new Dictionary<ConsoleKey, string>
        {
            { ConsoleKey.A, "Accelerometer"},
            { ConsoleKey.R, "Rfid Controller" },
            { ConsoleKey.U, "Ultrasonic Sensor"},
            { ConsoleKey.T, "Temperature Sensor"},
        };

        public static async Task ShowMenu()
        {
            var exit = false;
            bool pressKey = false;

            do
            {
                Console.Clear();
                pressKey = true;

                var mainOption = "Peripherals".ReadPrompt(MainOptions, "Esc to exit this menu");

                switch (mainOption.Key)
                {
                    case ConsoleKey.A:
                        await Program.ShowAccelerometerMenu().ConfigureAwait(false);
                        break;

                    case ConsoleKey.R:
                        await Program.ShowRfidMenu().ConfigureAwait(false);
                        break;

                    case ConsoleKey.U:
                        await Program.ShowUSMenu().ConfigureAwait(false);
                        break;

                    case ConsoleKey.T:
                        await Program.ShowTSMenu().ConfigureAwait(false);
                        break;

                    case ConsoleKey.Escape:
                        exit = true;
                        pressKey = false;
                        break;
                }

                if (pressKey)
                {
                    await Task.Delay(500).ConfigureAwait(false);
                    Console.WriteLine("Press any key to continue . . .");
                    Console.ReadKey(true);
                }
            }
            while (!exit);
        }
    }
}
