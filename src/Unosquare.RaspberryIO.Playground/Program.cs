namespace Unosquare.RaspberryIO.Playground
{
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
                TestSystemInfo();
                TestCaptureVideo();
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

        static void TestSystemInfo()
        {
            Console.WriteLine($"GPIO Controller initialized successfully with {Pi.Gpio.Count} pins");
            Console.WriteLine($"{Pi.Info.ToString()}");
            Console.WriteLine($"Microseconds Since GPIO Setup: {Pi.Timing.MicrosecondsSinceSetup}");
        }

        static void TestCaptureImage()
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
            var videoByteCount = 0;
            var videoEventCount = 0;
            var videoSettings = new CameraVideoSettings()
            {
                CaptureTimeoutMilliseconds = 0,
                CaptureDisplayPreview = false,
                ImageFlipVertically = true,
                //CaptureDisplayPreviewEncoded = true,
                //ImageEffect = CameraImageEffect.Denoise,
                CaptureExposure = CameraExposureMode.Night,
                CaptureWidth = 1920,
                CaptureHeight = 1080
            };


            Pi.Camera.OpenVideoStream(videoSettings,
                (data) => { videoByteCount += data.Length; videoEventCount++; }, null);
            var startTime = DateTime.UtcNow;

            Console.WriteLine("Press any key to stop reading the video stream . . .");
            Console.ReadKey(true);
            Pi.Camera.CloseVideoStream();
            Console.WriteLine($"Capture Stopped. Received {((float)videoByteCount / (float)(1024f * 1024f)).ToString("0.000")} Mbytes in {videoEventCount} callbacks in {DateTime.UtcNow.Subtract(startTime).TotalSeconds.ToString("0.000")} seconds");
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
