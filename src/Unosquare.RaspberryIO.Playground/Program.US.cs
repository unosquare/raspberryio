namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using Unosquare.RaspberryIO.Abstractions;
    using Unosquare.RaspberryIO.Peripherals;
    using Unosquare.Swan;

    public partial class Program
    {
        public static void TestUltrasonicSensor()
        {
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
                                color = ConsoleColor.Blue;
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

                        "\n Press any key to continue . . .".WriteLine();
                    }
                };

                sensor.Start();
                Console.ReadKey(true);
            }
        }

    }
}
