namespace Unosquare.RaspberryIO.Playground.Extra
{
    using Abstractions;
    using Swan;
    using Swan.Logging;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using WiringPi;

    public static partial class Extra
    {
        public static void TestLedBlinking()
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                var task = Blink(cancellationTokenSource.Token);

                while (true)
                {
                    var input = Console.ReadKey(true).Key;

                    if (input != ConsoleKey.Escape)
                        continue;
                    cancellationTokenSource.Cancel();
                    break;
                }

                task.Wait();
            }
        }

        public static void TestLedDimming(bool hardware)
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                Task task = null;
                if (hardware)
                {
                    task = DimHardware(cancellationTokenSource.Token);
                }
                else
                {
                    task = DimSoftware(cancellationTokenSource.Token);
                }

                while (true)
                {
                    var input = Console.ReadKey(true).Key;

                    if (input != ConsoleKey.Escape)
                        continue;
                    cancellationTokenSource.Cancel();
                    break;
                }
                task.Wait();
            }
        }

        /// <summary>
        /// For this test, connect an LED to Gpio13 and ground. (don't forget the resistor!)
        /// </summary>
        private static Task Blink(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
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
                    Terminal.WriteLine(ExitMessage);
                    Thread.Sleep(500);
                }

                blinkingPin.Write(0);
            });
        }

        private static Task DimHardware(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Console.Clear();
                "Hardware Dimming".Info();
                Terminal.WriteLine(ExitMessage);

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

        /// <summary>
        /// For this test, connect a two-color LED to Gpio23, Gpio24 and ground. Tested with the KY-011 Module. (don't forget the resistor!)
        /// </summary>
        private static Task DimSoftware(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Console.Clear();
                "Dimming".Info();
                Terminal.WriteLine(ExitMessage);

                var pinGreen = (GpioPin)Pi.Gpio[BcmPin.Gpio23];
                var pinRed = (GpioPin)Pi.Gpio[BcmPin.Gpio24];

                pinGreen.PinMode = GpioPinDriveMode.Output;
                pinGreen.StartSoftPwm(0, 100);
                pinRed.PinMode = GpioPinDriveMode.Output;
                pinRed.StartSoftPwm(0, 100);

                bool redOn = false;

                while (!cancellationToken.IsCancellationRequested)
                {
                    GpioPin pin = null;
                    if (redOn)
                    {
                        pin = pinRed;
                    }
                    else
                    {
                        pin = pinGreen;
                    }
                    redOn = !redOn;

                    for (var x = 0; x <= 100; x++)
                    {
                        pin.SoftPwmValue = (int)pinGreen.SoftPwmRange / 100 * x;
                        Thread.Sleep(10);
                    }

                    for (var x = 0; x <= 100; x++)
                    {
                        pin.SoftPwmValue = (int)pinGreen.SoftPwmRange - ((int)pinGreen.SoftPwmRange / 100 * x);
                        Thread.Sleep(10);
                    }
                }

                pinGreen.Write(0);
                pinRed.Write(0);
                Terminal.WriteLine("End of task");
            });
        }
    }
}
