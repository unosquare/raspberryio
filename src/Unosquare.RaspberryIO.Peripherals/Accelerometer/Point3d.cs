namespace Unosquare.RaspberryIO.Peripherals
{
    using System;
    using System.Globalization;

    /// <summary>
    /// 3D-space point.
    /// </summary>
    public struct Point3D : IEquatable<Point3D>, IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Point3D"/> struct.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        /// <param name="z">The Z component.</param>
        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point3D"/> struct.
        /// </summary>
        /// <param name="value">The point to extract X, Y and Zcomponents from.</param>
        public Point3D(Point3D value)
            : this(value.X, value.Y, value.Z)
        {
        }

        /// <summary>
        /// The X component of the point.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// The Y component of the point.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// The Z component of the point.
        /// </summary>
        public double Z { get; }

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Point3D left, Point3D right) =>
            left.Equals(right);

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Point3D left, Point3D right) =>
            !(left == right);

        /// <summary>
        /// Divides the first point by the second.
        /// </summary>
        /// <param name="left">The first source point.</param>
        /// <param name="right">The second source point.</param>
        /// <returns>A new point result of the division.</returns>
        public static Point3D operator /(Point3D left, Point3D right) =>
            new Point3D(left.X / right.X, left.Y / right.Y, left.Z / right.Z);

        /// <summary>
        /// Divides the point by the given scalar.
        /// </summary>
        /// <param name="point">The source point.</param>
        /// <param name="scalar">The scalar value.</param>
        /// <returns>A new point result of the division.</returns>
        public static Point3D operator /(Point3D point, double scalar) =>
            new Point3D(point.X / scalar, point.Y / scalar, point.Z / scalar);

        /// <summary>
        /// Divides the first point by the second.
        /// </summary>
        /// <param name="left">The first source point.</param>
        /// <param name="right">The second source point.</param>
        /// <returns>A new point result of the division.</returns>
        public static Point3D Divide(Point3D left, Point3D right) =>
            left / right;

        /// <summary>
        /// Divides the point by the given scalar.
        /// </summary>
        /// <param name="point">The source point.</param>
        /// <param name="scalar">The scalar value.</param>
        /// <returns>A new point result of the division.</returns>
        public static Point3D Divide(Point3D point, double scalar) =>
            point / scalar;

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Point3D))
                return false;

            return Equals((Point3D)obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(Point3D other) =>
            X == other.X && Y == other.Y && Z == other.Z;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() =>
            X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString() =>
            ToString("G", CultureInfo.CurrentCulture);

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public string ToString(string format) =>
            ToString(format, CultureInfo.CurrentCulture);

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider) =>
            $"X: {X.ToString(format, formatProvider)}\n" +
            $"Y: {Y.ToString(format, formatProvider)}\n" +
            $"Z: {Z.ToString(format, formatProvider)}";
    }
}
