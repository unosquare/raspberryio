namespace Unosquare.RaspberryIO.Native
{
    using System.Runtime.InteropServices;

    internal static class Standard
    {
        internal const string LibCLibrary = "libc";

        /// <summary>
        /// Fills in the structure with information about the system.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The result.</returns>
        [DllImport(LibCLibrary, EntryPoint = "uname", SetLastError = true)]
        public static extern int Uname(out SystemName name);
    }
}
