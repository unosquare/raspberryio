namespace Unosquare.RaspberryIO.Playground
{
    using Abstractions;
    using Camera;
    using Peripherals;
    using Swan;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
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
                        PeripheralsTests.ShowMenu();
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

        /// <summary>
        /// Tests the servo.
        /// </summary>
        // public static void TestServo()
        // {
        //    var servo = new HardwareServo((GpioPin)Pi.Gpio[BcmPin.Gpio18]);
        //    const double minPulse = 0.565;
        //    const double maxPulse = 2.620;
        //    var deltaPulse = 0.005;
        //
        //    while (true)
        //    {
        //        if (servo.PulseLengthMs >= maxPulse || servo.PulseLengthMs <= minPulse)
        //        {
        //            var stopPulseLength = servo.PulseLengthMs;
        //            while (true)
        //            {
        //                var k = "Q (increment), W (decrement) or E (scroll back)".ReadKey();
        //                if (k.Key == ConsoleKey.Q)
        //                {
        //                    servo.PulseLengthMs += Math.Abs(deltaPulse);
        //                    $"{servo}".Info("Servo");
        //                    var angle = servo.ComputeAngle(minPulse, maxPulse);
        //                    var pulseLength = servo.ComputePulseLength(angle, minPulse, maxPulse);
        //                    $"Angle is {angle,7:0.000}. Pulse Length Should be: {pulseLength,7:0.000}".Warn("Servo");
        //                }
        //                else if (k.Key == ConsoleKey.W)
        //                {
        //                    servo.PulseLengthMs -= Math.Abs(deltaPulse);
        //                    $"{servo}".Info("Servo");
        //                    var angle = servo.ComputeAngle(minPulse, maxPulse);
        //                    var pulseLength = servo.ComputePulseLength(angle, minPulse, maxPulse);
        //                    $"Angle is {angle,7:0.000}. Pulse Length Should be: {pulseLength,7:0.000}".Warn("Servo");
        //                }
        //                else if (k.Key == ConsoleKey.E)
        //                {
        //                    servo.PulseLengthMs = stopPulseLength;
        //                    $"{servo}".Info("Servo");
        //                    break;
        //                }
        //            }
        //
        //            deltaPulse *= -1;
        //            Thread.Sleep(100);
        //        }

        // servo.PulseLengthMs += deltaPulse;
        //        $"{servo} | Angle {servo.ComputeAngle(minPulse, maxPulse),7:0.00}".Info("Servo");
        //        Pi.Timing.SleepMicroseconds(1500);
        //    }
        // }

        /// <summary>
        /// Tests the SPI bus functionality.
        /// </summary>
        public static void TestSpi()
        {
            Pi.Spi.Channel0Frequency = SpiChannel.MinFrequency;

            var request = Encoding.UTF8.GetBytes("Hello over SPI");
            $"SPI Request: {BitConverter.ToString(request)}".Info();
            var response = Pi.Spi.Channel0.SendReceive(request);
            $"SPI Response: {BitConverter.ToString(response)}".Info();

            $"SPI Base Stream Request: {BitConverter.ToString(request)}".Info();
            Pi.Spi.Channel0.Write(request);
            response = Pi.Spi.Channel0.SendReceive(new byte[request.Length]);
            $"SPI Base Stream Response: {BitConverter.ToString(response)}".Info();
        }

        /// <summary>
        /// Tests the display.
        /// </summary>
        public static void TestDisplay()
        {
            var input = string.Empty;

            while (input.Equals("x") == false)
            {
                "Enter brightness value (0 to 255). Enter b to toggle Backlight, Enter x to Exit".Info();
                input = Console.ReadLine();

                if (input?.Equals("b") == true)
                {
                    Pi.PiDisplay.IsBacklightOn = !Pi.PiDisplay.IsBacklightOn;
                }
                else if (byte.TryParse(input, out var value) && value != Pi.PiDisplay.Brightness)
                {
                    $"Current Value: {Pi.PiDisplay.Brightness}, New Value: {value}".Info();
                    Pi.PiDisplay.Brightness = value;
                }

                $"Display Status - Backlight: {Pi.PiDisplay.IsBacklightOn}, Brightness: {Pi.PiDisplay.Brightness}"
                    .Info();
            }

            Pi.PiDisplay.IsBacklightOn = true;
            Pi.PiDisplay.Brightness = 96;
            $"Display Status - Backlight: {Pi.PiDisplay.IsBacklightOn}, Brightness: {Pi.PiDisplay.Brightness}".Info();
        }

        /// <summary>
        /// Test volume control.
        /// </summary>
        /// <returns> Performs an audio task. </returns>
        public static async Task TestVolumeControl()
        {
            Console.WriteLine("Volume control for Pi - Playground");

            await Pi.Audio.SetVolumePercentage(85).ConfigureAwait(false);
            await Pi.Audio.SetVolumeByDecibels(-1.00f).ConfigureAwait(false);
            await Pi.Audio.IncrementVolume(4.00f).ConfigureAwait(false);
            await Pi.Audio.IncrementVolume(4.00f).ConfigureAwait(false);

            try
            {
                var currentState = await Pi.Audio.GetState(1).ConfigureAwait(false);
                Console.WriteLine(currentState);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                var currentState = await Pi.Audio.GetState(0, "Master").ConfigureAwait(false);
                Console.WriteLine(currentState);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                await Pi.Audio.IncrementVolume(4.32f, 1).ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                await Pi.Audio.IncrementVolume(2.06f, 0, "Master").ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void TestCaptureImage()
        {
            var pictureBytes = Pi.Camera.CaptureImageJpeg(640, 480);
            const string targetPath = "/home/pi/picture.jpg";

            if (File.Exists(targetPath))
                File.Delete(targetPath);

            File.WriteAllBytes(targetPath, pictureBytes);
            $"Took picture -- Byte count: {pictureBytes.Length}".Info();
        }

        private static void TestCaptureVideo()
        {
            // Setup our working variables
            var videoByteCount = 0;
            var videoEventCount = 0;
            var startTime = DateTime.UtcNow;

            // Configure video settings
            var videoSettings = new CameraVideoSettings
            {
                CaptureTimeoutMilliseconds = 0,
                CaptureDisplayPreview = false,
                ImageFlipVertically = true,
                CaptureExposure = CameraExposureMode.Night,
                CaptureWidth = 1920,
                CaptureHeight = 1080,
            };

            try
            {
                "Press any key to START reading the video stream . . .".Info();
                Console.ReadLine();

                // Start the video recording
                Pi.Camera.OpenVideoStream(videoSettings,
                    onDataCallback: data =>
                    {
                        videoByteCount += data.Length;
                        videoEventCount++;
                    },
                    onExitCallback: null);

                // Wait for user interaction
                startTime = DateTime.UtcNow;
                "Press any key to STOP reading the video stream . . .".Info();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                $"{ex.GetType()}: {ex.Message}".Error();
            }
            finally
            {
                // Always close the video stream to ensure raspivid quits
                Pi.Camera.CloseVideoStream();

                // Output the stats
                var megaBytesReceived = (videoByteCount / (1024f * 1024f)).ToString("0.000");
                var recordedSeconds = DateTime.UtcNow.Subtract(startTime).TotalSeconds.ToString("0.000");
                $"Capture Stopped. Received {megaBytesReceived} Mbytes in {videoEventCount} callbacks in {recordedSeconds} seconds"
                    .Info();
            }
        }
    }
}
