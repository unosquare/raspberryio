namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    public static class SystemVolume
    {
        private static bool exit = false;
        private static bool mute = false;
        private static int CurrentLevel { get; set; }

        public static async Task ShowMenu()
        {
            ConsoleKey key;
            // Capture Key presses here.
            while (!exit)
            {
                // Do something
                Console.Clear();
                var state = await Pi.Audio.GetState().ConfigureAwait(false);
                CurrentLevel = state.Level;

                // info
                Console.WriteLine($"Control name: {state.ControlName}");
                Console.WriteLine($"Card number: {state.CardNumber}");
                Console.WriteLine($"Volume level (dB): {state.Decibels}dB");
                Console.WriteLine($"Mute: {state.IsMute}");

                Console.Write($"[");
                UpdateProgress(CurrentLevel);
                Console.Write($"] {CurrentLevel}%\n");

                // Key is available - read it
                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.DownArrow:
                        {
                            await DecrementVolume().ConfigureAwait(false);
                            break;
                        }

                    case ConsoleKey.UpArrow:
                        {
                            await IncrementVolume().ConfigureAwait(false);
                            break;
                        }

                    case ConsoleKey.M:
                        {
                            mute = !mute;
                            await ToggleMute().ConfigureAwait(false);
                            break;
                        }

                    case ConsoleKey.Escape:
                        {
                            exit = true;
                            break;
                        }

                    default:
                        break;
                }
            }
        }

        private static void UpdateProgress(int level)
        {
            var progress = new string((char)0x2588, 10);
            var filler = level / 10;

            for (int i = 0; i < filler; ++i)
            {
                if (i < 6)
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (i < 8)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else if (i < 9)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                else if (i > filler)
                    Console.ForegroundColor = ConsoleColor.Black;
                else
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.Write($"{progress[i]}");
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static async Task IncrementVolume() => await Pi.Audio.SetVolumePercentage(CurrentLevel + 1).ConfigureAwait(false);
        private static async Task DecrementVolume() => await Pi.Audio.SetVolumePercentage(CurrentLevel - 1).ConfigureAwait(false);
        private static async Task ToggleMute() => await Pi.Audio.ToggleMute(mute).ConfigureAwait(false);
    }
}
