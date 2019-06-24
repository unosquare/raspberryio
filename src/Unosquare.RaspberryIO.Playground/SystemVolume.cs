namespace Unosquare.RaspberryIO.Playground
{
    using Swan;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class SystemVolume
    {
        private static readonly Dictionary<ConsoleKey, string> MainOptions = new Dictionary<ConsoleKey, string>
        {
            { ConsoleKey.DownArrow, "Volume Down" },
            { ConsoleKey.UpArrow, "Volume Up" },
            { ConsoleKey.M, "Mute/Unmute" },
        };

        private static bool Mute { get; set; } = false;

        public static async Task ShowMenu()
        {
            var exit = false;
            bool pressKey;

            while (!exit)
            {
                Console.Clear();
                var state = await Pi.Audio.GetState().ConfigureAwait(false);
                pressKey = true;

                var mainOption = "System".ReadPrompt(MainOptions, "Press Esc to exit this menu");
                Console.WriteLine(state.Decibels);

                switch (mainOption.Key)
                {
                    case ConsoleKey.DownArrow:
                        await DecrementVolume().ConfigureAwait(false);
                        break;
                    case ConsoleKey.UpArrow:
                        await IncrementVolume().ConfigureAwait(false);
                        break;
                    case ConsoleKey.M:
                        await ToggleMute().ConfigureAwait(false);
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
        }

        private static async Task IncrementVolume() =>
            await Pi.Audio.IncrementVolume(1.00f).ConfigureAwait(false);

        private static async Task DecrementVolume() =>
            await Pi.Audio.IncrementVolume(-1.00f).ConfigureAwait(false);

        private static async Task ToggleMute() =>
            await Pi.Audio.ToggleMute(!Mute).ConfigureAwait(false);
    }
}
