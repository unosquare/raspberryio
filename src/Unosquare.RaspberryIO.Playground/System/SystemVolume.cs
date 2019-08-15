namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Threading.Tasks;
    using Swan;
    using Swan.Logging;

    public static class SystemVolume
    {
        private static bool _mute;
        private static int CurrentLevel { get; set; }

        public static async Task ShowMenu()
        {
            var exit = false;

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

                Terminal.WriteLine("Press UpArrow key to increment volume");
                Terminal.WriteLine("Press DownArrow key to decrement volume");
                Terminal.WriteLine("Press M key to Mute on/off\n");
                var key = Terminal.ReadKey("Press Esc key to continue . . .").Key;
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
                            _mute = !_mute;
                            await Pi.Audio.ToggleMute(_mute).ConfigureAwait(false);
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
            var filler = level / 10;
            Terminal.Write("\r[");

            for (var i = 1; i <= 10; ++i)
            {
                ConsoleColor color;
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

                Terminal.Write($"{(char)0x275A}", color);
            }

            Terminal.Write($"] {level}%\n\n");
        }
    }
}
