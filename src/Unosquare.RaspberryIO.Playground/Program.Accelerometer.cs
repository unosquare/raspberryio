namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Unosquare.RaspberryIO.Peripherals;
    using Unosquare.Swan;

    public static partial class Program
    {
        private static readonly Dictionary<ConsoleKey, string> AccelerometerOptions = new Dictionary<ConsoleKey, string>
        {
            { ConsoleKey.S, "Show General Data" },
        };

        public static async Task ShowAccelerometerMenu()
        {
            var exit = false;
            bool pressKey;

            do
            {
                Console.Clear();
                pressKey = true;

                var mainOption = "Rfid".ReadPrompt(AccelerometerOptions, "Esc to exit this menu");

                switch (mainOption.Key)
                {
                    case ConsoleKey.S:
                        TestAccelerometer();
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
                    };

                // Run accelerometer
                accelSensor.Start();
                Console.ReadKey(true);
            }
        }
    }
}
