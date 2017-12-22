namespace Unosquare.RaspberryIO.Native
{
    using System;
    using System.Runtime.InteropServices;

    internal static class Standard
    {
        internal const string LibCLibrary = "libc";

#pragma warning disable SA1300 // Element must begin with upper-case letter

        #region LibC Calls

        /// <summary>
        /// Gets the User ID - a user ID of 0 represents the root user
        /// </summary>
        /// <returns></returns>
        [DllImport(LibCLibrary, EntryPoint = nameof(getuid), SetLastError = true)]
        public static extern uint getuid();

        /// <summary>
        /// Gets a string describing the error number.
        /// </summary>
        /// <param name="errnum">The errnum.</param>
        /// <returns></returns>
        [DllImport(LibCLibrary, EntryPoint = nameof(strerror), SetLastError = true)]
        public static extern string strerror(int errnum);

        /// <summary>
        /// Changes file permissions on a Unix file system
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        [DllImport(LibCLibrary, EntryPoint = nameof(chmod), SetLastError = true)]
        public static extern int chmod(string filename, uint mode);

        /// <summary>
        /// Converts a string to a 32 bit integer. Use endpointer as IntPtr.Zero
        /// </summary>
        /// <param name="numberString">The number string.</param>
        /// <param name="endPointer">The end pointer.</param>
        /// <param name="numberBase">The number base.</param>
        /// <returns></returns>
        [DllImport(LibCLibrary, EntryPoint = nameof(strtol), SetLastError = true)]
        public static extern int strtol(string numberString, IntPtr endPointer, int numberBase);

        /// <summary>
        /// The write() function attempts to write nbytes from buffer to the file associated with handle. On text files, it expands each LF to a CR/LF.
        /// The function returns the number of bytes written to the file. A return value of -1 indicates an error, with errno set appropriately.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        [DllImport(LibCLibrary, EntryPoint = nameof(write), SetLastError = true)]
        public static extern int write(int fd, byte[] buffer, int count);

        /// <summary>
        /// Fills in the structure with information about the system.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        [DllImport(LibCLibrary, EntryPoint = nameof(uname), SetLastError = true)]
        public static extern int uname(out utsname name);

        /// <summary>
        /// Returns information on overall system statistics
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        [DllImport(LibCLibrary, EntryPoint = nameof(sysinfo), SetLastError = true)]
        public static extern int sysinfo(out utssysinfo name);

        #endregion

#pragma warning restore SA1300 // Element must begin with upper-case letter
    }
}