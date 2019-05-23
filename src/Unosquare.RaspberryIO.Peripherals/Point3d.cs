namespace Unosquare.RaspberryIO.Peripherals
{
    /// <summary>
    /// 3D-space point.
    /// </summary>
    public struct Point3d
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Point3d"/> struct.
        /// </summary>
        /// <param name="x"> X factor. </param>
        /// <param name="y"> Y factor. </param>
        /// <param name="z"> Z factor. </param>
        public Point3d(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// X coordinate.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Y coordinate.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Z coordinate.
        /// </summary>
        public double Z { get; }
    }
}
