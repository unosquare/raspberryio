using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Unosquare.RaspberryIO
{
    public delegate void InterrputServiceRequestCallback();
    public delegate void RaspberryPiThreadFunction();

    internal static class Interop
    {
        #region Library References

        internal const string WiringPiLibrary = "libwiringPi.so.2.32";
        internal const string LibCLibrary = "libc";

        #endregion

        #region WiringPi - Core Functions (https://github.com/WiringPi/WiringPi/blob/master/wiringPi/wiringPi.h)

        /// <summary>
        /// This initialises wiringPi and assumes that the calling program is going to be using the wiringPi pin numbering scheme. 
        /// This is a simplified numbering scheme which provides a mapping from virtual pin numbers 0 through 16 to the real underlying Broadcom GPIO pin numbers. 
        /// See the pins page for a table which maps the wiringPi pin number to the Broadcom GPIO pin number to the physical location on the edge connector.
        /// This function needs to be called with root privileges.
        /// </summary>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiSetup))]
        public static extern int wiringPiSetup();

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
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiSetupSys))]
        public static extern int wiringPiSetupSys();

        /// <summary>
        /// This is identical to wiringPiSetup, however it allows the calling programs to use the Broadcom GPIO 
        /// pin numbers directly with no re-mapping.
        /// As above, this function needs to be called with root privileges, and note that some pins are different 
        /// from revision 1 to revision 2 boards.
        /// </summary>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiSetupGpio))]
        public static extern int wiringPiSetupGpio();

        /// <summary>
        /// Identical to wiringPiSetup, however it allows the calling programs to use the physical pin numbers on the P1 connector only.
        /// This function needs to be called with root privileges.
        /// </summary>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiSetupPhys))]
        public static extern int wiringPiSetupPhys();

        /// <summary>
        /// This function is undocumented
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="mode">The mode.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(pinModeAlt))]
        public static extern void pinModeAlt(int pin, int mode);

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
        [DllImport(WiringPiLibrary, EntryPoint = nameof(pinMode))]
        public static extern void pinMode(int pin, int mode);

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
        [DllImport(WiringPiLibrary, EntryPoint = nameof(pullUpDnControl))]
        public static extern void pullUpDnControl(int pin, int pud);

        /// <summary>
        /// This function returns the value read at the given pin. It will be HIGH or LOW (1 or 0) depending on the logic level at the pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(digitalRead))]
        public static extern int digitalRead(int pin);

        /// <summary>
        /// Writes the value HIGH or LOW (1 or 0) to the given pin which must have been previously set as an output.
        /// WiringPi treats any non-zero number as HIGH, however 0 is the only representation of LOW.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="value">The value.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(digitalWrite))]
        public static extern void digitalWrite(int pin, int value);

        /// <summary>
        /// Writes the value to the PWM register for the given pin. The Raspberry Pi has one 
        /// on-board PWM pin, pin 1 (BMC_GPIO 18, Phys 12) and the range is 0-1024. 
        /// Other PWM devices may have other PWM ranges.
        /// This function is not able to control the Pi’s on-board PWM when in Sys mode.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="value">The value.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(pwmWrite))]
        public static extern void pwmWrite(int pin, int value);

        /// <summary>
        /// This returns the value read on the supplied analog input pin. You will need to 
        /// register additional analog modules to enable this function for devices such as the Gertboard, quick2Wire analog board, etc.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(analogRead))]
        public static extern int analogRead(int pin);

        /// <summary>
        /// This writes the given value to the supplied analog pin. You will need to register additional 
        /// analog modules to enable this function for devices such as the Gertboard.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="value">The value.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(analogWrite))]
        public static extern void analogWrite(int pin, int value);

        /// <summary>
        /// This returns the board revision of the Raspberry Pi. It will be either 1 or 2. Some of the BCM_GPIO pins changed number and 
        /// function when moving from board revision 1 to 2, so if you are using BCM_GPIO pin numbers, then you need to be aware of the differences.
        /// </summary>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(piBoardRev))]
        public static extern int piBoardRev();

        /// <summary>
        /// This function is undocumented
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="mem">The memory.</param>
        /// <param name="maker">The maker.</param>
        /// <param name="overVolted">The over volted.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(piBoardId))]
        public static extern int piBoardId(ref int model, ref int mem, ref int maker, ref int overVolted);

        /// <summary>
        /// This returns the BCM_GPIO pin number of the supplied wiringPi pin. It takes the board revision into account.
        /// </summary>
        /// <param name="wPiPin">The w pi pin.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wpiPinToGpio))]
        public static extern int wpiPinToGpio(int wPiPin);

        /// <summary>
        /// This returns the BCM_GPIO pin number of the supplied physical pin on the P1 connector.
        /// </summary>
        /// <param name="physPin">The physical pin.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(physPinToGpio))]
        public static extern int physPinToGpio(int physPin);

        /// <summary>
        /// This sets the “strength” of the pad drivers for a particular group of pins. 
        /// There are 3 groups of pins and the drive strength is from 0 to 7. Do not use this unless you know what you are doing.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(setPadDrive))]
        public static extern int setPadDrive(int group, int value);

        /// <summary>
        /// Undocumented function
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(getAlt))]
        public static extern int getAlt(int pin);

        /// <summary>
        /// Undocumented function
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="freq">The freq.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(pwmToneWrite))]
        public static extern int pwmToneWrite(int pin, int freq);

        /// <summary>
        /// This writes the 8-bit byte supplied to the first 8 GPIO pins. 
        /// It’s the fastest way to set all 8 bits at once to a particular value, although it still takes two write operations to the Pi’s GPIO hardware.
        /// </summary>
        /// <param name="value">The value.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(digitalWriteByte))]
        public static extern void digitalWriteByte(int value);

        /// <summary>
        /// Undocumented function
        /// This reads the 8-bit byte supplied to the first 8 GPIO pins. 
        /// It’s the fastest way to get all 8 bits at once to a particular value.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(digitalReadByte))]
        public static extern uint digitalReadByte(int pin);

        /// <summary>
        /// The PWM generator can run in 2 modes – “balanced” and “mark:space”. The mark:space mode is traditional, 
        /// however the default mode in the Pi is “balanced”. You can switch modes by supplying the parameter: PWM_MODE_BAL or PWM_MODE_MS.
        /// </summary>
        /// <param name="mode">The mode.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(pwmSetMode))]
        public static extern void pwmSetMode(int mode);

        /// <summary>
        /// This sets the range register in the PWM generator. The default is 1024.
        /// </summary>
        /// <param name="range">The range.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(pwmSetRange))]
        public static extern void pwmSetRange(uint range);

        /// <summary>
        /// This sets the divisor for the PWM clock.
        /// Note: The PWM control functions can not be used when in Sys mode. 
        /// To understand more about the PWM system, you’ll need to read the Broadcom ARM peripherals manual.
        /// </summary>
        /// <param name="divisor">The divisor.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(pwmSetClock))]
        public static extern void pwmSetClock(int divisor);

        /// <summary>
        /// Undocumented function
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="freq">The freq.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(gpioClockSet))]
        public static extern void gpioClockSet(int pin, int freq);

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
        /// <returns></returns>
        [Obsolete, DllImport(WiringPiLibrary, EntryPoint = nameof(waitForInterrupt))]
        public static extern int waitForInterrupt(int pin, int timeout);

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
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiISR))]
        public static extern int wiringPiISR(int pin, int mode, InterrputServiceRequestCallback method);

        /// <summary>
        /// This function creates a thread which is another function in your program previously declared using the PI_THREAD declaration. 
        /// This function is then run concurrently with your main program. An example may be to have this function wait for an interrupt while 
        /// your program carries on doing other tasks. The thread can indicate an event, or action by using global variables to 
        /// communicate back to the main program, or other threads.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(piThreadCreate))]
        public static extern int piThreadCreate(RaspberryPiThreadFunction method);

        /// <summary>
        /// These allow you to synchronise variable updates from your main program to any threads running in your program. keyNum is a number from 0 to 3 and represents a “key”. 
        /// When another process tries to lock the same key, it will be stalled until the first process has unlocked the same key.
        /// You may need to use these functions to ensure that you get valid data when exchanging data between your main program and a thread 
        /// – otherwise it’s possible that the thread could wake-up halfway during your data copy and change the data – 
        /// so the data you end up copying is incomplete, or invalid. See the wfi.c program in the examples directory for an example.
        /// </summary>
        /// <param name="key">The key.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(piLock))]
        public static extern void piLock(int key);

        /// <summary>
        /// These allow you to synchronise variable updates from your main program to any threads running in your program. keyNum is a number from 0 to 3 and represents a “key”. 
        /// When another process tries to lock the same key, it will be stalled until the first process has unlocked the same key.
        /// You may need to use these functions to ensure that you get valid data when exchanging data between your main program and a thread 
        /// – otherwise it’s possible that the thread could wake-up halfway during your data copy and change the data – 
        /// so the data you end up copying is incomplete, or invalid. See the wfi.c program in the examples directory for an example.
        /// </summary>
        /// <param name="key">The key.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(piUnlock))]
        public static extern void piUnlock(int key);

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
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(piHiPri))]
        public static extern int piHiPri(int priority);

        /// <summary>
        /// This causes program execution to pause for at least howLong milliseconds. 
        /// Due to the multi-tasking nature of Linux it could be longer. 
        /// Note that the maximum delay is an unsigned 32-bit integer or approximately 49 days.
        /// </summary>
        /// <param name="howLong">The how long.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(delay))]
        public static extern void delay(uint howLong);

        /// <summary>
        /// This causes program execution to pause for at least howLong microseconds. 
        /// Due to the multi-tasking nature of Linux it could be longer. 
        /// Note that the maximum delay is an unsigned 32-bit integer microseconds or approximately 71 minutes.
        /// Delays under 100 microseconds are timed using a hard-coded loop continually polling the system time, 
        /// Delays over 100 microseconds are done using the system nanosleep() function – You may need to consider the implications 
        /// of very short delays on the overall performance of the system, especially if using threads.
        /// </summary>
        /// <param name="howLong">The how long.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(delayMicroseconds))]
        public static extern void delayMicroseconds(uint howLong);

        /// <summary>
        /// This returns a number representing the number of milliseconds since your program called one of the wiringPiSetup functions. 
        /// It returns an unsigned 32-bit number which wraps after 49 days.
        /// </summary>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(millis))]
        public static extern uint millis();

        /// <summary>
        /// This returns a number representing the number of microseconds since your program called one of 
        /// the wiringPiSetup functions. It returns an unsigned 32-bit number which wraps after approximately 71 minutes.
        /// </summary>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(micros))]
        public static extern uint micros();

        #endregion

        #region WiringPi - Serial Port

        /// <summary>
        /// This opens and initialises the serial device and sets the baud rate. It sets the port into “raw” mode (character at a time and no translations), 
        /// and sets the read timeout to 10 seconds. The return value is the file descriptor or -1 for any error, in which case errno will be set as appropriate.
        /// The wiringSerial library is intended to provide simplified control – suitable for most applications, however if you need advanced control 
        /// – e.g. parity control, modem control lines (via a USB adapter, there are none on the Pi’s on-board UART!) and so on, 
        /// then you need to do some of this the old fashioned way.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="baud">The baud.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(serialOpen))]
        public static extern int serialOpen(string device, int baud);

        /// <summary>
        /// Closes the device identified by the file descriptor given.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(serialClose))]
        public static extern int serialClose(int fd);

        /// <summary>
        /// Sends the single byte to the serial device identified by the given file descriptor.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <param name="c">The c.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(serialPutchar))]
        public static extern void serialPutchar(int fd, byte c);

        /// <summary>
        /// Sends the nul-terminated string to the serial device identified by the given file descriptor.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <param name="s">The s.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(serialPuts))]
        public static extern void serialPuts(int fd, string s);

        /// <summary>
        /// Returns the number of characters available for reading, or -1 for any error condition, 
        /// in which case errno will be set appropriately.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(serialDataAvail))]
        public static extern int serialDataAvail(int fd);

        /// <summary>
        /// Returns the next character available on the serial device. 
        /// This call will block for up to 10 seconds if no data is available (when it will return -1)
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(serialGetchar))]
        public static extern int serialGetchar(int fd);

        /// <summary>
        /// This discards all data received, or waiting to be send down the given device.
        /// </summary>
        /// <param name="fd">The fd.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(serialFlush))]
        public static extern void serialFlush(int fd);

        #endregion

        #region WiringPi - Shift Library

        /// <summary>
        /// This shifts an 8-bit data value in with the data appearing on the dPin and the clock being sent out on the cPin. 
        /// Order is either LSBFIRST or MSBFIRST. The data is sampled after the cPin goes high. 
        /// (So cPin high, sample data, cPin low, repeat for 8 bits) The 8-bit value is returned by the function.
        /// </summary>
        /// <param name="dPin">The d pin.</param>
        /// <param name="cPin">The c pin.</param>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(shiftIn))]
        public static extern byte shiftIn(byte dPin, byte cPin, byte order);

        /// <summary>
        /// The shifts an 8-bit data value val out with the data being sent out on dPin and the clock being sent out on the cPin. 
        /// order is as above. Data is clocked out on the rising or falling edge – ie. dPin is set, then cPin is taken high then low 
        /// – repeated for the 8 bits.
        /// </summary>
        /// <param name="dPin">The d pin.</param>
        /// <param name="cPin">The c pin.</param>
        /// <param name="order">The order.</param>
        /// <param name="val">The value.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(shiftOut))]
        public static extern void shiftOut(byte dPin, byte cPin, byte order, byte val);

        #endregion

        #region WiringPi - Soft PWM (https://github.com/WiringPi/WiringPi/blob/master/wiringPi/softPwm.h)

        /// <summary>
        /// This creates a software controlled PWM pin. You can use any GPIO pin and the pin numbering will be that of the wiringPiSetup() 
        /// function you used. Use 100 for the pwmRange, then the value can be anything from 0 (off) to 100 (fully on) for the given pin.
        /// The return value is 0 for success. Anything else and you should check the global errno variable to see what went wrong.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="pwmRange">The PWM range.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(softPwmCreate))]
        public static extern int softPwmCreate(int pin, int initialValue, int pwmRange);

        /// <summary>
        /// This updates the PWM value on the given pin. The value is checked to be in-range and pins that haven’t previously 
        /// been initialised via softPwmCreate will be silently ignored.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="value">The value.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(softPwmWrite))]
        public static extern void softPwmWrite(int pin, int value);

        /// <summary>
        /// This function is undocumented
        /// </summary>
        /// <param name="pin">The pin.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(softPwmStop))]
        public static extern void softPwmStop(int pin);

        /// <summary>
        /// This creates a software controlled tone pin. You can use any GPIO pin and the pin numbering will be that of the wiringPiSetup() function you used.
        /// The return value is 0 for success. Anything else and you should check the global errno variable to see what went wrong.
        /// </summary>
        /// <param name="pin">The pin.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(softToneCreate))]
        public static extern void softToneCreate(int pin);

        /// <summary>
        /// This function is undocumented
        /// </summary>
        /// <param name="pin">The pin.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(softToneStop))]
        public static extern void softToneStop(int pin);

        /// <summary>
        /// This updates the tone frequency value on the given pin. The tone will be played until you set the frequency to 0.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="freq">The freq.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(softToneWrite))]
        public static extern void softToneWrite(int pin, int freq);

        #endregion

        #region WiringPi - SPI Library Calls

        /// <summary>
        /// This function is undocumented
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiSPIGetFd))]
        public static extern int wiringPiSPIGetFd(int channel);

        /// <summary>
        /// This performs a simultaneous write/read transaction over the selected SPI bus. Data that was in your buffer is overwritten by data returned from the SPI bus.
        /// That’s all there is in the helper library. It is possible to do simple read and writes over the SPI bus using the standard read() and write() system calls though – 
        /// write() may be better to use for sending data to chains of shift registers, or those LED strings where you send RGB triplets of data. 
        /// Devices such as A/D and D/A converters usually need to perform a concurrent write/read transaction to work.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="data">The data.</param>
        /// <param name="len">The length.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiSPIDataRW))]
        public static extern int wiringPiSPIDataRW(int channel, byte[] data, int len);

        /// <summary>
        /// This function is undocumented
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="speed">The speed.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiSPISetupMode))]
        public static extern int wiringPiSPISetupMode(int channel, int speed, int mode);

        /// <summary>
        /// This is the way to initialise a channel (The Pi has 2 channels; 0 and 1). The speed parameter is an integer 
        /// in the range 500,000 through 32,000,000 and represents the SPI clock speed in Hz.
        /// The returned value is the Linux file-descriptor for the device, or -1 on error. If an error has happened, you may use the standard errno global variable to see why.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="speed">The speed.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiSPISetup))]
        public static extern int wiringPiSPISetup(int channel, int speed);

        #endregion

        #region WiringPi - I2C Library Calls

        /// <summary>
        /// Simple device read. Some devices present data when you read them without having to do any register transactions.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiI2CRead))]
        public static extern int wiringPiI2CRead(int fd);

        /// <summary>
        /// These read an 8-bit value from the device register indicated.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <param name="reg">The reg.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiI2CReadReg8))]
        public static extern int wiringPiI2CReadReg8(int fd, int reg);

        /// <summary>
        /// These read a 16-bit value from the device register indicated.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <param name="reg">The reg.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiI2CReadReg16))]
        public static extern int wiringPiI2CReadReg16(int fd, int reg);

        /// <summary>
        /// Simple device write. Some devices accept data this way without needing to access any internal registers.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiI2CWrite))]
        public static extern int wiringPiI2CWrite(int fd, int data);

        /// <summary>
        /// These write an 8-bit data value into the device register indicated.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <param name="reg">The reg.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiI2CWriteReg8))]
        public static extern int wiringPiI2CWriteReg8(int fd, int reg, int data);

        /// <summary>
        /// These write a 16-bit data value into the device register indicated.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <param name="reg">The reg.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiI2CWriteReg16))]
        public static extern int wiringPiI2CWriteReg16(int fd, int reg, int data);

        /// <summary>
        /// This initialises the I2C system with your given device identifier. 
        /// The ID is the I2C number of the device and you can use the i2cdetect program to find this out. wiringPiI2CSetup() 
        /// will work out which revision Raspberry Pi you have and open the appropriate device in /dev.
        /// The return value is the standard Linux filehandle, or -1 if any error – in which case, you can consult errno as usual.
        /// E.g. the popular MCP23017 GPIO expander is usually device Id 0x20, so this is the number you would pass into wiringPiI2CSetup().
        /// </summary>
        /// <param name="devId">The dev identifier.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiI2CSetup))]
        public static extern int wiringPiI2CSetup(int devId);

        #endregion

        #region LibC Calls

        /// <summary>
        /// Gets the User ID - a user ID of 0 represents the root user
        /// </summary>
        /// <returns></returns>
        [DllImport(LibCLibrary)]
        public static extern uint getuid();

        #endregion
    }
}
