using System.Linq;

namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Swan;
    using Unosquare.Swan.Components;

    public static partial class SystemTests
    {
        private static readonly Dictionary<ConsoleKey, string> MainOptions = new Dictionary<ConsoleKey, string>
        {
            { ConsoleKey.B, "Bluetooth" },
            { ConsoleKey.C, "Camera" },
            { ConsoleKey.I, "System Info" },
            { ConsoleKey.V, "Volume" },
        };

        public static async Task ShowMenu()
        {
            var exit = false;

            do
            {
                Console.Clear();
                var mainOption = "System".ReadPrompt(MainOptions, "Esc to exit this menu");

                switch (mainOption.Key)
                {
                    case ConsoleKey.B:
                        await TestBluetooth().ConfigureAwait(false);
                        break;
                    case ConsoleKey.C:
                        SystemCamera.ShowMenu();
                        break;
                    case ConsoleKey.I:
                        await TestSystemInfo().ConfigureAwait(false);
                        break;
                    case ConsoleKey.V:
                        await SystemVolume.ShowMenu().ConfigureAwait(false);
                        break;
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                }
            }
            while (!exit);
        }

        private static async Task TestBluetooth()
        {
            Console.Clear();
            "test bluetooth".Info();
            var devices = await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "list").ConfigureAwait(false);
            var test = devices + devices + devices;
            "after test".ReadKey();
        }
    }
}
