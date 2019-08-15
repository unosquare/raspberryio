namespace Unosquare.RaspberryIO.Playground.Peripherals
{
    using System;
    using Swan;
    using Swan.Logging;
    using Unosquare.RaspberryIO.Abstractions;
    using Unosquare.RaspberryIO.Peripherals;

    public static partial class Peripherals
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
                    if (!e.IsValid) return;

                    Console.Clear();
                    validReadings++;
                    $"Temperature: \n {e.Temperature:0.00}°C \n {e.TemperatureFahrenheit:0.00}°F  \n Humidity: {e.HumidityPercentage:P0}\n\n".Info("DHT11");
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
