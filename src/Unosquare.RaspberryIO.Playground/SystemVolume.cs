namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    public static class SystemVolume
    {
        private static float CurrentLevel { get; set; }
        private static bool Mute { get; set; } = false;
        private static string Progress { get; set; }

        public static async Task ShowMenu()
        {
            ConsoleKey key;
            // Capture Key presses here.Console.WriteLine("Press ESC to stop");
            do
            {

                // Do something
                Console.Clear();
                var state = await Pi.Audio.GetState().ConfigureAwait(false);
                CurrentLevel = state.Decibels;

                // info
                Console.WriteLine($"Control name: {state.ControlName}");
                Console.WriteLine($"Card number: {state.CardNumber}");
                Console.WriteLine($"Volume level (dB): {CurrentLevel}dB");
                Console.WriteLine($"Mute: {state.IsMute}");

                UpdateProgress(state.Level);

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
                            await ToggleMute().ConfigureAwait(false);
                            break;
                        }
                    default:
                        break;
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }

        private static void UpdateProgress(int level)
        {
            Console.Write($"[");
            var progress = new StringBuilder();
            var filler = Math.Floor((float)level / 10);
            var emptier = Math.Floor((float)(100 - filler));

            for (int i = 0; i < filler; ++i)
            {
                progress.Append(char.ConvertFromUtf32(0x2588));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{progress.ToString()}");
            }

            for (int i = 0; i < emptier; ++i)
            {
                progress.Append(char.ConvertFromUtf32(0x2588));
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($"{progress.ToString()}");
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            // Progress bar here
            Console.Write($"] {level}%\n");
        }

        private static async Task IncrementVolume() => await Pi.Audio.SetVolumeByDecibels(CurrentLevel + 1.00f).ConfigureAwait(false);
        private static async Task DecrementVolume() => await Pi.Audio.SetVolumeByDecibels(CurrentLevel - 1.00f).ConfigureAwait(false);
        private static async Task ToggleMute() => await Pi.Audio.ToggleMute(!Mute).ConfigureAwait(false);
    }
}
