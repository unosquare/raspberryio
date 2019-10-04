namespace Unosquare.RaspberryIO.Playground
{
    using Camera;
    using Swan;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    public static class SystemCamera
    {
        private const string DefaultPicturePath = "/home/pi/playground-cs";

        private static readonly Dictionary<ConsoleKey, string> MainOptions = new Dictionary<ConsoleKey, string>
        {
            { ConsoleKey.P, "Take picture" },
        };

        public static void ShowMenu()
        {
            var exit = false;

            do
            {
                Console.Clear();
                var mainOption = Terminal.ReadPrompt("System", MainOptions, "Esc to exit this menu");

                switch (mainOption.Key)
                {
                    case ConsoleKey.P:
                        CaptureImage();
                        break;
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                }
            }
            while (!exit);
        }

        private static void CaptureImage()
        {
            Terminal.Clear();

            var imageWidth = Terminal.ReadNumber("Set the image width:", 640);
            var imageHeight = Terminal.ReadNumber("Set the image height:", 480);
            var fileName = Terminal.ReadLine("Set the file name:");

            Console.Clear();

            var pictureBytes = Pi.Camera.CaptureImageJpeg(imageWidth, imageHeight);
            var targetPath = $"{DefaultPicturePath}/{fileName}.jpg";

            File.WriteAllBytes(targetPath, pictureBytes);

            Console.WriteLine($"Picture taken: {fileName}.jpg");
            Console.WriteLine($"Size: {pictureBytes.Length}B");
            Console.WriteLine($"Date Created: {DateTime.Now:MM/dd/yyyy}");
            Console.WriteLine($"At {DefaultPicturePath}\n");

            Console.WriteLine("Press Esc key to continue . . .");

            while (true)
            {
                var input = Console.ReadKey(true).Key;
                if (input != ConsoleKey.Escape) continue;

                break;
            }
        }

        private static void CaptureVideo()
        {
            Console.Clear();
            Console.WriteLine("Set the video width:");
            var videoWidth = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);
            Console.WriteLine("Set the video height:");
            var videoHeight = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);
            Console.WriteLine("Set the file name:");
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
                data =>
                {
                    videoByteCount += data.Length;
                    videoEventCount++;
                });

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($" {(char)0x25CF} ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Recording\n");
            Console.WriteLine("Press any key to STOP recording. . .");
            Console.ReadLine();
            Console.Clear();

            Pi.Camera.CloseVideoStream();

            var megaBytesReceived = (videoByteCount / (1024f * 1024f)).ToString("0.000", CultureInfo.InvariantCulture.NumberFormat);
            var recordedSeconds = DateTime.UtcNow.Subtract(startTime).TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture.NumberFormat);
            Console.WriteLine("Recording stopped. . .\n\n");
            Console.WriteLine($"Recorded {megaBytesReceived}MB\n{videoEventCount} callbacks\nRecorded {recordedSeconds} seconds\nCreated {DateTime.Now}\nAt {videoSettings.VideoFileName}\n\n");
        }
    }
}
