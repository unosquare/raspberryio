namespace Unosquare.RaspberryIO.Playground.Peripherals
{
    using System;
    using Swan;
    using Abstractions;
    using Unosquare.RaspberryIO.Peripherals;

    public static partial class Peripherals
    {
        /// <summary>
        /// Tests the infrared sensor HX1838.
        /// </summary>
        public static void TestInfraredSensor()
        {
            Console.Clear();
            Console.WriteLine("Send a signal...");
            var inputPin = Pi.Gpio[BcmPin.Gpio25]; // BCM Pin 25 or Physical pin 22 on the right side of the header.
            var sensor = new InfraredSensor(inputPin, true);

            sensor.DataAvailable += (s, e) =>
               {
                   Console.Clear();
                   var necData = InfraredSensor.NecDecoder.DecodePulses(e.Pulses);
                   if (necData != null)
                   {
                       Console.WriteLine($"NEC Data: {BitConverter.ToString(necData).Replace("-", " "),12}    Pulses: {e.Pulses.Length,4}    Duration(us): {e.TrainDurationUsecs,6}    Reason: {e.FlushReason}");

                       if (InfraredSensor.NecDecoder.IsRepeatCode(e.Pulses))
                           return;
                   }
                   else
                   {
                       if (e.Pulses.Length >= 4)
                       {
                           var debugData = InfraredSensor.DebugPulses(e.Pulses);
                           Console.WriteLine($"RX    Length: {e.Pulses.Length,5}; Duration: {e.TrainDurationUsecs,7}; Reason: {e.FlushReason}");
                           Console.WriteLine($"Debug data: {debugData}");
                       }
                       else
                       {
                           Console.WriteLine($"RX (Garbage): {e.Pulses.Length,5}; Duration: {e.TrainDurationUsecs,7}; Reason: {e.FlushReason}");
                       }
                   }

                   Console.WriteLine(ExitMessage);
               };

            while (true)
            {
                var input = Console.ReadKey(true).Key;
                if (input != ConsoleKey.Escape) continue;

                break;
            }

            sensor.Dispose();
        }
    }
}
