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

                UpdateProgress(CurrentLevel);

                var key = "Press Esc key to continue . . .".ReadKey(true).Key;
                var validOption = false;

                while (!validOption)
                {
                    switch (key)
                    {
                        case ConsoleKey.DownArrow:
                            await Pi.Audio.SetVolumePercentage(CurrentLevel - 1).ConfigureAwait(false);
                            validOption = true;
                            break;
                        case ConsoleKey.UpArrow:
                            await Pi.Audio.SetVolumePercentage(CurrentLevel + 1).ConfigureAwait(false);
                            validOption = true;
                            break;
                        case ConsoleKey.M:
                            mute = !mute;
                            await Pi.Audio.ToggleMute(mute).ConfigureAwait(false);
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
            $"{level}".Info();
            // var progress = new string((char)0x275A, 10);

            var renderCharacter = (char)0x275A;

            var filler = level / 10;

            $"{filler}".Info();
            // var emptier = 10 - filler;

            var color = ConsoleColor.Black;

            "\r[".Write();

            //for (int i = 0; i < filler; ++i)
            //{
            //    if (i < 6)
            //        color = ConsoleColor.Green;
            //    else if (i < 9)
            //        color = ConsoleColor.Yellow;
            //    else
            //        color = ConsoleColor.Red;
            //
            //    $"{renderCharacter}".Write(color);
            //}
            //
            //for (int i = 0; i < emptier; ++i)
            //{
            //    $"{renderCharacter}".Write(ConsoleColor.Black);
            //}

            for (int i = 1; i < 10; ++i)
            {
                if (i <= filler)
                {
                    if (i < 6)
                        color = ConsoleColor.Green;
                    else if (i < 9)
                        color = ConsoleColor.Yellow;
                    else
                        color = ConsoleColor.Red;
                }
                else
                {
                    color = ConsoleColor.Black;
                }

                $"{renderCharacter}".Write(color);
            }

            $"] {level}%\n\n".Write();
        }
    }
}
