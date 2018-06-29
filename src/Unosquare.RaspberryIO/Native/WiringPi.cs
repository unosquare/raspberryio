namespace Unosquare.RaspberryIO.Native
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Provides native C WiringPi Library function call wrappers
    /// All credit for the native library goes to the author of http://wiringpi.com/
    /// The wrappers were written based on https://github.com/WiringPi/WiringPi/blob/master/wiringPi/wiringPi.h
    /// </summary>
    public partial class WiringPi
    {
        internal const string WiringPiLibrary = "libwiringPi.so.2.46";

        #region WiringPi - Core Functions (https://github.com/WiringPi/WiringPi/blob/master/wiringPi/wiringPi.h)

        /// <summary>
        /// This initialises wiringPi and assumes that the calling program is going to be using the wiringPi pin numbering scheme.
        /// This is a simplified numbering scheme which provides a mapping from virtual pin numbers 0 through 16 to the real underlying Broadcom GPIO pin numbers.
        /// See the pins page for a table which maps the wiringPi pin number to the Broadcom GPIO pin number to the physical location on the edge connector.
        /// This function needs to be called with root privileges.
        /// </summary>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "wiringPiSetup", SetLastError = true)]
        public static extern int WiringPiSetup();

        /// <summary>
        /// This initialises wiringPi but uses the /sys/class/gpio interface rather than accessing the hardware directly.
        /// This can be called as a non-root user provided the GPIO pins have been exported before-hand using the gpio program.
        /// Pin numbering in this mode is the native Broadcom GPIO numbers – the same as wiringPiSetupGpio() above,
        /// so be aware of the differences between Rev 1 and Rev 2 boards.
        ///
        /// Note: In this mode you can only use the pins which have been exported via the /sys/class/gpio interface before you run your program.
        /// You can do this in a separate shell-script, or by using the system() function from inside your program to call the gpio program.
        /// Also note that some functions have no effect when using this mode as they’re not currently possible to action unless called with root privileges.
        /// (although you can use system() to call gpio to set/change modes if needed)
        /// </summary>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "wiringPiSetupSys", SetLastError = true)]
        public static extern int WiringPiSetupSys();

        /// <summary>
        /// This is identical to wiringPiSetup, however it allows the calling programs to use the Broadcom GPIO
        /// pin numbers directly with no re-mapping.
        /// As above, this function needs to be called with root privileges, and note that some pins are different
        /// from revision 1 to revision 2 boards.
        /// </summary>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "wiringPiSetupGpio", SetLastError = true)]
        public static extern int WiringPiSetupGpio();

        /// <summary>
        /// Identical to wiringPiSetup, however it allows the calling programs to use the physical pin numbers on the P1 connector only.
        /// This function needs to be called with root privileges.
        /// </summary>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "wiringPiSetupPhys", SetLastError = true)]
        public static extern int WiringPiSetupPhys();

        /// <summary>
        /// This function is undocumented
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="mode">The mode.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "pinModeAlt", SetLastError = true)]
        public static extern void PinModeAlt(int pin, int mode);

        /// <summary>
        /// This sets the mode of a pin to either INPUT, OUTPUT, PWM_OUTPUT or GPIO_CLOCK.
        /// Note that only wiringPi pin 1 (BCM_GPIO 18) supports PWM output and only wiringPi pin 7 (BCM_GPIO 4)
        /// supports CLOCK output modes.
        ///
        /// This function has no effect when in Sys mode. If you need to change the pin mode, then you can
        /// do it with the gpio program in a script before you start your program.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="mode">The mode.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "pinMode", SetLastError = true)]
        public static extern void PinMode(int pin, int mode);

        /// <summary>
        /// This sets the pull-up or pull-down resistor mode on the given pin, which should be set as an input.
        /// Unlike the Arduino, the BCM2835 has both pull-up an down internal resistors. The parameter pud should be; PUD_OFF,
        /// (no pull up/down), PUD_DOWN (pull to ground) or PUD_UP (pull to 3.3v) The internal pull up/down resistors
        /// have a value of approximately 50KΩ on the Raspberry Pi.
        ///
        /// This function has no effect on the Raspberry Pi’s GPIO pins when in Sys mode.
        /// If you need to activate a pull-up/pull-down, then you can do it with the gpio program in a script before you start your program.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="pud">The pud.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "pullUpDnControl", SetLastError = true)]
        public static extern void PullUpDnControl(int pin, int pud);

        /// <summary>
        /// This function returns the value read at the given pin. It will be HIGH or LOW (1 or 0) depending on the logic level at the pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "digitalRead", SetLastError = true)]
        public static extern int DigitalRead(int pin);

        /// <summary>
        /// Writes the value HIGH or LOW (1 or 0) to the given pin which must have been previously set as an output.
        /// WiringPi treats any non-zero number as HIGH, however 0 is the only representation of LOW.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="value">The value.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "digitalWrite", SetLastError = true)]
        public static extern void DigitalWrite(int pin, int value);

        /// <summary>
        /// Writes the value to the PWM register for the given pin. The Raspberry Pi has one
        /// on-board PWM pin, pin 1 (BMC_GPIO 18, Phys 12) and the range is 0-1024.
        /// Other PWM devices may have other PWM ranges.
        /// This function is not able to control the Pi’s on-board PWM when in Sys mode.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="value">The value.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "pwmWrite", SetLastError = true)]
        public static extern void PwmWrite(int pin, int value);

        /// <summary>
        /// This returns the value read on the supplied analog input pin. You will need to
        /// register additional analog modules to enable this function for devices such as the Gertboard, quick2Wire analog board, etc.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "analogRead", SetLastError = true)]
        public static extern int AnalogRead(int pin);

        /// <summary>
        /// This writes the given value to the supplied analog pin. You will need to register additional
        /// analog modules to enable this function for devices such as the Gertboard.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="value">The value.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "analogWrite", SetLastError = true)]
        public static extern void AnalogWrite(int pin, int value);

        /// <summary>
        /// This returns the board revision of the Raspberry Pi. It will be either 1 or 2. Some of the BCM_GPIO pins changed number and
        /// function when moving from board revision 1 to 2, so if you are using BCM_GPIO pin numbers, then you need to be aware of the differences.
        /// </summary>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "piBoardRev", SetLastError = true)]
        public static extern int PiBoardRev();

        /// <summary>
        /// This function is undocumented
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="mem">The memory.</param>
        /// <param name="maker">The maker.</param>
        /// <param name="overVolted">The over volted.</param>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "piBoardId", SetLastError = true)]
        public static extern int PiBoardId(ref int model, ref int mem, ref int maker, ref int overVolted);

        /// <summary>
        /// This returns the BCM_GPIO pin number of the supplied wiringPi pin. It takes the board revision into account.
        /// </summary>
        /// <param name="wPiPin">The w pi pin.</param>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "wpiPinToGpio", SetLastError = true)]
        public static extern int WpiPinToGpio(int wPiPin);

        /// <summary>
        /// This returns the BCM_GPIO pin number of the supplied physical pin on the P1 connector.
        /// </summary>
        /// <param name="physPin">The physical pin.</param>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "physPinToGpio", SetLastError = true)]
        public static extern int PhysPinToGpio(int physPin);

        /// <summary>
        /// This sets the “strength” of the pad drivers for a particular group of pins.
        /// There are 3 groups of pins and the drive strength is from 0 to 7. Do not use this unless you know what you are doing.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="value">The value.</param>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "setPadDrive", SetLastError = true)]
        public static extern int SetPadDrive(int group, int value);

        /// <summary>
        /// Undocumented function
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "getAlt", SetLastError = true)]
        public static extern int GetAlt(int pin);

        /// <summary>
        /// Undocumented function
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="freq">The freq.</param>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "pwmToneWrite", SetLastError = true)]
        public static extern int PwmToneWrite(int pin, int freq);

        /// <summary>
        /// This writes the 8-bit byte supplied to the first 8 GPIO pins.
        /// It’s the fastest way to set all 8 bits at once to a particular value, although it still takes two write operations to the Pi’s GPIO hardware.
        /// </summary>
        /// <param name="value">The value.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "digitalWriteByte", SetLastError = true)]
        public static extern void DigitalWriteByte(int value);

        /// <summary>
        /// This writes the 8-bit byte supplied to the first 8 GPIO pins.
        /// It’s the fastest way to set all 8 bits at once to a particular value, although it still takes two write operations to the Pi’s GPIO hardware.
        /// </summary>
        /// <param name="value">The value.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "digitalWriteByte2", SetLastError = true)]
        public static extern void DigitalWriteByte2(int value);

        /// <summary>
        /// Undocumented function
        /// This reads the 8-bit byte supplied to the first 8 GPIO pins.
        /// It’s the fastest way to get all 8 bits at once to a particular value.
        /// </summary>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "digitalReadByte", SetLastError = true)]
        public static extern uint DigitalReadByte();

        /// <summary>
        /// Undocumented function
        /// This reads the 8-bit byte supplied to the first 8 GPIO pins.
        /// It’s the fastest way to get all 8 bits at once to a particular value.
        /// </summary>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "digitalReadByte2", SetLastError = true)]
        public static extern uint DigitalReadByte2();

        /// <summary>
        /// The PWM generator can run in 2 modes – “balanced” and “mark:space”. The mark:space mode is traditional,
        /// however the default mode in the Pi is “balanced”. You can switch modes by supplying the parameter: PWM_MODE_BAL or PWM_MODE_MS.
        /// </summary>
        /// <param name="mode">The mode.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "pwmSetMode", SetLastError = true)]
        public static extern void PwmSetMode(int mode);

        /// <summary>
        /// This sets the range register in the PWM generator. The default is 1024.
        /// </summary>
        /// <param name="range">The range.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "pwmSetRange", SetLastError = true)]
        public static extern void PwmSetRange(uint range);

        /// <summary>
        /// This sets the divisor for the PWM clock.
        /// Note: The PWM control functions can not be used when in Sys mode.
        /// To understand more about the PWM system, you’ll need to read the Broadcom ARM peripherals manual.
        /// </summary>
        /// <param name="divisor">The divisor.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "pwmSetClock", SetLastError = true)]
        public static extern void PwmSetClock(int divisor);

        /// <summary>
        /// Undocumented function
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="freq">The freq.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "gpioClockSet", SetLastError = true)]
        public static extern void GpioClockSet(int pin, int freq);

        /// <summary>
        /// Note: Jan 2013: The waitForInterrupt() function is deprecated – you should use the newer and easier to use wiringPiISR() function below.
        /// When called, it will wait for an interrupt event to happen on that pin and your program will be stalled. The timeOut parameter is given in milliseconds,
        /// or can be -1 which means to wait forever.
        /// The return value is -1 if an error occurred (and errno will be set appropriately), 0 if it timed out, or 1 on a successful interrupt event.
        /// Before you call waitForInterrupt, you must first initialise the GPIO pin and at present the only way to do this is to use the gpio program, either
        /// in a script, or using the system() call from inside your program.
        /// e.g. We want to wait for a falling-edge interrupt on GPIO pin 0, so to setup the hardware, we need to run: gpio edge 0 falling
        /// before running the program.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The result code</returns>
        [Obsolete]
        [DllImport(WiringPiLibrary, EntryPoint = "waitForInterrupt", SetLastError = true)]
        public static extern int WaitForInterrupt(int pin, int timeout);

        /// <summary>
        /// This function registers a function to received interrupts on the specified pin.
        /// The edgeType parameter is either INT_EDGE_FALLING, INT_EDGE_RISING, INT_EDGE_BOTH or INT_EDGE_SETUP.
        /// If it is INT_EDGE_SETUP then no initialisation of the pin will happen – it’s assumed that you have already setup the pin elsewhere
        /// (e.g. with the gpio program), but if you specify one of the other types, then the pin will be exported and initialised as specified.
        /// This is accomplished via a suitable call to the gpio utility program, so it need to be available.
        /// The pin number is supplied in the current mode – native wiringPi, BCM_GPIO, physical or Sys modes.
        /// This function will work in any mode, and does not need root privileges to work.
        /// The function will be called when the interrupt triggers. When it is triggered, it’s cleared in the dispatcher before calling your function,
        /// so if a subsequent interrupt fires before you finish your handler, then it won’t be missed. (However it can only track one more interrupt,
        /// if more than one interrupt fires while one is being handled then they will be ignored)
        /// This function is run at a high priority (if the program is run using sudo, or as root) and executes concurrently with the main program.
        /// It has full access to all the global variables, open file handles and so on.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="method">The method.</param>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "wiringPiISR", SetLastError = true)]
        public static extern int WiringPiISR(int pin, int mode, InterruptServiceRoutineCallback method);

        /// <summary>
        /// This function creates a thread which is another function in your program previously declared using the PI_THREAD declaration.
        /// This function is then run concurrently with your main program. An example may be to have this function wait for an interrupt while
        /// your program carries on doing other tasks. The thread can indicate an event, or action by using global variables to
        /// communicate back to the main program, or other threads.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "piThreadCreate", SetLastError = true)]
        public static extern int PiThreadCreate(ThreadWorker method);

        /// <summary>
        /// These allow you to synchronise variable updates from your main program to any threads running in your program. keyNum is a number from 0 to 3 and represents a key.
        /// When another process tries to lock the same key, it will be stalled until the first process has unlocked the same key.
        /// You may need to use these functions to ensure that you get valid data when exchanging data between your main program and a thread
        /// – otherwise it’s possible that the thread could wake-up halfway during your data copy and change the data –
        /// so the data you end up copying is incomplete, or invalid. See the wfi.c program in the examples directory for an example.
        /// </summary>
        /// <param name="key">The key.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "piLock", SetLastError = true)]
        public static extern void PiLock(int key);

        /// <summary>
        /// These allow you to synchronise variable updates from your main program to any threads running in your program. keyNum is a number from 0 to 3 and represents a key.
        /// When another process tries to lock the same key, it will be stalled until the first process has unlocked the same key.
        /// You may need to use these functions to ensure that you get valid data when exchanging data between your main program and a thread
        /// – otherwise it’s possible that the thread could wake-up halfway during your data copy and change the data –
        /// so the data you end up copying is incomplete, or invalid. See the wfi.c program in the examples directory for an example.
        /// </summary>
        /// <param name="key">The key.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "piUnlock", SetLastError = true)]
        public static extern void PiUnlock(int key);

        /// <summary>
        /// This attempts to shift your program (or thread in a multi-threaded program) to a higher priority
        /// and enables a real-time scheduling. The priority parameter should be from 0 (the default) to 99 (the maximum).
        /// This won’t make your program go any faster, but it will give it a bigger slice of time when other programs are running.
        /// The priority parameter works relative to others – so you can make one program priority 1 and another priority 2
        /// and it will have the same effect as setting one to 10 and the other to 90 (as long as no other
        /// programs are running with elevated priorities)
        /// The return value is 0 for success and -1 for error. If an error is returned, the program should then consult the errno global variable, as per the usual conventions.
        /// Note: Only programs running as root can change their priority. If called from a non-root program then nothing happens.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "piHiPri", SetLastError = true)]
        public static extern int PiHiPri(int priority);

        /// <summary>
        /// This causes program execution to pause for at least howLong milliseconds.
        /// Due to the multi-tasking nature of Linux it could be longer.
        /// Note that the maximum delay is an unsigned 32-bit integer or approximately 49 days.
        /// </summary>
        /// <param name="howLong">The how long.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "delay", SetLastError = true)]
        public static extern void Delay(uint howLong);

        /// <summary>
        /// This causes program execution to pause for at least howLong microseconds.
        /// Due to the multi-tasking nature of Linux it could be longer.
        /// Note that the maximum delay is an unsigned 32-bit integer microseconds or approximately 71 minutes.
        /// Delays under 100 microseconds are timed using a hard-coded loop continually polling the system time,
        /// Delays over 100 microseconds are done using the system nanosleep() function – You may need to consider the implications
        /// of very short delays on the overall performance of the system, especially if using threads.
        /// </summary>
        /// <param name="howLong">The how long.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "delayMicroseconds", SetLastError = true)]
        public static extern void DelayMicroseconds(uint howLong);

        /// <summary>
        /// This returns a number representing the number of milliseconds since your program called one of the wiringPiSetup functions.
        /// It returns an unsigned 32-bit number which wraps after 49 days.
        /// </summary>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "millis", SetLastError = true)]
        public static extern uint Millis();

        /// <summary>
        /// This returns a number representing the number of microseconds since your program called one of
        /// the wiringPiSetup functions. It returns an unsigned 32-bit number which wraps after approximately 71 minutes.
        /// </summary>
        /// <returns>The result code</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "micros", SetLastError = true)]
        public static extern uint Micros();

        #endregion
    }
}