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
        private static readonly Dictionary<ConsoleKey, string> TSOptions = new Dictionary<ConsoleKey, string>
        {
            { ConsoleKey.S, "Show General Data" },
        };

        public static async Task ShowTSMenu()
        {
            var exit = false;
            bool pressKey;

            do
            {
                Console.Clear();
                pressKey = true;

                var mainOption = "Rfid".ReadPrompt(TSOptions, "Esc to exit this menu");

                switch (mainOption.Key)
                {
                    case ConsoleKey.S:
                        TestTempSensor();
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
        /// Tests the temperature sensor.
        /// </summary>
        public static void TestTempSensor()
        {
            Console.Clear();
            using (var sensor = DhtSensor.Create(DhtType.Dht11, Pi.Gpio[BcmPin.Gpio04]))
            {
                var totalReadings = 0.0;
                var validReadings = 0.0;

                sensor.OnDataAvailable += (s, e) =>
                {
                    Console.Clear();

                    totalReadings++;
                    if (e.IsValid)
                    {
                        validReadings++;
                        $"Temperature: {e?.Temperature ?? 0:0.00}°C / {e?.TemperatureFahrenheit ?? 0:0.00}°F | Humidity: {e?.HumidityPercentage ?? 0:P0}".Info("DHT11");
                    }
                    else
                    {
                        "Invalid Reading".Error("DHT11");
                    }

                    $"Valid reading percentage: {validReadings / totalReadings:P}".Info("DHT11");
                };

                sensor.Start();
                Console.ReadKey(true);
            }
        }
    }
}
