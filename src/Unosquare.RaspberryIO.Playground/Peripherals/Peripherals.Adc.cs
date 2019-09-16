using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.RaspberryIO.Peripherals;

namespace Unosquare.RaspberryIO.Playground.Peripherals
{
    public static partial class Peripherals
    {
        /// <summary>
        /// Test the ADS1115 ADC (or the KY-053 sensor module) together with a two-axis analog joystick (KY-023 module)
        /// Connect the joystick with the ADC as described in http://sensorkit.joy-it.net/index.php?title=KY-023_Joystick_Modul_(XY-Achsen).
        /// Note that I2C-Bus support must be enabled in config. 
        /// </summary>
        public static void TestJoystick()
        {
            Console.Clear();

            // Add device
            var device = Pi.I2C.AddDevice(ADS1015.ADS1x15ADDRESS);

            // The joystick module we're testing has also an extra digital signal when it is pressed down
            var inputPin = Pi.Gpio[Abstractions.BcmPin.Gpio24];

            inputPin.PinMode = GpioPinDriveMode.Input;

            // For some reason, this command doesn't do anything right now. To set the pull-up mode, run the python KY-023 sample first. 
            inputPin.InputPullMode = GpioPinResistorPullMode.PullUp; 

            Console.WriteLine(ExitMessage);
            var adc = new ADS1115(device);
            while (true)
            {
                var a0 = adc.ReadChannel(0);
                var a1 = adc.ReadChannel(1);
                var a2 = adc.ReadChannel(2); // This 3rd channel can be connected to 3.3V or 0V to see that the reference values are converted correctly. 
                bool pressed = inputPin.Read();
                Console.WriteLine($"Button {(pressed ? "pressed" : "not pressed")}\nX:{a0}\nY:{a1}\nReference:{a2}");
                if (Console.KeyAvailable)
                {
                    var input = Console.ReadKey(true).Key;
                    if (input == ConsoleKey.Escape)
                    {
                        break;
                    }
                }
            }
        }
    }
}
