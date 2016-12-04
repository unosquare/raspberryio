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
                var cts = new CancellationTokenSource();

                var pictureBytes = Pi.Camera.CaptureJpeg(640, 480, cts.Token).GetAwaiter().GetResult();
                var targetPath = "/home/pi/picture.jpg";
                if (File.Exists(targetPath))
                    File.Delete(targetPath);

                File.WriteAllBytes(targetPath, pictureBytes);
                Console.WriteLine($"Took picture -- Byte count: {pictureBytes.Length}");

                Pi.Gpio.Pin00.RegisterInterruptCallback(EdgeDetection.EdgeBoth, new InterrputServiceRoutineCallback(() => { Console.WriteLine("Detected ISR on pin"); }));
                Console.WriteLine($"GPIO Controller initialized successfully with {Pi.Gpio.Count} pins");
                Console.WriteLine($"{Pi.Info.ToString()}");
                Console.WriteLine($"Micros Since Setup: {Pi.Timing.MicrosecondsSinceSetup}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType()} {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Program finished.");
            }
        }
    }
}
