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
                var mainOption = "System".ReadPrompt(MainOptions, "Esc to exit this menu");

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
            Console.Clear();

            var imageWidth = "Set the image width:".ReadNumber(640);
            var imageHeight = "Set the image height:".ReadNumber(480);

            "Set the file name:".Write();
            var fileName = Console.ReadLine();

            Console.Clear();

            var pictureBytes = Pi.Camera.CaptureImageJpeg(imageWidth, imageHeight);
            var targetPath = $"{DefaultPicturePath}/{fileName}.jpg";

            File.WriteAllBytes(targetPath, pictureBytes);

            $"Picture taken: {fileName}.jpg".Info();
            $"Size: {pictureBytes.Length}B".Info();
            $"Date Created: {DateTime.Now.ToString("MM/dd/yyyy")}".Info();
            $"At {DefaultPicturePath}\n".Info();

            var input = "Press Esc key to continue . . .".ReadKey(true).Key;
            while (true)
            {
                if (input == ConsoleKey.Escape)
                {
                    break;
                }

                input = Console.ReadKey(true).Key;
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

            "Press any key to START recording . . .".Info();
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
            Console.Write($" {(char)0x25CF} ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Recording\n");
            Console.WriteLine("Press any key to STOP recording. . .");
            Console.ReadLine();
            Console.Clear();

            Pi.Camera.CloseVideoStream();

            var megaBytesReceived = (videoByteCount / (1024f * 1024f)).ToString("0.000", CultureInfo.InvariantCulture.NumberFormat);
            var recordedSeconds = DateTime.UtcNow.Subtract(startTime).TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture.NumberFormat);
            "Recording stopped. . .\n\n".Info();
            $"Recorded {megaBytesReceived}MB\n{videoEventCount} callbacks\nRecorded {recordedSeconds} seconds\nCreated {DateTime.Now}\nAt {videoSettings.VideoFileName}\n\n".Info();
        }
    }
}
