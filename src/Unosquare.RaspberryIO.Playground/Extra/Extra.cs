namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Collections.Generic;
    using Swan;

    public static partial class Extra
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
                        TestButton();
                        break;
                    case ConsoleKey.L:
                        TestLedBlinking();
                        break;
                    case ConsoleKey.D:
                        TestLedDimming();
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
