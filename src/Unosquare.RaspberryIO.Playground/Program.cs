namespace Unosquare.RaspberryIO.Playground
{
    using System;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"Starting program at {DateTime.Now}");

            //return;
            try
            {                
                Console.WriteLine($"GPIO Controller initialized successfully with {Pi.Gpio.Count} pins");
                Console.WriteLine($"{Pi.Info.ToString()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType()} {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Program finished");
            }
        }
    }
}
