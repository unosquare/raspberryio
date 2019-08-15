namespace Unosquare.RaspberryIO.Playground.Peripherals
{
    using System;
    using Swan;
    using Swan.Logging;
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
            "Send a signal...".Info("IR");
            var inputPin = Pi.Gpio[BcmPin.Gpio25]; // BCM Pin 25 or Physical pin 22 on the right side of the header.
            var sensor = new InfraredSensor(inputPin, true);

            sensor.DataAvailable += (s, e) =>
               {
                   Console.Clear();
                   var necData = InfraredSensor.NecDecoder.DecodePulses(e.Pulses);
                   if (necData != null)
                   {
                       $"NEC Data: {BitConverter.ToString(necData).Replace("-", " "),12}    Pulses: {e.Pulses.Length,4}    Duration(us): {e.TrainDurationUsecs,6}    Reason: {e.FlushReason}".Warn("IR");

                       if (InfraredSensor.NecDecoder.IsRepeatCode(e.Pulses))
                           return;
                   }
                   else
                   {
                       if (e.Pulses.Length >= 4)
                       {
                           var debugData = InfraredSensor.DebugPulses(e.Pulses);
                           $"RX    Length: {e.Pulses.Length,5}; Duration: {e.TrainDurationUsecs,7}; Reason: {e.FlushReason}".Warn("IR");
                           debugData.Info("IR");
                       }
                       else
                       {
                           $"RX (Garbage): {e.Pulses.Length,5}; Duration: {e.TrainDurationUsecs,7}; Reason: {e.FlushReason}".Error("IR");
                       }
                   }

                   Terminal.WriteLine(ExitMessage);
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
