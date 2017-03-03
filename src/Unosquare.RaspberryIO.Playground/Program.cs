namespace Unosquare.RaspberryIO.Playground
{
    using Camera;
    using Computer;
    using Gpio;
    using Samples;
    using Swan;
    using Swan.Formatters;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    public class Program
    {
        public static void Main(string[] args)
        {
            $"Starting program at {DateTime.Now}".Info();

            Terminal.Settings.DisplayLoggingMessageType = LogMessageType.Info;

            try
            {
                TestSystemInfo();
                //TestCaptureImage();
                //TestCaptureVideo();
                //TestLedStripGraphics();
                //TestLedStrip();
            }
            catch (Exception ex)
            {
                "Error".Error(nameof(Main), ex);
            }
            finally
            {
                "Program finished.".Info();
                Terminal.Flush(TimeSpan.FromSeconds(2));
                if (System.Diagnostics.Debugger.IsAttached)
                    "Press any key to exit . . .".ReadKey();
            }
        }

        public static void TestLedStripGraphics()
        {
            BitmapBuffer pixels = null;

            try
            {
                using (var bitmap = new System.Drawing.Bitmap(Path.Combine(Runtime.EntryAssemblyDirectory, "fractal.jpg")))
                {
                    $"Loaded bitmap with format {bitmap.PixelFormat}".Info();
                    pixels = new BitmapBuffer(bitmap);
                    $"Loaded Pixel Data: {pixels.Data.Length} bytes".Info();
                }
            }
            catch (Exception ex)
            {
                $"Error Loading image: {ex.Message}".Error();
            }

            var exitAnimation = false;
            var useDynamicBrightness = false;
            var frameRenderTimes = new Queue<int>();
            var frameTimes = new Queue<int>();

            var thread = new Thread(() =>
            {
                var strip = new LedStrip(60 * 4, 1, 1000000); // 1 Mhz is sufficient for such a short strip (only 240 LEDs)
                var millisecondsPerFrame = 1000 / 25;
                var lastRenderTime = DateTime.UtcNow;
                var currentFrameNumber = 0;

                var currentBrightness = 0.8f;
                var currentRow = 0;
                var currentDirection = 1;

                while (!exitAnimation)
                {
                    // Push pixels into the Frame Buffer
                    strip.SetPixels(pixels, 0, currentRow, currentBrightness);

                    // Move the current row slowly at FPS
                    currentRow += currentDirection;
                    if (currentRow >= pixels.ImageHeight)
                    {
                        currentRow = pixels.ImageHeight - 2;
                        currentDirection = -1;
                    }
                    else if (currentRow <= 0)
                    {
                        currentRow = 1;
                        currentDirection = 1;
                    }

                    if (useDynamicBrightness)
                        currentBrightness = 0.05f + 0.80f * (currentRow / (pixels.ImageHeight - 1f));

                    // Stats and sleep time
                    var delayMilliseconds = (int)DateTime.UtcNow.Subtract(lastRenderTime).TotalMilliseconds;
                    frameRenderTimes.Enqueue(delayMilliseconds);
                    delayMilliseconds = millisecondsPerFrame - delayMilliseconds;

                    if (delayMilliseconds > 0 && exitAnimation == false)
                        Thread.Sleep(delayMilliseconds);
                    else
                        $"Lagging framerate: {delayMilliseconds} milliseconds".Info();

                    frameTimes.Enqueue((int)DateTime.UtcNow.Subtract(lastRenderTime).TotalMilliseconds);
                    lastRenderTime = DateTime.UtcNow;

                    // Push the framebuffer to SPI
                    strip.Render();

                    if (currentFrameNumber == int.MaxValue)
                        currentFrameNumber = 0;
                    else
                        currentFrameNumber++;
                    if (frameRenderTimes.Count >= 2048) frameRenderTimes.Dequeue();
                    if (frameTimes.Count >= 20148) frameTimes.Dequeue();

                }

                strip.ClearPixels();
                strip.Render();

                var avg = frameRenderTimes.Average();
                $"Frames: {currentFrameNumber + 1}, FPS: {Math.Round((1000f / frameTimes.Average()), 3)}, Strip Render: {Math.Round(avg, 3)} ms, Max FPS: {Math.Round(1000 / avg, 3)}".Info();
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

                    for (var i = 0; i < tailSize; i++)
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
                        $"Lagging framerate: {delayMilliseconds} milliseconds".Info();
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
            $"SPI Request: {BitConverter.ToString(request)}".Info();
            var response = Pi.Spi.Channel0.SendReceive(request);
            $"SPI Response: {BitConverter.ToString(response)}".Info();

            $"SPI Base Stream Request: {BitConverter.ToString(request)}".Info();
            Pi.Spi.Channel0.Write(request);
            response = Pi.Spi.Channel0.SendReceive(new byte[request.Length]);
            $"SPI Base Stream Response: {BitConverter.ToString(response)}".Info();

        }

        public static void TestDisplay()
        {
            var input = string.Empty;

            while (input.Equals("x") == false)
            {
                "Enter brightness value (0 to 255). Enter b to toggle Backlight, Enter x to Exit".Info();
                input = Console.ReadLine();

                if (input.Equals("b"))
                {
                    Pi.PiDisplay.IsBacklightOn = !Pi.PiDisplay.IsBacklightOn;
                }
                else
                {
                    byte value = 128;
                    if (byte.TryParse(input, out value))
                    {
                        if (value != Pi.PiDisplay.Brightness)
                        {
                            $"Current Value: {Pi.PiDisplay.Brightness}, New Value: {value}".Info();
                            Pi.PiDisplay.Brightness = value;
                        }
                    }
                }

                $"Display Status - Backlight: {Pi.PiDisplay.IsBacklightOn}, Brightness: {Pi.PiDisplay.Brightness}".Info();
            }

            Pi.PiDisplay.IsBacklightOn = true;
            Pi.PiDisplay.Brightness = 96;
            $"Display Status - Backlight: {Pi.PiDisplay.IsBacklightOn}, Brightness: {Pi.PiDisplay.Brightness}".Info();
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
            $"GPIO Controller initialized successfully with {Pi.Gpio.Count} pins".Info();
            $"{Pi.Info}".Info();
            $"Microseconds Since GPIO Setup: {Pi.Timing.MicrosecondsSinceSetup}".Info();
            $"Uname {Pi.Info.OperatingSystem}".Info();
            $"HostName {Computer.NetworkSettings.Instance.HostName}".Info();
            $"Uptime (seconds) {Pi.Info.Uptime}".Info();
            var timeSpan = Pi.Info.UptimeTimeSpan;
            $"Uptime (timespan) {timeSpan.Days} days {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}".Info();

            foreach (var adapter in Computer.NetworkSettings.Instance.RetrieveAdapters())
                $"Network Adapters = {adapter.Name} IPv4 {adapter.IPv4} IPv6 {adapter.IPv6} AccessPoint {adapter.AccessPointName} MAC Address: {adapter.MacAddress}".Info();
        }

        private static void TestCaptureImage()
        {
            var pictureBytes = Pi.Camera.CaptureImageJpeg(640, 480);
            var targetPath = "/home/pi/picture.jpg";
            if (File.Exists(targetPath))
                File.Delete(targetPath);

            File.WriteAllBytes(targetPath, pictureBytes);
            $"Took picture -- Byte count: {pictureBytes.Length}".Info();
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
                "Press any key to stop reading the video stream . . .".ReadKey();
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
                $"Capture Stopped. Received {megaBytesReceived} Mbytes in {videoEventCount} callbacks in {recordedSeconds} seconds".Info();
            }
        }

        static void TestColors()
        {
            var colors = new CameraColor[]
            {
                CameraColor.Black,
                CameraColor.White,
                CameraColor.Red,
                CameraColor.Green,
                CameraColor.Blue
            };

            foreach (var color in colors)
            {
                $"{color.Name,-15}: RGB Hex: {color.ToRgbHex(false)}    YUV Hex: {color.ToYuvHex(true)}".Info();
            }
        }
    }
}