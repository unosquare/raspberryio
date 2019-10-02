namespace Unosquare.RaspberryIO.Playground
{
    using Swan;
    using Swan.Logging;
    using System;
    using System.Collections.Generic;
    using WiringPi;

    /// <summary>
    /// Main entry point class.
    /// </summary>
    public static partial class Program
    {
        private static readonly Dictionary<ConsoleKey, string> MainOptions = new Dictionary<ConsoleKey, string>
        {
            // Module Control Items
            { ConsoleKey.S, "System" },
            { ConsoleKey.P, "Peripherals" },
            { ConsoleKey.X, "Extra examples" },
        };

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        public static void Main()
        {
            // We shouldn't be logging to the console in a console app that is user-interactive
            Swan.Logging.Logger.UnregisterLogger<ConsoleLogger>();
            Pi.Init<BootstrapWiringPi>();

            var exit = false;
            do
            {
                Console.Clear();
                Console.CursorVisible = true;
                var mainOption = Terminal.ReadPrompt("Main options", MainOptions, "Esc to exit this program");
                Console.CursorVisible = true;
                switch (mainOption.Key)
                {
                    case ConsoleKey.S:
                        SystemTests.ShowMenu();
                        break;
                    case ConsoleKey.P:
                        Peripherals.Peripherals.ShowMenu();
                        break;
                    case ConsoleKey.X:
                        Extra.Extra.ShowMenu();
                        break;                        
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                }
            }
            while (!exit);

            Console.Clear();
            Console.CursorVisible = true;
            Console.ResetColor();
        }
    }
}
