namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Unosquare.RaspberryIO.Abstractions;
    using Unosquare.RaspberryIO.Peripherals;
    using Unosquare.Swan;

    public partial class Program
    {
        private static readonly Dictionary<ConsoleKey, string> USOptions= new Dictionary<ConsoleKey, string>
        {
            { ConsoleKey.D, "Distance Measurement" },
        };

        public static async Task ShowUSMenu()
        {
            var exit = false;
            bool pressKey;

            do
            {
                Console.Clear();
                pressKey = true;

                var mainOption = "Rfid".ReadPrompt(USOptions, "Esc to exit this menu");

                switch (mainOption.Key)
                {
                    case ConsoleKey.D:
                        TestUltrasonicSensor();
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

        public static void DistanceMeasurement()
        {

        }

        public static void TestUltrasonicSensor()
        {
            using (var sensor = new UltrasonicHcsr04(Pi.Gpio[BcmPin.Gpio23], Pi.Gpio[BcmPin.Gpio24]))
            {
                sensor.OnDataAvailable += (s, e) =>
                {
                    if (e.IsValid)
                    {
                        if (e.HasObstacles)
                        {
                            if (e.Distance < 10)
                                $"Obstacle detected at {e.Distance:N2}cm / {e.DistanceInch:N2}in".WriteLine(ConsoleColor.Red);
                            else if (e.Distance < 30)
                                $"Obstacle detected at {e.Distance:N2}cm / {e.DistanceInch:N2}in".WriteLine(ConsoleColor.DarkYellow);
                            else if (e.Distance < 100)
                                $"Obstacle detected at {e.Distance:N2}cm / {e.DistanceInch:N2}in".WriteLine(ConsoleColor.Yellow);
                            else
                                $"Obstacle detected at {e.Distance:N2}cm / {e.DistanceInch:N2}in".WriteLine(ConsoleColor.Green);
                        }
                        else
                        {
                            "No obstacles detected.".Info("HC - SR04");
                        }
                    }
                    else
                    {
                        "Invalid Reading".Error("HC-SR04");
                    }
                };

                sensor.Start();
                Console.ReadKey(true);
            }
        }

    }
}
