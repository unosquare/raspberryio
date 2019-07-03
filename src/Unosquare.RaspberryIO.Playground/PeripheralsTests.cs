namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Unosquare.Swan;

    public static class PeripheralsTests
    {
        private static readonly Dictionary<ConsoleKey, string> MainOptions = new Dictionary<ConsoleKey, string>
        {
            { ConsoleKey.A, "Accelerometer"},
            { ConsoleKey.I, "Infrared Sensor"},
            { ConsoleKey.R, "Rfid Controller" },
            { ConsoleKey.U, "Ultrasonic Sensor"},
            { ConsoleKey.T, "Temperature and Humidity Sensor"},
        };

        public static void ShowMenu()
        {
            var exit = false;

            do
            {
                Console.Clear();
                var mainOption = "Peripherals".ReadPrompt(MainOptions, "Esc to exit this menu");

                switch (mainOption.Key)
                {
                    case ConsoleKey.A:
                        Program.TestAccelerometer();
                        break;
                    case ConsoleKey.I:
                        Program.TestInfraredSensor();
                        break;
                    case ConsoleKey.R:
                        Program.ShowRfidMenu();
                        break;
                    case ConsoleKey.U:
                        Program.TestUltrasonicSensor();
                        break;
                    case ConsoleKey.T:
                        Program.TestTempSensor();
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
