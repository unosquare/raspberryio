namespace Unosquare.RaspberryIO.Peripherals
{
    using System;

    /// <summary>
    /// Encapsulates math operations for specific purposes.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Get distance between two points.
        /// </summary>
        /// <param name="a"> Point A. </param>
        /// <param name="b"> Point B. </param>
        /// <returns> Distance between two points. </returns>
        internal static double DistanceTo(double a, double b) =>
            Math.Sqrt((a * a) + (b * b));

        /// <summary>
        /// Get rotation on x axis.
        /// </summary>
        /// <param name="point"> Point. </param>
        /// <returns> Rotation on degrees along X axis. </returns>
        internal static double GetRotationX(Point3d point) =>
            Math.Atan2(point.Y, DistanceTo(point.X, point.Z)) * (180.0 / Math.PI);

        /// <summary>
        /// Get rotation on y axis.
        /// </summary>
        /// <param name="point"> Point. </param>
        /// <returns> Rotation on degrees along Y axis. </returns>
        internal static double GetRotationY(Point3d point) =>
            Math.Atan2(point.X, DistanceTo(point.Y, point.Z)) * (180.0 / Math.PI);
    }
}
