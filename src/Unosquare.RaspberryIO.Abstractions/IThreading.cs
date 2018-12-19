namespace Unosquare.RaspberryIO.Abstractions
{
    using System;

    /// <summary>
    /// Interface to represent threading methods using interop.
    /// </summary>
    public interface IThreading
    {
        /// <summary>
        /// Starts a new thread of execution which runs concurrently with your main program.
        /// </summary>
        /// <param name="worker">The thread routine.</param>
        void StartThread(Action worker);

        /// <summary>
        /// Starts a new thread of execution which runs concurrently with your main program.
        /// </summary>
        /// <param name="worker">The thread routine.</param>
        /// <param name="userData">A pointer to the user data.</param>
        /// <returns>A pointer to the new thread.</returns>
        UIntPtr StartThreadEx(Action<UIntPtr> worker, UIntPtr userData);

        /// <summary>
        /// Stops the thread pointed at by handle.
        /// </summary>
        /// <param name="handle">A thread pointer returned by <see cref="StartThreadEx(Action{UIntPtr}, UIntPtr)"/>.</param>
        void StopThreadEx(UIntPtr handle);
    }
}
