namespace Unosquare.RaspberryIO
{
    using System.Globalization;

    /// <summary>
    /// Defines the Raspberry Pi camera's sensor ROI (Region of Interest)
    /// </summary>
    public struct CameraRect
    {
        /// <summary>
        /// Gets or sets the x in relative coordinates. (0.0 to 1.0)
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        decimal X { get; set; }
        /// <summary>
        /// Gets or sets the y location in relative coordinates.  (0.0 to 1.0)
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        decimal Y { get; set; }
        /// <summary>
        /// Gets or sets the width in relative coordinates.  (0.0 to 1.0)
        /// </summary>
        /// <value>
        /// The w.
        /// </value>
        decimal W { get; set; }
        /// <summary>
        /// Gets or sets the heigth in relative coordinates.  (0.0 to 1.0)
        /// </summary>
        /// <value>
        /// The h.
        /// </value>
        decimal H { get; set; }

        /// <summary>
        /// The default ROI which is the entire area.
        /// </summary>
        public static readonly CameraRect Default = new CameraRect() { X = 0M, Y = 0M, W = 1.0M, H = 1.0M };

        /// <summary>
        /// Clamps the members of this ROI to their minimum and maximum values
        /// </summary>
        public void Clamp()
        {
            X = X.Clamp(0M, 1M);
            Y = Y.Clamp(0M, 1M);
            W = W.Clamp(0M, 1M - X);
            H = H.Clamp(0M, 1M - Y);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is equal to the default (The entire area).
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault
        {
            get
            {
                Clamp();
                return X == Default.X && Y == Default.Y && W == Default.W && H == Default.H;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{X.ToString(CultureInfo.InvariantCulture)},{Y.ToString(CultureInfo.InvariantCulture)},{W.ToString(CultureInfo.InvariantCulture)},{H.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}
