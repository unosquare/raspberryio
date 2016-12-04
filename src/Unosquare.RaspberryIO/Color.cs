using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unosquare.RaspberryIO
{
    public class Color
    {
        private readonly byte[] RgbBytes;
        private readonly byte[] YuvBytes;

        public Color(int r, int g, int b)
            : this(r, g, b, string.Empty)
        {
        }

        public Color(int r, int g, int b, string name)
        {
            RgbBytes = new byte[3] { Convert.ToByte(r.Clamp(0, 255)), Convert.ToByte(g.Clamp(0, 255)), Convert.ToByte(b.Clamp(0, 255)) };

            float y = R * .299000f + G * .587000f + B * .114000f;
            float u = R * -.168736f + G * -.331264f + B * .500000f + 128f;
            float v = R * .500000f + G * -.418688f + B * -.081312f + 128f;

            YuvBytes = new byte[3] { (byte)y.Clamp(0, 255), (byte)u.Clamp(0, 255), (byte)v.Clamp(0, 255) };
            Name = name;
        }

        public string Name { get; private set; }

        public byte R { get { return RgbBytes[0]; } }
        public byte G { get { return RgbBytes[1]; } }
        public byte B { get { return RgbBytes[2]; } }

        public byte[] RGB { get { return RgbBytes; } }
        public byte[] YUV { get { return YuvBytes; } }

        private static string ToHex(byte[] data)
        {
            return $"0x{BitConverter.ToString(data).Replace("-", "").ToLowerInvariant()}";
        }

        public string ToRgbHex(bool reverse)
        {
            var data = RGB.ToArray();
            if (reverse) Array.Reverse(data);
            return ToHex(data);
        }

        public string ToYuvHex(bool reverse)
        {
            var data = YUV.ToArray();
            if (reverse) Array.Reverse(data);
            return ToHex(data);
        }

        static public readonly Color White = new Color(255, 255, 255, nameof(White));
        static public readonly Color Red = new Color(255, 0, 0, nameof(Red));
        static public readonly Color Green = new Color(0, 255, 0, nameof(Green));
        static public readonly Color Blue = new Color(0, 0, 255, nameof(Blue));
        static public readonly Color Black = new Color(0, 0, 0, nameof(Black));
    }
}
