namespace Unosquare.RaspberryIO.Playground
{
    using Camera;
    using Swan;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

    public static class SystemCamera
    {
        private static readonly Dictionary<ConsoleKey, string> MainOptions = new Dictionary<ConsoleKey, string>
        {
            // Module COntrol Items
            { ConsoleKey.P, "Take a picture" },
            { ConsoleKey.V, "Record video." },
        };
        private static int DefaultHeight = 1080;
        private static int DefaultWidth = 1920;
        private static string DefaultPicturePath = "/home/pi/picture.jpg";

        public static async Task ShowMenu()
        {
            var exit = false;
            bool pressKey;

            do
            {
                Console.Clear();
                pressKey = true;

                var mainOption = "System".ReadPrompt(MainOptions, "Esc to exit this menu");

                switch (mainOption.Key)
                {
                    case ConsoleKey.P:
                        CaptureImage();
                        break;
                    case ConsoleKey.V:
                        CaptureVideo();
                        break;
                    case ConsoleKey.Escape:
                        exit = true;
                        pressKey = false;
                        break;
                    default:
                        pressKey = false;
                        break;
                }

                if (pressKey)
                {
                    await Task.Delay(500).ConfigureAwait(false);
                    Console.WriteLine("Press any key to continue . . .");
                    Console.ReadKey(true);
                }
            }
            while (!exit);
        }

        private static void CaptureImage()
        {
            var imageWidth = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);
            var imageHeight = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);

            var pictureBytes = Pi.Camera.CaptureImageJpeg(imageWidth, imageHeight);
            var targetPath = DefaultPicturePath;

            if (File.Exists(targetPath))
                File.Delete(targetPath);

            File.WriteAllBytes(targetPath, pictureBytes);
            Console.WriteLine($"Picture taken - Size: {pictureBytes.Length}");
        }

        private static void CaptureVideo()
        {
            // Setup our working variables
            var videoByteCount = 0;
            var videoEventCount = 0;

            // Configure video settings
            var videoSettings = new CameraVideoSettings
            {
                CaptureDisplayPreview = false,
                CaptureExposure = CameraExposureMode.Night,
                CaptureHeight = DefaultHeight,
                CaptureTimeoutMilliseconds = 0,
                CaptureWidth = DefaultWidth,
                ImageFlipVertically = true,
            };

            Console.WriteLine("Press any key to START recording . . .");
            Console.ReadLine();
            var startTime = DateTime.UtcNow;

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
            Console.WriteLine("Press any key to STOP recording. . .");
            Console.ReadLine();

            // Always close the video stream to ensure raspivid quits
            Pi.Camera.CloseVideoStream();

            // Output the stats
            var megaBytesReceived = (videoByteCount / (1024f * 1024f)).ToString("0.000", CultureInfo.InvariantCulture.NumberFormat);
            var recordedSeconds = DateTime.UtcNow.Subtract(startTime).TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture.NumberFormat);
            $"Recording stopped.\n Recorded {megaBytesReceived} MB in {videoEventCount} callbacks in {recordedSeconds} seconds".Info();
        }
    }
}
