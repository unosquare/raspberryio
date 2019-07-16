namespace Unosquare.RaspberryIO.Playground
{
    using Swan;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
        /// <returns>A task representing the program.</returns>
        public static async Task Main()
        {
            $"Starting program at {DateTime.Now}".Info();

            Terminal.Settings.DisplayLoggingMessageType = LogMessageType.Info | LogMessageType.Warning | LogMessageType.Error;
            Pi.Init<BootstrapWiringPi>();

            var exit = false;
            do
            {
                Console.Clear();
                var mainOption = "Main options".ReadPrompt(MainOptions, "Esc to exit this program");

                switch (mainOption.Key)
                {
                    case ConsoleKey.S:
                        await SystemTests.ShowMenu().ConfigureAwait(false);
                        break;
                    case ConsoleKey.P:
                        Peripherals.ShowMenu();
                        break;
                    case ConsoleKey.X:
                        ExtraExamples.ShowMenu();
                        break;
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                }
            }
            while (!exit);

            Console.Clear();
        }
    }
}
