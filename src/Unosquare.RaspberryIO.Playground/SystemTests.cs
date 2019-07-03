namespace Unosquare.RaspberryIO.Playground
{
    using Computer;
    using Swan;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class SystemTests
    {
        private static readonly Dictionary<ConsoleKey, string> MainOptions = new Dictionary<ConsoleKey, string>
        {
            // Module COntrol Items
            { ConsoleKey.C, "Camera" },
            { ConsoleKey.I, "System Info" },
            { ConsoleKey.V, "Volume" },
        };

        public static async Task ShowMenu()
        {
            var exit = false;
            bool pressKey;

            do
            {
                Console.Clear();
                pressKey = true;

                var mainOption = "System".ReadPrompt(MainOptions, "Esc to exit this menu");

                switch (mainOption.Key)
                {
                    case ConsoleKey.C:
                        await SystemCamera.ShowMenu().ConfigureAwait(false);
                        break;
                    case ConsoleKey.I:
                        await TestSystemInfo().ConfigureAwait(false);
                        break;
                    case ConsoleKey.V:
                        await SystemVolume.ShowMenu().ConfigureAwait(false);
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

            (await NetworkSettings.Instance.RetrieveAdapters())
                .Select(adapter =>
                    $"Adapter: {adapter.Name,6} | IPv4: {adapter.IPv4,16} | IPv6: {adapter.IPv6,28} | AP: {adapter.AccessPointName,16} | MAC: {adapter.MacAddress,18}")
                .ToList()
                .ForEach(x => x.Info());
        }
    }
}
