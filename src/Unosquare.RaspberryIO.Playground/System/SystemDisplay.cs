namespace Unosquare.RaspberryIO.Playground
{
    using Swan;
    using Swan.Logging;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Manipulates settings for the default 7" Raspberry display. Will probably not work with displays connected trough HDMI. 
    /// </summary>
    public static class SystemDisplay
    {
        private static readonly Dictionary<ConsoleKey, string> ScreenOptions = new Dictionary<ConsoleKey, string>
        {
            { ConsoleKey.B, "Set Brightness" },
            { ConsoleKey.L, "Toggle backlight" },
        };
        public static void ShowMenu()
        {
            var exit = false;

            while (!exit)
            {
                Terminal.Clear();
                Terminal.WriteLine($"Brightness: {Pi.PiDisplay.Brightness}");
                Terminal.WriteLine($"Backlight: [{(Pi.PiDisplay.IsBacklightOn ? (char)0x2714 : (char)0x2718)}]\n");
                var mainOption = Terminal.ReadPrompt("System", ScreenOptions, "Esc to exit this menu");

                var key = mainOption.Key;

                switch (key)
                {
                    case ConsoleKey.B:
                        SetBrightness();
                        break;
                    case ConsoleKey.L:
                        ToggleBackLight();
                        break;
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                }
            }
        }

        public static void SetBrightness()
        {
            Console.Clear();
            Console.WriteLine("Set a new brightness value [0 - 255]");
            var brightValue = byte.TryParse(Console.ReadLine(), out var brightness);

            if (!brightValue)
            {
                "Failed to set brightness value.".Error();
                return;
            }

            if (brightness == Pi.PiDisplay.Brightness)
            {
                "Set a different brightness value.".Error();
                return;
            }

            if (brightness > 255)
            {
                "Valid brightness values are between [0 - 255]".Error();
                return;
            }

            $"The new brightness value is {brightness}".Info();
            Pi.PiDisplay.Brightness = brightness;
        }

        public static void ToggleBackLight() =>
            Pi.PiDisplay.IsBacklightOn = !Pi.PiDisplay.IsBacklightOn;
    }
}
