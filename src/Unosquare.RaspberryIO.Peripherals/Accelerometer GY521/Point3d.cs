namespace Unosquare.RaspberryIO.Peripherals
{
    using System;
    /// <summary>
    /// 3D-space point.
    /// </summary>
    public struct Point3d : IEquatable<Point3d>
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
        /// Implements the == operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Point3d left, Point3d right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Point3d left, Point3d right)
        {
            return !(left == right);
        }

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

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;

            var point = (Point3d)obj;
            return X.Equals(point.X) && Y.Equals(point.Y) && Z.Equals(point.Z);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(Point3d other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString() =>
                $"  X:  {Math.Round(X, 2)}\n" +
                $"  Y:  {Math.Round(Y, 2)}\n" +
                $"  Z:  {Math.Round(Z, 2)}\n";
    }
}
