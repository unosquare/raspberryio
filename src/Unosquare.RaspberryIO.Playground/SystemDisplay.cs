namespace Unosquare.RaspberryIO.Playground
{
    using Swan;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;

    public static class SystemDisplay
    {
        private static bool exit = false;
        public static async Task ShowMenu()
        {
            exit = false;
            ConsoleKey key;

            while (!exit)
            {
                Console.Clear();

                Console.WriteLine($"\rBrightness: {Pi.PiDisplay.Brightness}");
                Console.Write($"\rBlacklight: [");
                Console.ForegroundColor = Pi.PiDisplay.IsBacklightOn ? ConsoleColor.Green : ConsoleColor.Red;
                Console.Write($"{(Pi.PiDisplay.IsBacklightOn ? (char)0x2714 : (char)0x2718)}");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"]\n");

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.B:
                        SetBrightness();
                        break;
                    case ConsoleKey.L:
                        ToggleBlackLight();
                        break;
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                    default:
                        break;
                }
            }
        }

        public static void SetBrightness()
        {
            Console.Clear();
            Console.WriteLine($"Set a new brightness value [0 - 255]");
            var brightValue = byte.TryParse(Console.ReadLine(), out var brightness);
            if (brightValue)
            {
                if (brightness == Pi.PiDisplay.Brightness)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Set a different brightness value.");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    return;
                }
                else if (brightness > 255 || brightness < 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Valid brightness values are between [0 - 255]");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    return;
                }

                Console.WriteLine($"The new brightness value is {brightness}");
                Pi.PiDisplay.Brightness = brightness;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed to set brightness value.");
                Console.ForegroundColor = ConsoleColor.Gray;
                return;
            }
        }

        public static void ToggleBlackLight() =>
            Pi.PiDisplay.IsBacklightOn = !Pi.PiDisplay.IsBacklightOn;
    }
}
