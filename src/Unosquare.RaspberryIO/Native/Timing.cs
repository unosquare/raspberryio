namespace Unosquare.RaspberryIO.Native
{
    using Swan;
    using Swan.Abstractions;
    using System;
    
    /// <summary>
    /// Provides access to timing and threading properties and methods
    /// </summary>
    public class Timing : SingletonBase<Timing>
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="Timing"/> class from being created.
        /// </summary>
        /// <exception cref="System.NotSupportedException">Could not initialize the GPIO controller</exception>
        private Timing()
        {
            // placeholder
        }

        /// <summary>
        /// This returns a number representing the number of milliseconds since your program 
        /// initialized the GPIO controller. 
        /// It returns an unsigned 32-bit number which wraps after 49 days.
        /// </summary>
        /// <value>
        /// The milliseconds since setup.
        /// </value>
        public uint MillisecondsSinceSetup => WiringPi.millis();

        /// <summary>
        /// This returns a number representing the number of microseconds since your 
        /// program initialized the GPIO controller
        /// It returns an unsigned 32-bit number which wraps after approximately 71 minutes.
        /// </summary>
        /// <value>
        /// The microseconds since setup.
        /// </value>
        public uint MicrosecondsSinceSetup => WiringPi.micros();

        /// <summary>
        /// This causes program execution to pause for at least howLong milliseconds. 
        /// Due to the multi-tasking nature of Linux it could be longer. 
        /// Note that the maximum delay is an unsigned 32-bit integer or approximately 49 days.
        /// </summary>
        /// <param name="value">The value.</param>
        public static void SleepMilliseconds(uint value)
        {
            WiringPi.delay(value);
        }

        /// <summary>
        /// This causes program execution to pause for at least howLong microseconds. 
        /// Due to the multi-tasking nature of Linux it could be longer. 
        /// Note that the maximum delay is an unsigned 32-bit integer microseconds or approximately 71 minutes.
        /// Delays under 100 microseconds are timed using a hard-coded loop continually polling the system time, 
        /// Delays over 100 microseconds are done using the system nanosleep() function – 
        /// You may need to consider the implications of very short delays on the overall performance of the system, 
        /// especially if using threads.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SleepMicroseconds(uint value)
        {
            WiringPi.delayMicroseconds(value);
        }

        /// <summary>
        /// This attempts to shift your program (or thread in a multi-threaded program) to a higher priority and 
        /// enables a real-time scheduling. The priority parameter should be from 0 (the default) to 99 (the maximum). 
        /// This won’t make your program go any faster, but it will give it a bigger slice of time when other programs 
        /// are running. The priority parameter works relative to others – so you can make one program priority 1 and 
        /// another priority 2 and it will have the same effect as setting one to 10 and the other to 90 
        /// (as long as no other programs are running with elevated priorities)
        /// </summary>
        /// <param name="priority">The priority.</param>
        public void SetThreadPriority(int priority)
        {
            priority = priority.Clamp(0, 99);
            var result = WiringPi.piHiPri(priority);
            if (result < 0) HardwareException.Throw(nameof(Timing), nameof(SetThreadPriority));
        }

        /// <summary>
        /// This is really nothing more than a simplified interface to the Posix threads mechanism that Linux supports.
        /// See the manual pages on Posix threads (man pthread) if you need more control over them.
        /// </summary>
        /// <param name="worker">The worker.</param>
        /// <exception cref="System.ArgumentNullException">worker</exception>
        public void CreateThread(ThreadWorker worker)
        {
            if (worker == null)
                throw new ArgumentNullException(nameof(worker));

            var result = WiringPi.piThreadCreate(worker);
            if (result != 0) HardwareException.Throw(nameof(Timing), nameof(CreateThread));
        }

        /// <summary>
        /// These allow you to synchronize variable updates from your main program to any threads running in your program. 
        /// keyNum is a number from 0 to 3 and represents a “key”. When another process tries to lock the same key, 
        /// it will be stalled until the first process has unlocked the same key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Lock(ThreadLockKey key)
        {
            WiringPi.piLock((int)key);
        }

        /// <summary>
        /// These allow you to synchronize variable updates from your main program to any threads running in your program. 
        /// keyNum is a number from 0 to 3 and represents a “key”. When another process tries to lock the same key, 
        /// it will be stalled until the first process has unlocked the same key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Unlock(ThreadLockKey key)
        {
            WiringPi.piUnlock((int)key);
        }
    }
}
