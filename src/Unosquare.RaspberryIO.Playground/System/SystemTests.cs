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
            { ConsoleKey.D, "Display Brightness" },
            { ConsoleKey.I, "System Info" },
            { ConsoleKey.V, "Volume" },
        };

        public static void ShowMenu()
        {
            var exit = false;

            do
            {
                Terminal.Clear();
                var mainOption = Terminal.ReadPrompt("System", MainOptions, "Esc to exit this menu");

                switch (mainOption.Key)
                {
                    case ConsoleKey.C:
                        SystemCamera.ShowMenu();
                        break;
                    case ConsoleKey.I:
                        TestSystemInfo();
                        break;
                    case ConsoleKey.V:
                        Task ret = SystemVolume.ShowMenu();
                        ret.Wait();
                        break;
                    case ConsoleKey.D:
                        SystemDisplay.ShowMenu();
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
