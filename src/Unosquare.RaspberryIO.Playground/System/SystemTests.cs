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
            //var controllers = await Pi.Bluetooth.ListControllers().ConfigureAwait(false);
            //v.ForEach((device) => $"{device}".Info());

            //var defaultControllerName = controllers.Find((controller) => controller.Contains("default"));
            //var defaultControllerAddress = defaultControllerName.Trim().Split(' ')[1].Trim();
            //$"{defaultControllerAddress}".Info();
            //
            //await ProcessRunner.GetProcessOutputAsync("bluetoothctl", $"select {defaultControllerAddress}").ConfigureAwait(false); // Selects the controller to pair. Once you select the controller, all controller-related commands will apply to it for three minutes.
            //var discoverable = await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "discoverable on").ConfigureAwait(false); // Makes the controller visible to other devices.
            //$"{discoverable}".Info();
            //
            //var pairable = await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "pairable on").ConfigureAwait(false); // Readies the controller for pairing. Remember that you have three minutes after running this command to pair.
            //$"{pairable}".Info();
            //
            var deviceAddress = "F8:C3:9E:BB:9D:CB"; //"F8:C3:9E:BB:9D:CA";

            //var pair = await ProcessRunner.GetProcessOutputAsync("bluetoothctl", $"pair {deviceAddress}").ConfigureAwait(false); // Pairs the device with the controller.
            // $" pair {pair}".Info();

            //var info = await ProcessRunner.GetProcessOutputAsync("bluetoothctl", $"info {deviceAddress}").ConfigureAwait(false); // 
            var info = await ProcessRunner.GetProcessResultAsync("bluetoothctl", "Devices").ConfigureAwait(false);
            $"{info}".Info();

            //await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "discoverable off").ConfigureAwait(false); // Hides the controller from other Bluetooth devices. Otherwise, any device that can detect it has access to it, leaving a major security hole.
            //$"{discoverable}".Info();

            "after test".ReadKey();
        }
    }
}
