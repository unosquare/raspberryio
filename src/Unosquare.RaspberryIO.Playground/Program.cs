using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unosquare.RaspberryIO.Playground
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"This is a test {DateTime.Now}");
            //var initTest = GpioController.TestWiringPi();
            //Console.WriteLine($"Init Test: {initTest}");

            //return;
            try
            {                
                var pinCount = GpioController.Instance.Count;
                Console.WriteLine($"GPIO Controller initialized successfully with {pinCount} pins on the {GpioController.System.RaspberryPiVersion}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType()} {ex.Message}");
            }
            finally
            {
                Console.Write("Program finished");
                //Console.ReadKey(true);
                Console.WriteLine();
            }
        }
    }
}
