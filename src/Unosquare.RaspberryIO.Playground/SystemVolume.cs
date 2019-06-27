namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    public static class SystemVolume
    {
        private static bool Mute { get; set; } = false;
        private static string Progress { get; set; }

        public static async Task ShowMenu()
        {
            Console.Clear();
            var state = await Pi.Audio.GetState().ConfigureAwait(false);

            // info
            Console.WriteLine($"Control name: {state.ControlName}");
            Console.WriteLine($"Card number: {state.CardNumber}");
            Console.WriteLine($"Volume level (dB): {state.Decibels}dB");
            Console.WriteLine($"Mute: {state.IsMute}");

            UpdateProgress(state.Level);
        }

        private static void UpdateProgress(int level)
        {
            Console.Write($"[");
            var progress = new StringBuilder();
            var filler = Math.Floor((float)level / 10);

            // var emptier = Math.Floor((float)(100 - level) / 10);
            for (int i = 0; i < filler; ++i)
            {
                progress.Append(char.ConvertFromUtf32(0x2588));
                if (i < 7)
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (i < 9)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{progress.ToString()}");
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            // Progress bar here
            Console.Write($"] {level}%\n");
        }

        private static async Task IncrementVolume() => await Pi.Audio.IncrementVolume(1.00f).ConfigureAwait(false);
        private static async Task DecrementVolume() => await Pi.Audio.IncrementVolume(-1.00f).ConfigureAwait(false);
        private static async Task ToggleMute() => await Pi.Audio.ToggleMute(!Mute).ConfigureAwait(false);
    }
}
