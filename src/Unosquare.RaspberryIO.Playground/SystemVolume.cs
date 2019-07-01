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
                UpdateProgress(state.Level);
                Console.Write($"] {state.Level}%\n");

                // Key is available - read it
                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        {
                            await DecrementVolume().ConfigureAwait(false);
                            break;
                        }

                    case ConsoleKey.RightArrow:
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
            var emptier = 10 - filler;

            for (int i = 0; i < 10; ++i)
            {
                if (i < filler)
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($"{progress[i]}");
            }

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static async Task IncrementVolume() => await Pi.Audio.SetVolumePercentage(CurrentLevel + 1).ConfigureAwait(false);
        private static async Task DecrementVolume() => await Pi.Audio.SetVolumePercentage(CurrentLevel - 1).ConfigureAwait(false);
        private static async Task ToggleMute()
        {
            await Pi.Audio.ToggleMute(mute).ConfigureAwait(false);
            await Pi.Audio.SetVolumePercentage(0).ConfigureAwait(false);
        }
    }
}
