namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Swan;
    using WiringPi;

    public static partial class Extra
    {
        public static void TestLedBlinking()
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                Blink(cancellationTokenSource.Token);

                while (true)
                {
                    var input = Console.ReadKey(true).Key;

                    if (input != ConsoleKey.Escape) continue;
                    cancellationTokenSource.Cancel();
                    break;
                }
            }
        }

        public static void TestLedDimming()
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                Dim(cancellationTokenSource.Token);

                while (true)
                {
                    var input = Console.ReadKey(true).Key;

                    if (input != ConsoleKey.Escape) continue;
                    cancellationTokenSource.Cancel();
                    break;
                }
            }
        }

        private static void Blink(CancellationToken cancellationToken) =>
            Task.Run(() =>
            {
                Console.Clear();
                var blinkingPin = Pi.Gpio[BcmPin.Gpio13];

                // Configure the pin as an output
                blinkingPin.PinMode = GpioPinDriveMode.Output;

                // perform writes to the pin by toggling the isOn variable
                var isOn = false;
                while (!cancellationToken.IsCancellationRequested)
                {
                    isOn = !isOn;
                    blinkingPin.Write(isOn);
                    var ledState = isOn ? "on" : "off";
                    Console.Clear();
                    $"Blinking {ledState}".Info();
                    "Press Esc key to continue . . .".WriteLine();
                    Thread.Sleep(500);
                }

                blinkingPin.Write(0);
            });

        private static void Dim(CancellationToken cancellationToken) =>
            Task.Run(() =>
            {
                Console.Clear();
                "Dimming".Info();
                "Press Esc key to continue . . .".WriteLine();
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
    }
}
