namespace Unosquare.RaspberryIO.Peripherals
{
    using System;
    internal static class Extensions
    {
        /// <summary>
        /// Get distance between two points.
        /// </summary>
        /// <param name="a"> Point A. </param>
        /// <param name="b"> Point B. </param>
        /// <returns> Distance between two points. </returns>
        internal static double DistanceTo(this double a, double b) =>
            Math.Sqrt((a * a) + (b * b));

        /// <summary>
        /// Get rotation on x axis.
        /// </summary>
        /// <param name="x"> Point X. </param>
        /// <param name="y"> Point Y. </param>
        /// <param name="z"> Point Z. </param>
        /// <returns> Rotation on degrees along X axis. </returns>
        internal static double GetRotationX(this double x, double y, double z) =>
            Math.Atan2(y, x.DistanceTo(z)) * (180.0 / Math.PI);

        /// <summary>
        /// Get rotation on y axis.
        /// </summary>
        /// <param name="x"> Point X. </param>
        /// <param name="y"> Point Y. </param>
        /// <param name="z"> Point Z. </param>
        /// <returns> Rotation on degrees along Y axis. </returns>
        internal static double GetRotationY(this double x, double y, double z) =>
            Math.Atan2(x, y.DistanceTo(z)) * (180.0 / Math.PI);
    }
}
