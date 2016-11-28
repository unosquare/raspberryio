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
            try
            {
                var pinCount = GpioController.Instance.Count;
                Console.WriteLine($"GPIO Controller initialized successfully with {pinCount} pins on the {GpioController.System.RaspberryPiVersion}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType()} {ex.Message}");
            }
            finally
            {
                Console.Write("Press any key to continue . . .");
                Console.ReadKey(true);
            }
        }
    }
}
