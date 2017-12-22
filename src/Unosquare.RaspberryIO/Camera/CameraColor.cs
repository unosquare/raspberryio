namespace Unosquare.RaspberryIO.Camera
{
    using Swan;
    using System;
    using System.Linq;

    /// <summary>
    /// A simple RGB color class to represent colors in RGB and YUV colorspaces.
    /// </summary>
    public class CameraColor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CameraColor"/> class.
        /// </summary>
        /// <param name="r">The red.</param>
        /// <param name="g">The green.</param>
        /// <param name="b">The blue.</param>
        public CameraColor(int r, int g, int b)
            : this(r, g, b, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraColor"/> class.
        /// </summary>
        /// <param name="r">The red.</param>
        /// <param name="g">The green.</param>
        /// <param name="b">The blue.</param>
        /// <param name="name">The well-known color name.</param>
        public CameraColor(int r, int g, int b, string name)
        {
            RGB = new[] { Convert.ToByte(r.Clamp(0, 255)), Convert.ToByte(g.Clamp(0, 255)), Convert.ToByte(b.Clamp(0, 255)) };

            var y = R * .299000f + G * .587000f + B * .114000f;
            var u = R * -.168736f + G * -.331264f + B * .500000f + 128f;
            var v = R * .500000f + G * -.418688f + B * -.081312f + 128f;

            YUV = new byte[] { (byte)y.Clamp(0, 255), (byte)u.Clamp(0, 255), (byte)v.Clamp(0, 255) };
            Name = name;
        }

        /// <summary>
        /// Gets the well-known color name.
        /// </summary>
        public string Name { get; }

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
        /// <param name="data">The data.</param>
        /// <returns>A string</returns>
        private static string ToHex(byte[] data)
        {
            return $"0x{BitConverter.ToString(data).Replace("-", string.Empty).ToLowerInvariant()}";
        }

        /// <summary>
        /// Returns a hexadecimal representation of the RGB byte array.
        /// Preceded by 0x and all in lowercase
        /// </summary>
        /// <param name="reverse">if set to <c>true</c> [reverse].</param>
        /// <returns>A string</returns>
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
        /// <returns>A string</returns>
        public string ToYuvHex(bool reverse)
        {
            var data = YUV.ToArray();
            if (reverse) Array.Reverse(data);
            return ToHex(data);
        }

        /// <summary>
        /// Gets the predefined white color.
        /// </summary>
        public static CameraColor White => new CameraColor(255, 255, 255, nameof(White));
        
        /// <summary>
        /// Gets the predefined red color.
        /// </summary>
        public static CameraColor Red => new CameraColor(255, 0, 0, nameof(Red));

        /// <summary>
        /// Gets the predefined green color.
        /// </summary>
        public static CameraColor Green => new CameraColor(0, 255, 0, nameof(Green));

        /// <summary>
        /// Gets the predefined blue color.
        /// </summary>
        public static CameraColor Blue => new CameraColor(0, 0, 255, nameof(Blue));

        /// <summary>
        /// Gets the predefined black color.
        /// </summary>
        public static CameraColor Black => new CameraColor(0, 0, 0, nameof(Black));
    }
}