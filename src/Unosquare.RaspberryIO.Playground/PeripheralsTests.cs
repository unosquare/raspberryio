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
            { ConsoleKey.I, "Infrared Sensor"},
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
                        Program.TestAccelerometer();
                        break;
                    case ConsoleKey.I:
                        Program.TestInfraredSensor();
                        break;
                    case ConsoleKey.R:
                        await Program.ShowRfidMenu().ConfigureAwait(false);
                        break;
                    case ConsoleKey.U:
                        Program.TestUltrasonicSensor();
                        break;
                    case ConsoleKey.T:
                        Program.TestTempSensor();
                        break;
                    case ConsoleKey.Escape:
                        exit = true;
                        pressKey = false;
                        break;
                    default:
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
