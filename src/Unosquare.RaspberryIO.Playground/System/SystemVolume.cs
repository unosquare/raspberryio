namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Threading.Tasks;
    using Swan;

    public static class SystemVolume
    {
        private static bool _mute;
        private static int CurrentLevel { get; set; }

        /// <summary>
        /// This shows how these commands can be used asynchronously, even though that is quite pointless for the demonstration application. 
        /// </summary>
        public static async Task ShowMenu()
        {
            var exit = false;

            while (!exit)
            {
                var state = await Pi.Audio.GetState().ConfigureAwait(false);
                CurrentLevel = state.Level;

                Terminal.Clear();
                Terminal.WriteLine($"\rControl name: {state.ControlName}");
                Terminal.WriteLine($"\rCard number: {state.CardNumber}");
                Terminal.WriteLine($"\rMute: [{(state.IsMute ? (char)0x2714 : (char)0x2718)}]\n");
                Terminal.WriteLine($"\rVolume level (dB): {state.Decibels}dB");

                UpdateProgress(CurrentLevel);

                Terminal.WriteLine("Press UpArrow key to increment volume");
                Terminal.WriteLine("Press DownArrow key to decrement volume");
                Terminal.WriteLine("Press M key to Mute on/off\n");
                var key = Terminal.ReadKey(true).Key;
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
                        key = Terminal.ReadKey(true).Key;
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
