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
        /// Initializes a new instance of the <see cref="Point3d"/> struct.
        /// </summary>
        /// <param name="copy">The copy.</param>
        public Point3d(Point3d copy)
        {
            X = copy.X;
            Y = copy.Y;
            Z = copy.Z;
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

        /// <summary>
        /// Implements the / operator.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="scale">The scale.</param>
        /// <returns>The result of the operator.</returns>
        public static Point3d operator / (Point3d point, double scale) =>
            new Point3d(point.X / scale, point.Y / scale, point.Z / scale);

        /// <summary>
        /// Divides the specified scale.
        /// </summary>
        /// <param name="scale">The scale.</param>
        /// <returns>Point3d.</returns>
        public Point3d Divide(double scale) =>
            this / scale;
    }
}
