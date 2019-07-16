namespace Unosquare.RaspberryIO.Playground
{
    using Swan;
    using System;
    using System.Threading.Tasks;

    public static class SystemDisplay
    {
        public static async Task ShowMenu()
        {
            var exit = false;
            ConsoleKey key;

            while (!exit)
            {
                Console.Clear();

                $"Brightness: {Pi.PiDisplay.Brightness}".Info();
                $"Blacklight: [{(Pi.PiDisplay.IsBacklightOn ? (char)0x2714 : (char)0x2718)}]\n".Info();

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
                }

                if (!exit)
                {
                    await Task.Delay(500).ConfigureAwait(false);
                    Console.WriteLine("Press any key to continue . . .");
                    Console.ReadKey(true);
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

            if (brightness > 255 || brightness < 0)
            {
                "Valid brightness values are between [0 - 255]".Error();
                return;
            }

            $"The new brightness value is {brightness}".Info();
            Pi.PiDisplay.Brightness = brightness;
        }

        public static void ToggleBlackLight() =>
            Pi.PiDisplay.IsBacklightOn = !Pi.PiDisplay.IsBacklightOn;
    }
}
