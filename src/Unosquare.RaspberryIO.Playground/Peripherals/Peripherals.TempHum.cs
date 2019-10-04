namespace Unosquare.RaspberryIO.Playground.Peripherals
{
    using System;
    using Abstractions;
    using Unosquare.RaspberryIO.Peripherals;

    public static partial class Peripherals
    {
        /// <summary>
        /// Tests the temperature sensor.
        /// The DHT11 sensor, also available as KY-015 for usage on experimental boards, needs to be connected to Gpio04 (physical pin 7) with its
        /// single data line. The sensor has a conspicuous blue grid-shaped housing. 
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
                    if (!e.IsValid)
                        return;

                    Console.Clear();
                    validReadings++;
                    Console.WriteLine($"DHT11 Temperature: \n {e.Temperature:0.00}°C \n {e.TemperatureFahrenheit:0.00}°F  \n Humidity: {e.HumidityPercentage:P0}\n\n");
                    Console.WriteLine($"      Number of valid data samples received: {validReadings} of {totalReadings}");
                    Console.WriteLine();
                    Console.WriteLine(ExitMessage);
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
