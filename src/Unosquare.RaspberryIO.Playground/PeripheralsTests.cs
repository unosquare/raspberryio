namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Unosquare.Swan;

    public static class PeripheralsTests
    {

        private static readonly Dictionary<ConsoleKey, string> MainOptions = new Dictionary<ConsoleKey, string>
        {
            { ConsoleKey.R, "Rfid Controller" },
        };

        public static async Task ShowMenu()
        {
            var exit = false;
            bool pressKey = false;

            do
            {
                Console.Clear();
                pressKey = true;

                var mainOption = "System".ReadPrompt(MainOptions, "Esc to exit this menu");

                switch (mainOption. Key)
                {
                    case ConsoleKey.R:
                        //call the TestRfidController(); here
                        break;
                }



            } while (!exit);
        }
    }
}
