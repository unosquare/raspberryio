namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Collections.Generic;
    using Swan;

    public static class ExtraExamples
    {
        private static readonly Dictionary<ConsoleKey, string> MainOptions = new Dictionary<ConsoleKey, string>
        {
            { ConsoleKey.B, "Test Button" },
            { ConsoleKey.L, "Led Blinking" },
            { ConsoleKey.D, "Led Dimming" },
        };

        public static void ShowMenu()
        {
            var exit = false;

            do
            {
                Console.Clear();
                var mainOption = "Extra Examples".ReadPrompt(MainOptions, "Esc to exit this menu");

                switch (mainOption.Key)
                {
                    case ConsoleKey.B:
                        Program.TestButton();
                        break;
                    case ConsoleKey.L:
                        Program.TestLedBlinking();
                        break;
                    case ConsoleKey.D:
                        Program.TestLedDimming();
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
