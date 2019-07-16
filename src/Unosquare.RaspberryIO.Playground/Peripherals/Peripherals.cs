﻿namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Collections.Generic;
    using Swan;

    public static partial class Peripherals
    {
        private static readonly Dictionary<ConsoleKey, string> MainOptions = new Dictionary<ConsoleKey, string>
        {
            { ConsoleKey.A, "Accelerometer" },
            { ConsoleKey.I, "Infrared Sensor" },
            { ConsoleKey.R, "Rfid Controller" },
            { ConsoleKey.U, "Ultrasonic Sensor" },
            { ConsoleKey.T, "Temperature and Humidity Sensor" },
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
                        TestAccelerometer();
                        break;
                    case ConsoleKey.I:
                        TestInfraredSensor();
                        break;
                    case ConsoleKey.R:
                        ShowRfidMenu();
                        break;
                    case ConsoleKey.U:
                        TestUltrasonicSensor();
                        break;
                    case ConsoleKey.T:
                        TestTempSensor();
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
