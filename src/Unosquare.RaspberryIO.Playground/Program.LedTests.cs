namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Unosquare.RaspberryIO.Abstractions;
    using Unosquare.Swan;
    using Unosquare.WiringPi;

    public partial class Program
    {
        public static void TestLedBlinking()
        {
            Console.Clear();

            // Get a reference to the pin you need to use.
            // All methods below are exactly equivalent and reference the same pin
            var blinkingPin = Pi.Gpio[16];
            blinkingPin = Pi.Gpio[BcmPin.Gpio26];
            blinkingPin = Pi.Gpio[P1.Pin37];
            blinkingPin = ((GpioController)Pi.Gpio)[WiringPiPin.Pin26];
            blinkingPin = ((GpioController)Pi.Gpio).Pin26;
            blinkingPin = ((GpioController)Pi.Gpio).HeaderP1[37];

            // Configure the pin as an output
            blinkingPin.PinMode = GpioPinDriveMode.Output;

            // perform writes to the pin by toggling the isOn variable
            var isOn = false;
            for (var i = 0; i < 20; i++)
            {
                isOn = !isOn;
                blinkingPin.Write(isOn);
                var ledState = isOn ? "on" : "off";
                Console.Clear();
                $"Blinking {ledState}".Info();
                Thread.Sleep(500);
            }

            "Press any key to continue . . .".ReadKey(true);
        }

        public static async Task TestLedDimming()
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                Dimming(cancellationTokenSource.Token);

                while (true)
                {
                    var input = Console.ReadKey(false).Key;

                    if (input == ConsoleKey.Escape)
                    {
                        cancellationTokenSource.Cancel();
                        break;
                    }
                    //KeyboardListener(cancellationTokenSource);
                }
            }
        }

        private static void Dimming(CancellationToken cancellationToken)
            => Task.Run(() =>
            {
                Console.Clear();
                "Dimming".Info();
                "Press any key to continue . . .".WriteLine();
                var pin = (GpioPin)Pi.Gpio[BcmPin.Gpio13];
                pin.PinMode = GpioPinDriveMode.PwmOutput;
                pin.PwmMode = PwmMode.Balanced;
                pin.PwmClockDivisor = 2;
                while (!cancellationToken.IsCancellationRequested)
                {
                    for (var x = 0; x <= 100; x++)
                    {
                        pin.PwmRegister = (int)pin.PwmRange / 100 * x;
                        Thread.Sleep(10);
                    }

                    for (var x = 0; x <= 100; x++)
                    {
                        pin.PwmRegister = (int)pin.PwmRange - ((int)pin.PwmRange / 100 * x);
                        Thread.Sleep(10);
                    }
                }

                pin.PinMode = GpioPinDriveMode.Output;
                pin.Write(0);
            });

        private static void KeyboardListener(CancellationTokenSource cancellationTokenSource)
            => Task.Run(() =>
            {
                var input = Console.ReadKey().Key;
                if (input == ConsoleKey.Escape)
                {
                    cancellationTokenSource.Cancel();
                    return;
                }
            });
    }
}
