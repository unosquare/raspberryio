namespace Unosquare.RaspberryIO.Playground.Peripherals
{
    using System;
    using Abstractions;
    using Swan;
    using Swan.Logging;
    using Unosquare.RaspberryIO.Peripherals;

    public static partial class Peripherals
    {
        public static void TestUltrasonicSensor()
        {
            ConsoleColor color;

            using (var sensor = new UltrasonicHcsr04(Pi.Gpio[BcmPin.Gpio23], Pi.Gpio[BcmPin.Gpio24]))
            {
                sensor.OnDataAvailable += (s, e) =>
                {
                    Console.Clear();

                    if (!e.IsValid) return;

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
                            color = ConsoleColor.Blue;
                        else
                            color = ConsoleColor.White;

                        var distance = e.Distance < 57 ? e.Distance : 58;

                        Terminal.WriteLine($"{new string('█', (int)distance)}", color);
                        Terminal.WriteLine("--------------------------------------------------------->");
                        Terminal.WriteLine("          10        20        30        40        50       cm");
                        Terminal.WriteLine($"Obstacle detected at {e.Distance:N2}cm / {e.DistanceInch:N2}in\n");
                    }
                    else
                    {
                        "No obstacles detected.\n".Info("HC - SR04");
                    }

                    Terminal.WriteLine(ExitMessage);
                };

                sensor.Start();
                while (true)
                {
                    var input = Console.ReadKey(true).Key;
                    if (input != ConsoleKey.Escape) continue;

                    break;
                }
            }
        }
    }
}
