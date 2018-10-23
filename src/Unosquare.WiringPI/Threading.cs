namespace Unosquare.WiringPI
{
    using System;
    using Native;
    using RaspberryIO.Abstractions;
    using RaspberryIO.Abstractions.Native;
    using Swan;

    public class Threading
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
        /// This is really nothing more than a simplified interface to the Posix threads mechanism that Linux supports.
        /// See the manual pages on Posix threads (man pthread) if you need more control over them.
        /// </summary>
        /// <param name="worker">The worker.</param>
        /// <exception cref="ArgumentNullException">worker.</exception>
        public void CreateThread(ThreadWorker worker)
        {
            if (worker == null)
                throw new ArgumentNullException(nameof(worker));

            var result = WiringPi.PiThreadCreate(worker);
            if (result != 0) HardwareException.Throw(nameof(Timing), nameof(CreateThread));
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
    }
}
