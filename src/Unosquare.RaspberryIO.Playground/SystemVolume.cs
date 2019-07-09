namespace Unosquare.RaspberryIO.Playground
{
    using Swan;
    using System;
    using System.Threading.Tasks;

    public static class SystemVolume
    {
        private static bool exit = false;
        private static bool mute = false;
        private static int CurrentLevel { get; set; }

        public static async Task ShowMenu()
        {
            exit = false;
            ConsoleKey key;

            while (!exit)
            {
                Console.Clear();
                var state = await Pi.Audio.GetState().ConfigureAwait(false);
                CurrentLevel = state.Level;

                $"\rControl name: {state.ControlName}".Info();
                $"\rCard number: {state.CardNumber}".Info();
                $"\rMute: [{(state.IsMute ? (char)0x2714 : (char)0x2718)}]\n".Info();
                $"\rVolume level (dB): {state.Decibels}dB".Info();

                Console.Write($"\r[");
                UpdateProgress(CurrentLevel);
                Console.Write($"] {CurrentLevel}%\n");

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.DownArrow:
                        //await DecrementVolume().ConfigureAwait(false);
                        await ChangeVolume(-1).ConfigureAwait(false);
                        break;
                    case ConsoleKey.UpArrow:
                        await ChangeVolume(1).ConfigureAwait(false);
                       // await IncrementVolume().ConfigureAwait(false);
                        break;
                    case ConsoleKey.M:
                        mute = !mute;
                        await ToggleMute().ConfigureAwait(false);
                        break;
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private static void UpdateProgress(int level)
        {
            var progress = new string((char)0x275A, 10);
            var filler = level / 10;
            var emptier = 10 - filler;

            for (int i = 0; i < filler; ++i)
            {
                if (i < 6)
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (i < 9)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.Write($"{progress[i]}");
            }

            for (int i = 0; i < emptier; ++i)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($"{progress[filler + i]}");
            }

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static async Task IncrementVolume() => await Pi.Audio.SetVolumePercentage(CurrentLevel + 1).ConfigureAwait(false);
        private static async Task DecrementVolume() => await Pi.Audio.SetVolumePercentage(CurrentLevel - 1).ConfigureAwait(false);

        private static async Task ChangeVolume(int percentage) =>
            await Pi.Audio.SetVolumePercentage(CurrentLevel + percentage).ConfigureAwait(false);
        private static async Task ToggleMute() => await Pi.Audio.ToggleMute(mute).ConfigureAwait(false);
    }
}
