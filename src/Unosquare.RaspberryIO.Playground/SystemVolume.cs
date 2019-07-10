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

            while (!exit)
            {
                var state = await Pi.Audio.GetState().ConfigureAwait(false);
                CurrentLevel = state.Level;

                Console.Clear();
                $"\rControl name: {state.ControlName}".Info();
                $"\rCard number: {state.CardNumber}".Info();
                $"\rMute: [{(state.IsMute ? (char)0x2714 : (char)0x2718)}]\n".Info();
                $"\rVolume level (dB): {state.Decibels}dB".Info();

                "\r[".Write();
                UpdateProgress(CurrentLevel); //show the current level
                $"] {CurrentLevel}%\n\n".Write();

                var key = "Press Esc key to continue . . .".ReadKey(true).Key;
                var validOption = false;

                while (!validOption)
                {
                    switch (key)
                    {
                        case ConsoleKey.DownArrow:
                            await DecrementVolume().ConfigureAwait(false);
                            validOption = true;
                            break;
                        case ConsoleKey.UpArrow:
                            await IncrementVolume().ConfigureAwait(false);
                            validOption = true;
                            break;
                        case ConsoleKey.M:
                            mute = !mute;
                            await ToggleMute().ConfigureAwait(false);
                            validOption = true;
                            break;
                        case ConsoleKey.Escape:
                            exit = true;
                            validOption = true;
                            break;
                    }

                    if (!validOption)
                        key = Console.ReadKey(true).Key;
                }
            }
        }

        private static void UpdateProgress(int level)
        {
            var progress = new string((char)0x275A, 10);
            var filler = level / 10;
            var emptier = 10 - filler;
            var color = ConsoleColor.Black;

            for (int i = 0; i < filler; ++i)
            {
                if (i < 6)
                    color = ConsoleColor.Green;
                else if (i < 9)
                    color = ConsoleColor.Yellow;
                else
                    color = ConsoleColor.Red;

                $"{progress[i]}".Write(color);
            }

            for (int i = 0; i < emptier; ++i)
            {
                $"{progress[filler + i]}".Write(ConsoleColor.Black);
            }

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static async Task IncrementVolume() => await Pi.Audio.SetVolumePercentage(CurrentLevel + 1).ConfigureAwait(false);
        private static async Task DecrementVolume() => await Pi.Audio.SetVolumePercentage(CurrentLevel - 1).ConfigureAwait(false);
        private static async Task ToggleMute() => await Pi.Audio.ToggleMute(mute).ConfigureAwait(false);
    }
}
