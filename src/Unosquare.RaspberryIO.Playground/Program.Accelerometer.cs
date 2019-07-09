namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using Unosquare.RaspberryIO.Peripherals;
    using Unosquare.Swan;

    public static partial class Program
    {
        /// <summary>
        /// Test the GY521 Accelerometer and Gyroscope.
        /// </summary>
        public static void TestAccelerometer()
        {
            Console.Clear();
            // Add device
            var accel_device = Pi.I2C.AddDevice(0x68);

            // Set accelerometer
            using (var accelSensor = new AccelerometerGY521(accel_device))
            {
                // Present info to screen
                accelSensor.DataAvailable +=
                    (s, e) =>
                    {
                        Console.Clear();
                        $"\nAccelerometer:\n{e.Accel}\n\nGyroscope:\n{e.Gyro}\n\nTemperature: {Math.Round(e.Temperature, 2)}°C\n"
                            .Info("GY-521");
                        "Press Esc key to continue . . .".WriteLine();
                    };

                // Run accelerometer
                accelSensor.Start();
                while (true)
                {
                    var input = Console.ReadKey(true).Key;
                    if (input == ConsoleKey.Escape)
                    {
                        break;
                    }
                }
            }
        }
    }
}
