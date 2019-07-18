namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Swan;

    public static partial class SystemTests
    {
        private const string ExitMessage = "Press Esc key to continue . . .";

        private static readonly Dictionary<ConsoleKey, string> MainOptions = new Dictionary<ConsoleKey, string>
        {
            { ConsoleKey.C, "Camera" },
            { ConsoleKey.I, "System Info" },
            { ConsoleKey.V, "Volume" },
        };

        public static async Task ShowMenu()
        {
            var exit = false;

            do
            {
                Console.Clear();
                var mainOption = "System".ReadPrompt(MainOptions, "Esc to exit this menu");

                switch (mainOption.Key)
                {
                    case ConsoleKey.C:
                        SystemCamera.ShowMenu();
                        break;
                    case ConsoleKey.I:
                        await TestSystemInfo().ConfigureAwait(false);
                        break;
                    case ConsoleKey.V:
                        await SystemVolume.ShowMenu().ConfigureAwait(false);
                        break;
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                }
            }
            while (!exit);
        }
    }
}
