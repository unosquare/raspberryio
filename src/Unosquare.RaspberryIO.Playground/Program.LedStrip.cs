#if NET461
namespace Unosquare.RaspberryIO.Playground
{
    using Peripherals;
    using Swan;
    using Swan.Formatters;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    public partial class Program
    {
        /// <summary>
        /// Tests the led strip graphics.
        /// </summary>
        public static void TestLedStripGraphics()
        {
            BitmapBuffer pixels = null;

            try
            {
                using (var bitmap =
                    new System.Drawing.Bitmap(Path.Combine(Runtime.EntryAssemblyDirectory, "fractal.jpg")))
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
                var strip = new LedStripAPA102C(60 * 4, 1, 1000000); // 1 Mhz is sufficient for such a short strip (only 240 LEDs)
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
                        currentBrightness = 0.05f + (0.80f * (currentRow / (pixels.ImageHeight - 1f)));

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
                $"Frames: {currentFrameNumber + 1}, FPS: {Math.Round(1000f / frameTimes.Average(), 3)}, Strip Render: {Math.Round(avg, 3)} ms, Max FPS: {Math.Round(1000 / avg, 3)}"
                    .Info();
                strip.Render();
            });

            thread.Start();
            Console.Write("Press any key to stop and clear");
            Console.ReadKey(true);
            Console.WriteLine();
            exitAnimation = true;
        }

        /// <summary>
        /// Tests the led strip.
        /// </summary>
        public static void TestLedStrip()
        {
            var exitAnimation = false;

            var thread = new Thread(() =>
            {
                var strip = new LedStripAPA102C(60 * 4);
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

                    var delayMilliseconds = Convert.ToInt32(DateTime.UtcNow.Subtract(lastRenderTime).TotalMilliseconds);
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

            thread.IsBackground = true;
            thread.Name = nameof(LedStripAPA102C);
            thread.Start();
            Console.Write("Press any key to stop and clear");
            Console.ReadKey(true);
            Console.WriteLine();
            exitAnimation = true;
        }
    }
}
#endif