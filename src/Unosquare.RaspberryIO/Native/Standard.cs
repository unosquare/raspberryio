﻿namespace Unosquare.RaspberryIO.Native
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Provides standard libc calls using platform-invoke
    /// </summary>
    internal static class Standard
    {
        internal const string LibCLibrary = "libc";

        #region LibC Calls

        /// <summary>
        /// Gets a string describing the error number.
        /// </summary>
        /// <param name="errnum">The errnum.</param>
        /// <returns>The result</returns>
        [DllImport(LibCLibrary, EntryPoint = "strerror", SetLastError = true)]
        public static extern string StrError(int errnum);

        /// <summary>
        /// Changes file permissions on a Unix file system
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>The result</returns>
        [DllImport(LibCLibrary, EntryPoint = "chmod", SetLastError = true)]
        public static extern int Chmod(string filename, uint mode);

        /// <summary>
        /// Converts a string to a 32 bit integer. Use endpointer as IntPtr.Zero
        /// </summary>
        /// <param name="numberString">The number string.</param>
        /// <param name="endPointer">The end pointer.</param>
        /// <param name="numberBase">The number base.</param>
        /// <returns>The result</returns>
        [DllImport(LibCLibrary, EntryPoint = "strtol", SetLastError = true)]
        public static extern int StringToInteger(string numberString, IntPtr endPointer, int numberBase);

        /// <summary>
        /// The write() function attempts to write nbytes from buffer to the file associated with handle. On text files, it expands each LF to a CR/LF.
        /// The function returns the number of bytes written to the file. A return value of -1 indicates an error, with errno set appropriately.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="count">The count.</param>
        /// <returns>The result</returns>
        [DllImport(LibCLibrary, EntryPoint = "write", SetLastError = true)]
        public static extern int Write(int fd, byte[] buffer, int count);

        /// <summary>
        /// Fills in the structure with information about the system.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The result</returns>
        [DllImport(LibCLibrary, EntryPoint = "uname", SetLastError = true)]
        public static extern int Uname(out SystemName name);

        #endregion
    }
}