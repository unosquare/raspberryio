namespace Unosquare.RaspberryIO.Playground
{
    using Computer;
    using Swan;
    using Swan.Logging;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public static partial class SystemTests
    {
        private static async Task TestSystemInfo()
        {
            Console.Clear();

            $"GPIO Controller initialized successfully with {Pi.Gpio.Count} pins".Info();
            $"{Pi.Info}".Info();

            try
            {
                $"BoardModel {Pi.Info.BoardModel}".Info();
                $"ProcessorModel {Pi.Info.ProcessorModel}".Info();
                $"Manufacturer {Pi.Info.Manufacturer}".Info();
                $"MemorySize {Pi.Info.MemorySize}".Info();
            }
            catch
            {
                // ignore
            }

            $"Uname {Pi.Info.OperatingSystem}".Info();
            $"HostName {NetworkSettings.Instance.HostName}".Info();
            $"Uptime (seconds) {Pi.Info.Uptime}".Info();
            var timeSpan = Pi.Info.UptimeTimeSpan;
            $"Uptime (timespan) {timeSpan.Days} days {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}"
                .Info();

            (await NetworkSettings.Instance.RetrieveAdapters().ConfigureAwait(false))
                .Select(adapter => $"Adapter: {adapter.Name,6} | IPv4: {adapter.IPv4,16} | IPv6: {adapter.IPv6,28} | AP: {adapter.AccessPointName,16} | MAC: {adapter.MacAddress,18}")
                .ToList()
                .ForEach(x => x.Info());

            Terminal.WriteLine(ExitMessage);

            while (true)
            {
                var input = Console.ReadKey(true).Key;
                if (input != ConsoleKey.Escape) continue;

                break;
            }
        }
    }
}
