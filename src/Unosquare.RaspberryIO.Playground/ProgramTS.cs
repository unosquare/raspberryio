namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using Unosquare.RaspberryIO.Abstractions;
    using Unosquare.RaspberryIO.Peripherals;
    using Unosquare.Swan;

    public partial class Program
    {
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
                    totalReadings++;
                    if (e.IsValid)
                    {
                        Console.Clear();
                        validReadings++;
                        $"Temperature: \n {e?.Temperature ?? 0:0.00}°C \n {e?.TemperatureFahrenheit ?? 0:0.00}°F  \n Humidity: {e?.HumidityPercentage ?? 0:P0}\n\n".Info("DHT11");
                        "Press Esc key to continue . . .".WriteLine();
                    }
                };

                sensor.Start();
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
