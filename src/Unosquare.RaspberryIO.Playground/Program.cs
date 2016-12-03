namespace Unosquare.RaspberryIO.Playground
{
    using System;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"Starting program at {DateTime.Now}");
            
            try
            {
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
