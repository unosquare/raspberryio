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
            { ConsoleKey.P, "Take picture" },
            { ConsoleKey.V, "Record video." },
        };

        private static string DefaultPicturePath = "/home/pi/playground-cs";

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
            Console.Clear();
            Console.WriteLine("Set the image width:");
            var imageWidth = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);
            Console.Clear();
            Console.WriteLine("Set the image height:");
            var imageHeight = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);
            Console.Clear();
            Console.WriteLine("Set the file name:");
            var fileName = Console.ReadLine();
            Console.Clear();

            var pictureBytes = Pi.Camera.CaptureImageJpeg(imageWidth, imageHeight);
            var targetPath = $"{DefaultPicturePath}/{fileName}.jpg";

            if (File.Exists(targetPath))
                File.Delete(targetPath);

            File.WriteAllBytes(targetPath, pictureBytes);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{(char)0x2714} ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"Picture taken: {fileName}.jpg\nSize: {pictureBytes.Length}B\nDate Created: {DateTime.Now}\nAt {DefaultPicturePath}\n\n");
        }

        private static void CaptureVideo()
        {
            Console.Clear();
            Console.WriteLine("Set the video width:");
            var videoWidth = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);
            Console.Clear();
            Console.WriteLine("Set the video height:");
            var videoHeight = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);
            Console.Clear();
            Console.WriteLine("Set the video path:");
            var videoPath = Console.ReadLine();
            Console.Clear();

            var videoByteCount = 0;
            var videoEventCount = 0;

            var videoSettings = new CameraVideoSettings
            {
                CaptureDisplayPreview = false,
                CaptureExposure = CameraExposureMode.Night,
                CaptureHeight = videoHeight,
                CaptureTimeoutMilliseconds = 0,
                CaptureWidth = videoWidth,
                ImageFlipVertically = true,
                VideoFileName = $"{DefaultPicturePath}/{videoPath}.h264",
            };

            Console.WriteLine("Press any key to START recording . . .");
            Console.ReadLine();
            Console.Clear();
            var startTime = DateTime.UtcNow;

            Pi.Camera.OpenVideoStream(
                videoSettings,
                onDataCallback: data =>
                {
                    videoByteCount += data.Length;
                    videoEventCount++;
                },
                onExitCallback: null);

            startTime = DateTime.UtcNow;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{(char)0x25CF} ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Recording\n");
            Console.WriteLine("Press any key to STOP recording. . .");
            Console.ReadLine();
            Console.Clear();

            Pi.Camera.CloseVideoStream();

            var megaBytesReceived = (videoByteCount / (1024f * 1024f)).ToString("0.000", CultureInfo.InvariantCulture.NumberFormat);
            var recordedSeconds = DateTime.UtcNow.Subtract(startTime).TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture.NumberFormat);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{(char)0x2714} ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"Recording stopped. . .\n\n");
            Console.Write($"Recorded {megaBytesReceived}MB\n{videoEventCount} callbacks\nRecorded {recordedSeconds} seconds\n\n");
        }
    }
}
