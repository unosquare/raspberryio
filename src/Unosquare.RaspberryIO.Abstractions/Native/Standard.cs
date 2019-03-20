namespace Unosquare.RaspberryIO.Abstractions.Native
{
    using System;
    using Swan;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Provides standard 'libc' calls using platform-invoke.
    /// </summary>
    public static class Standard
    {
        internal const string LibCLibrary = "libc";

        #region LibC Calls

        /// <summary>
        /// Strerrors the specified error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>The error string.</returns>
        public static string Strerror(int error)
        {
            if (!Runtime.IsUsingMonoRuntime) return Marshal.PtrToStringAnsi(StrError(error));

            try
            {
                var buffer = new StringBuilder(256);
                var result = Strerror(error, buffer, (ulong)buffer.Capacity);
                return (result != -1) ? buffer.ToString() : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        /// <summary>
        /// Returns a pointer to a string that describes the error code passed in the argument.
        /// </summary>
        /// <param name="errnum">The error code.</param>
        /// <returns>A pointer to a string that describes the error code.</returns>
        [DllImport(LibCLibrary, EntryPoint = "strerror", SetLastError = true)]
        private static extern IntPtr StrError(int errnum);

        [DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_strerror_r", SetLastError = true)]
        private static extern int Strerror(int error, [Out] StringBuilder buffer, ulong length);

        #endregion
    }
}