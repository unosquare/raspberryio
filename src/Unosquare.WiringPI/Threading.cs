namespace Unosquare.WiringPI
{
    using System;
    using Native;
    using RaspberryIO.Abstractions;
    using RaspberryIO.Abstractions.Native;
    using Swan;

    public class Threading : IThreading
    {
        /// <summary>
        /// This attempts to shift your program (or thread in a multi-threaded program) to a higher priority and
        /// enables a real-time scheduling. The priority parameter should be from 0 (the default) to 99 (the maximum).
        /// This won’t make your program go any faster, but it will give it a bigger slice of time when other programs
        /// are running. The priority parameter works relative to others – so you can make one program priority 1 and
        /// another priority 2 and it will have the same effect as setting one to 10 and the other to 90
        /// (as long as no other programs are running with elevated priorities).
        /// </summary>
        /// <param name="priority">The priority.</param>
        public void SetThreadPriority(int priority)
        {
            priority = priority.Clamp(0, 99);
            var result = WiringPi.PiHiPri(priority);
            if (result < 0) HardwareException.Throw(nameof(Timing), nameof(SetThreadPriority));
        }

        /// <summary>
        /// These allow you to synchronize variable updates from your main program to any threads running in your program.
        /// keyNum is a number from 0 to 3 and represents a “key”. When another process tries to lock the same key,
        /// it will be stalled until the first process has unlocked the same key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Lock(ThreadLockKey key) => WiringPi.PiLock((int)key);

        /// <summary>
        /// These allow you to synchronize variable updates from your main program to any threads running in your program.
        /// keyNum is a number from 0 to 3 and represents a “key”. When another process tries to lock the same key,
        /// it will be stalled until the first process has unlocked the same key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Unlock(ThreadLockKey key) => WiringPi.PiUnlock((int)key);

        /// <inheritdoc />
        /// <summary>
        /// This is really nothing more than a simplified interface to the Posix threads mechanism that Linux supports.
        /// See the manual pages on Posix threads (man pthread) if you need more control over them.
        /// </summary>
        /// <exception cref="ArgumentNullException">worker.</exception>
        public void StartThread(Action worker)
        {
            if (worker == null)
                throw new ArgumentNullException(nameof(worker));

            var result = WiringPi.PiThreadCreate(new ThreadWorker(worker));

            if (result != 0)
                HardwareException.Throw(nameof(Timing), nameof(StartThread));
        }

        /// <inheritdoc />
        public UIntPtr StartThreadEx(Action<UIntPtr> worker, UIntPtr userData) =>
            throw new NotSupportedException("WiringPi does only support a simple thread callback that has no parameters.");

        /// <inheritdoc />
        public void StopThreadEx(UIntPtr handle) =>
            throw new NotSupportedException("WiringPi does not support stopping threads.");
    }
}
