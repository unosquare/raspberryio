namespace Unosquare.RaspberryIO.Playground
{
    using Computer;
    using System;
    using System.Linq;

    public static partial class SystemTests
    {
        private static void TestSystemInfo()
        {
            Console.Clear();

            Console.WriteLine($"GPIO Controller initialized successfully with {Pi.Gpio.Count} pins");
            Console.WriteLine($"{Pi.Info}");

            try
            {
                Console.WriteLine($"BoardModel {Pi.Info.BoardModel}");
                Console.WriteLine($"ProcessorModel {Pi.Info.ProcessorModel}");
                Console.WriteLine($"Manufacturer {Pi.Info.Manufacturer}");
                Console.WriteLine($"MemorySize {Pi.Info.MemorySize}");
            }
            catch (Exception x)
            {
                Console.WriteLine("Error obtaining system configuration: " + x.Message);
            }

            try
            {
                Console.WriteLine($"Uname {Pi.Info.OperatingSystem}");
                Console.WriteLine($"HostName {NetworkSettings.Instance.HostName}");
                Console.WriteLine($"Uptime (seconds) {Pi.Info.Uptime}");
                var timeSpan = Pi.Info.UptimeTimeSpan;
                Console.WriteLine($"Uptime (timespan) {timeSpan.Days} days {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");

                var adapters = NetworkSettings.Instance.RetrieveAdapters();
                adapters.Result.Select(adapter => $"Adapter: {adapter.Name,6} | IPv4: {adapter.IPv4,16} | IPv6: {adapter.IPv6,28} | AP: {adapter.AccessPointName,16} | MAC: {adapter.MacAddress,18}")
                    .ToList()
                    .ForEach(x => Console.WriteLine(x));
            }
            catch(Exception x)
            {
                Console.WriteLine("Error retrieving network interface settings: " + x.Message);
            }
            Console.WriteLine(ExitMessage);

            while (true)
            {
                var input = Console.ReadKey(true).Key;
                if (input != ConsoleKey.Escape) continue;

                break;
            }
        }
    }
}
