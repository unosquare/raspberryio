namespace Unosquare.RaspberryIO.Peripherals
{
    using Swan;
    using System.Collections;
    using System.Linq;

    public sealed partial class InfraredSensor
    {
        /// <summary>
        /// Provides decoding Methods for the NEC IR Protocol.
        /// Idea taken from here: https://www.sbprojects.net/knowledge/ir/nec.php.
        /// </summary>
        public static class NecDecoder
        {
            private const long ShortPulseLengthMin = 560 - 160;
            private const long ShortPulseLengthMax = 560 + 140;

            private const long BurstSpaceLengthMin = 9000 - 1800;
            private const long BurstSpaceLengthMax = 9000 + 1800;

            private const long BurstMarkLengthMin = 4500 - 600;
            private const long BurstMarkLengthMax = 4500 + 600;

            private const long RepeatPulseLengthMin = 2500 - 500;
            private const long RepeatPulseLengthMax = 2500 + 500;

            private const int MaxBitLength = 32;

            /// <summary>
            /// Decodes the IR pulses according to the NEC protocol.
            /// If the train of pulses does not match the protocol, then it returns a null byte array.
            /// If the train of pulses is a repeat code, then it returns a 4-byte array with all bits set.
            /// </summary>
            /// <param name="pulses">The pulses.</param>
            /// <returns>The decoded bytes.</returns>
            public static byte[] DecodePulses(InfraredPulse[] pulses)
            {
                // check if we have a repeat code
                if (IsRepeatCode(pulses))
                    return new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };

                // from here: https://www.sbprojects.net/knowledge/ir/nec.php
                var startPulseIndex = -1;

                // The first thing we look for is the ACG (1) which should be around 9ms
                for (var pulseIndex = 0; pulseIndex < pulses.Length; pulseIndex++)
                {
                    var p = pulses[pulseIndex];

                    if (p.Value && p.DurationUsecs >= BurstSpaceLengthMin && p.DurationUsecs <= BurstSpaceLengthMax)
                    {
                        startPulseIndex = pulseIndex;
                        break;
                    }
                }

                // Return a null result if we could not find ACG (1)
                if (startPulseIndex == -1)
                    return null;
                
                startPulseIndex += 1;

                // Find the first 0 value, 4.5 Millisecond
                for (var pulseIndex = startPulseIndex; pulseIndex < pulses.Length; pulseIndex++)
                {
                    var p = pulses[pulseIndex];
                    startPulseIndex = -1;

                    if (p.Value == false && p.DurationUsecs >= BurstMarkLengthMin && p.DurationUsecs <= BurstMarkLengthMax)
                    {
                        startPulseIndex = pulseIndex;
                        break;
                    }
                }

                // Return a null result if we could not find the start of the train of pulses
                if (startPulseIndex == -1)
                    return null;
                
                startPulseIndex += 1;

                // Verify that the last pulse is a space (1) and and it is a short pulse
                var bits = new BitArray(MaxBitLength);
                var bitCount = 0;

                var lastPulse = pulses[pulses.Length - 1];
                if (lastPulse.Value == false || lastPulse.DurationUsecs.IsBetween(ShortPulseLengthMin, ShortPulseLengthMax) == false)
                    return null;

                // preallocate the pulse references
                InfraredPulse p1;
                InfraredPulse p2;

                // parse the bits
                for (var pulseIndex = startPulseIndex; pulseIndex < pulses.Length - 1; pulseIndex += 2)
                {
                    // logical 1 is 1 space (1) and 3 marks (0)
                    // logical 0 is 1 space (1) and 1 Mark (0)
                    p1 = pulses[pulseIndex + 0];
                    p2 = pulses[pulseIndex + 1];

                    // Expect a short Space pulse followed by a Mark pulse of variable length
                    if (p1.Value && p2.Value == false && p1.DurationUsecs.IsBetween(ShortPulseLengthMin, ShortPulseLengthMax))
                        bits[bitCount++] = p2.DurationUsecs.IsBetween(ShortPulseLengthMin, ShortPulseLengthMax);

                    if (bitCount >= MaxBitLength)
                        break;
                }

                // Check the message is 4 bytes long
                if (bitCount != 32)
                    return null;

                // Return the result
                var result = new byte[bitCount / 8];
                bits.CopyTo(result, 0);
                return result;
            }

            /// <summary>
            /// Determines whether the set of pulses represent a repeat code.
            /// </summary>
            /// <param name="pulses">The pulses.</param>
            /// <returns>
            ///   <c>true</c> if the train of pulses represents an NEC repeat code.
            /// </returns>
            public static bool IsRepeatCode(InfraredPulse[] pulses)
            {
                if (pulses.Length != 4) return false;
                var trainLength = pulses.Sum(s => s.DurationUsecs);
                if (trainLength.IsBetween(12000, 120000) == false)
                    return false;

                var startPulseIndex = -1;

                // The first thing we look for is the ACG (1) which should be around 9ms
                for (var pulseIndex = 0; pulseIndex < pulses.Length; pulseIndex++)
                {
                    var p = pulses[pulseIndex];
                    if (p.Value && p.DurationUsecs >= BurstSpaceLengthMin && p.DurationUsecs <= BurstSpaceLengthMax)
                    {
                        startPulseIndex = pulseIndex;
                        break;
                    }
                }

                if (startPulseIndex == -1)
                    return false;

                if (startPulseIndex + 2 >= pulses.Length)
                    return false;

                // Check the next pulse is a 2.5ms low value
                var p1 = pulses[startPulseIndex + 1];
                if (p1.Value || p1.DurationUsecs.IsBetween(RepeatPulseLengthMin, RepeatPulseLengthMax) == false)
                    return false;

                // Check the next pulse is a 560 microsecond high value
                var p2 = pulses[startPulseIndex + 2];

                // All checks passed. Looks like it really is a repeat code
                return p2.Value && p2.DurationUsecs.IsBetween(ShortPulseLengthMin, ShortPulseLengthMax);
            }
        }
    }
}
