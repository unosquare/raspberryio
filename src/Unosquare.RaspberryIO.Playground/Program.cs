namespace Unosquare.RaspberryIO.Playground
{
    using Samples;
    using System;
    using System.IO;
    using System.Threading;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"Starting program at {DateTime.Now}");
            try
            {
                //TestSystemInfo();
                TestLedStripGraphics();
                //TestLedStrip();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType()} {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Console.WriteLine("Program finished.");
            }
        }

        public static void TestLedStripGraphics()
        {
            var bitmap = new System.Drawing.Bitmap("fractal.jpg");
            Console.WriteLine($"Loaded bitmap with format {bitmap.PixelFormat}");

            var exitAnimation = false;

            var thread = new Thread(() =>
            {
                var strip = new LedStrip(60 * 4);
                var millisecondsPerFrame = 1000 / 24;
                var lastRenderTime = DateTime.UtcNow;
                var currentRow = 0;
                var currentDirection = 1;

                while (!exitAnimation)
                {
                    strip.ClearPixels();

                    strip.SetPixels(0, currentRow, bitmap, 0.1f);
                    currentRow += currentDirection;
                    if (currentRow >= bitmap.Height)
                    {
                        currentRow = bitmap.Height - 2;
                        currentDirection = -1;
                    }

                    if (currentRow <= 0)
                    {
                        currentRow = 1;
                        currentDirection = 1;
                    }

                    var delayMilliseconds = (int)DateTime.UtcNow.Subtract(lastRenderTime).TotalMilliseconds;
                    delayMilliseconds = millisecondsPerFrame - delayMilliseconds;
                    if (delayMilliseconds > 0 && exitAnimation == false)
                        Thread.Sleep(delayMilliseconds);
                    else
                        Console.WriteLine($"Lagging framerate: {delayMilliseconds} milliseconds");


                    lastRenderTime = DateTime.UtcNow;
                    strip.Render();
                }

                strip.ClearPixels();
                strip.Render();

            });

            thread.Start();
            Console.Write("Press any key to stop and clear");
            Console.ReadKey(true);
            Console.WriteLine();
            exitAnimation = true;

        }

        public static void TestLedStrip()
        {
            var exitAnimation = false;

            var thread = new Thread(() =>
            {
                var strip = new LedStrip(60 * 4);
                var millisecondsPerFrame = 1000 / 25;
                var lastRenderTime = DateTime.UtcNow;

                var tailSize = strip.LedCount;
                byte red = 0;

                while (!exitAnimation)
                {
                    strip.ClearPixels();

                    red = red >= 254 ? default(byte) : (byte)(red + 1);

                    for (int i = 0; i < tailSize; i++)
                    {
                        strip[i].Brightness = i / (tailSize - 1f);
                        strip[i].R = red;
                        strip[i].G = (byte)(255 - red);
                        strip[i].B = (byte)(strip[i].Brightness * 254);
                    }

                    var delayMilliseconds = (int)DateTime.UtcNow.Subtract(lastRenderTime).TotalMilliseconds;
                    delayMilliseconds = millisecondsPerFrame - delayMilliseconds;
                    if (delayMilliseconds > 0 && exitAnimation == false)
                    {
                        Thread.Sleep(delayMilliseconds);
                    }
                    else
                    {
                        Console.WriteLine($"Lagging framerate: {delayMilliseconds} milliseconds");
                    }


                    lastRenderTime = DateTime.UtcNow;
                    strip.Render();
                }

                strip.ClearPixels();
                strip.Render();

            });

            thread.Start();
            Console.Write("Press any key to stop and clear");
            Console.ReadKey(true);
            Console.WriteLine();
            exitAnimation = true;
        }

        public static void TestSpi()
        {
            Pi.Spi.Channel0Frequency = SpiChannel.MinFrequency;

            var request = System.Text.Encoding.UTF8.GetBytes("Hello over SPI");
            Console.WriteLine($"SPI Request: {BitConverter.ToString(request)}");
            var response = Pi.Spi.Channel0.SendReceive(request);
            Console.WriteLine($"SPI Response: {BitConverter.ToString(response)}");

            Console.WriteLine($"SPI Base Stream Request: {BitConverter.ToString(request)}");
            Pi.Spi.Channel0.Write(request);
            response = Pi.Spi.Channel0.SendReceive(new byte[request.Length]);
            Console.WriteLine($"SPI Base Stream Response: {BitConverter.ToString(response)}");

        }

        public static void TestDisplay()
        {
            string input = string.Empty;

            while (input.Equals("x") == false)
            {
                Console.WriteLine("Enter brightness value (0 to 255). Enter b to toggle Backlight, Enter x to Exit");
                input = Console.ReadLine();

                if (input.Equals("b"))
                {
                    Pi.Display.IsBacklightOn = !Pi.Display.IsBacklightOn;
                }
                else
                {
                    byte value = 128;
                    if (byte.TryParse(input, out value))
                    {
                        if (value != Pi.Display.Brightness)
                        {
                            Console.WriteLine($"Current Value: {Pi.Display.Brightness}, New Value: {value}");
                            Pi.Display.Brightness = value;
                        }
                    }
                }

                Console.WriteLine($"Display Status - Backlight: {Pi.Display.IsBacklightOn}, Brightness: {Pi.Display.Brightness}");
            }

            Pi.Display.IsBacklightOn = true;
            Pi.Display.Brightness = 96;
            Console.WriteLine($"Display Status - Backlight: {Pi.Display.IsBacklightOn}, Brightness: {Pi.Display.Brightness}");
        }

        public static void TestLedBlinking()
        {
            // Get a reference to the pin you need to use.
            // All 3 methods below are exactly equivalente
            var blinkingPin = Pi.Gpio[0];
            blinkingPin = Pi.Gpio[WiringPiPin.Pin00];
            blinkingPin = Pi.Gpio.Pin00;

            // Configure the pin as an output
            blinkingPin.PinMode = GpioPinDriveMode.Output;

            // perform writes to the pin by toggling the isOn variable
            var isOn = false;
            for (var i = 0; i < 20; i++)
            {
                isOn = !isOn;
                blinkingPin.Write(isOn);
                System.Threading.Thread.Sleep(500);
            }
        }

        private static void TestSystemInfo()
        {
            Console.WriteLine($"GPIO Controller initialized successfully with {Pi.Gpio.Count} pins");
            Console.WriteLine($"{Pi.Info}");
            Console.WriteLine($"Microseconds Since GPIO Setup: {Pi.Timing.MicrosecondsSinceSetup}");
        }

        private static void TestCaptureImage()
        {
            var pictureBytes = Pi.Camera.CaptureImageJpeg(640, 480);
            var targetPath = "/home/pi/picture.jpg";
            if (File.Exists(targetPath))
                File.Delete(targetPath);

            File.WriteAllBytes(targetPath, pictureBytes);
            Console.WriteLine($"Took picture -- Byte count: {pictureBytes.Length}");
        }

        static void TestCaptureVideo()
        {
            // Setup our working variables
            var videoByteCount = 0;
            var videoEventCount = 0;
            var startTime = DateTime.UtcNow;

            // Configure video settings
            var videoSettings = new CameraVideoSettings()
            {
                CaptureTimeoutMilliseconds = 0,
                CaptureDisplayPreview = false,
                ImageFlipVertically = true,
                CaptureExposure = CameraExposureMode.Night,
                CaptureWidth = 1920,
                CaptureHeight = 1080
            };

            try
            {
                // Start the video recording
                Pi.Camera.OpenVideoStream(videoSettings,
                    onDataCallback: (data) => { videoByteCount += data.Length; videoEventCount++; },
                    onExitCallback: null);

                // Wait for user interaction
                startTime = DateTime.UtcNow;
                Console.WriteLine("Press any key to stop reading the video stream . . .");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType()}: {ex.Message}");
            }
            finally
            {
                // Always close the video stream to ensure raspivid quits
                Pi.Camera.CloseVideoStream();

                // Output the stats
                var megaBytesReceived = (videoByteCount / (1024f * 1024f)).ToString("0.000");
                var recordedSeconds = DateTime.UtcNow.Subtract(startTime).TotalSeconds.ToString("0.000");
                Console.WriteLine($"Capture Stopped. Received {megaBytesReceived} Mbytes in {videoEventCount} callbacks in {recordedSeconds} seconds");
            }
        }

        static void TestColors()
        {
            var colors = new Color[]
            {
                Color.Black,
                Color.White,
                Color.Red,
                Color.Green,
                Color.Blue
            };

            foreach (var color in colors)
            {
                Console.WriteLine($"{color.Name,-15}: RGB Hex: {color.ToRgbHex(false)}    YUV Hex: {color.ToYuvHex(true)}");
            }
        }
    }
}
