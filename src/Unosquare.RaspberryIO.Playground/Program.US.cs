namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Unosquare.RaspberryIO.Abstractions;
    using Unosquare.RaspberryIO.Peripherals;
    using Unosquare.Swan;

    public partial class Program
    {
        private static readonly Dictionary<ConsoleKey, string> USOptions = new Dictionary<ConsoleKey, string>
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

        public static void TestUltrasonicSensor()
        {
            Console.Clear();
            var color = ConsoleColor.White;
            using (var sensor = new UltrasonicHcsr04(Pi.Gpio[BcmPin.Gpio23], Pi.Gpio[BcmPin.Gpio24]))
            {
                sensor.OnDataAvailable += (s, e) =>
                {
                    Console.Clear();
                    if (e.IsValid)
                    {
                        if (e.HasObstacles)
                        {
                            if (e.Distance <= 10)
                                color = ConsoleColor.DarkRed;
                            else if (e.Distance <= 20)
                                color = ConsoleColor.DarkYellow;
                            else if (e.Distance <= 30)
                                color = ConsoleColor.Yellow;
                            else if (e.Distance <= 40)
                                color = ConsoleColor.Green;
                            else if (e.Distance <= 50)
                                color = ConsoleColor.Cyan;
                            else
                                color = ConsoleColor.White;

                            var distance = e.Distance < 57 ? e.Distance : 58;
                            $"{new string('█', (int)distance)}".WriteLine(color);
                            "--------------------------------------------------------->".WriteLine();
                            "          10        20        30        40        50       cm".WriteLine();
                            $"Obstacle detected at {e.Distance:N2}cm / {e.DistanceInch:N2}in".WriteLine();
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
