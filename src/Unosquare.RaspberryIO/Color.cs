namespace Unosquare.RaspberryIO
{
    using System;
    using System.Linq;

    /// <summary>
    /// A simple RGB color class to represent colors in RGB and YUV colorspaces.
    /// </summary>
    public class Color
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> class.
        /// </summary>
        /// <param name="r">The red.</param>
        /// <param name="g">The green.</param>
        /// <param name="b">The blue.</param>
        public Color(int r, int g, int b)
            : this(r, g, b, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> class.
        /// </summary>
        /// <param name="r">The red.</param>
        /// <param name="g">The green.</param>
        /// <param name="b">The blue.</param>
        /// <param name="name">The well-known color name.</param>
        public Color(int r, int g, int b, string name)
        {
            RGB = new byte[] { Convert.ToByte(r.Clamp(0, 255)), Convert.ToByte(g.Clamp(0, 255)), Convert.ToByte(b.Clamp(0, 255)) };

            float y = R * .299000f + G * .587000f + B * .114000f;
            float u = R * -.168736f + G * -.331264f + B * .500000f + 128f;
            float v = R * .500000f + G * -.418688f + B * -.081312f + 128f;

            YUV = new byte[] { (byte)y.Clamp(0, 255), (byte)u.Clamp(0, 255), (byte)v.Clamp(0, 255) };
            Name = name;
        }

        /// <summary>
        /// Gets the well-known color name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the red byte.
        /// </summary>
        public byte R => RGB[0];

        /// <summary>
        /// Gets the green byte.
        /// </summary>
        public byte G => RGB[1];

        /// <summary>
        /// Gets the blue byte.
        /// </summary>
        public byte B => RGB[2];

        /// <summary>
        /// Gets the RGB byte array (3 bytes).
        /// </summary>
        public byte[] RGB { get; }

        /// <summary>
        /// Gets the YUV byte array (3 bytes).
        /// </summary>
        public byte[] YUV { get; }

        /// <summary>
        /// Returns a hexadecimal representation of the data byte array 
        /// </summary>
        private static string ToHex(byte[] data)
        {
            return $"0x{BitConverter.ToString(data).Replace("-", "").ToLowerInvariant()}";
        }

        /// <summary>
        /// Returns a hexadecimal representation of the RGB byte array.
        /// Preceded by 0x and all in lowercase
        /// </summary>
        /// <param name="reverse">if set to <c>true</c> [reverse].</param>
        /// <returns></returns>
        public string ToRgbHex(bool reverse)
        {
            var data = RGB.ToArray();
            if (reverse) Array.Reverse(data);
            return ToHex(data);
        }

        /// <summary>
        /// Returns a hexadecimal representation of the YUV byte array.
        /// Preceded by 0x and all in lowercase
        /// </summary>
        /// <param name="reverse">if set to <c>true</c> [reverse].</param>
        /// <returns></returns>
        public string ToYuvHex(bool reverse)
        {
            var data = YUV.ToArray();
            if (reverse) Array.Reverse(data);
            return ToHex(data);
        }

        /// <summary>
        /// Gets the predefined white color.
        /// </summary>
        public static Color White => new Color(255, 255, 255, nameof(White));
        
        /// <summary>
        /// Gets the predefined red color.
        /// </summary>
        public static Color Red => new Color(255, 0, 0, nameof(Red));

        /// <summary>
        /// Gets the predefined green color.
        /// </summary>
        public static Color Green => new Color(0, 255, 0, nameof(Green));

        /// <summary>
        /// Gets the predefined blue color.
        /// </summary>
        public static Color Blue => new Color(0, 0, 255, nameof(Blue));

        /// <summary>
        /// Gets the predefined black color.
        /// </summary>
        public static Color Black => new Color(0, 0, 0, nameof(Black));
    }
}
