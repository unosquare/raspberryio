using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Unosquare.RaspberryIO
{
    public delegate void InterrputServiceRequestCallback();

    internal static class Interop
    {
        #region Library References

        private const string WiringPiLibrary = "libwiringPi.so";
        private const string WiringPiSpiLibrary = "libwiringPiSPI.so";
        private const string WiringPiI2CLibrary = "libwiringPiI2C.so";
        private const string LibCLibrary = "libc";

        #endregion

        #region WiringPi Library Calls

        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiSetup))]
        public static extern int wiringPiSetup();

        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiSetupGpio))]
        public static extern int wiringPiSetupGpio();

        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiSetupSys))]
        public static extern int wiringPiSetupSys();

        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiSetupPhys))]
        public static extern int wiringPiSetupPhys();

        [DllImport(WiringPiLibrary, EntryPoint = nameof(pinMode))]
        public static extern void pinMode(int pin, int mode);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(digitalWrite))]
        public static extern void digitalWrite(int pin, int value);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(digitalWriteByte))]
        public static extern void digitalWriteByte(int value);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(digitalRead))]
        public static extern int digitalRead(int pin);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(pullUpDnControl))]
        public static extern void pullUpDnControl(int pin, int pud);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(pwmWrite))]
        public static extern void pwmWrite(int pin, int value);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(pwmSetMode))]
        public static extern void pwmSetMode(int mode);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(pwmSetRange))]
        public static extern void pwmSetRange(uint range);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(pwmSetClock))]
        public static extern void pwmSetClock(int divisor);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(gpioClockSet))]
        public static extern void gpioClockSet(int pin, int freq);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(softPwmCreate))]
        public static extern int softPwmCreate(int pin, int initialValue, int pwmRange);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(softPwmWrite))]
        public static extern void softPwmWrite(int pin, int value);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(softPwmStop))]
        public static extern void softPwmStop(int pin);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(millis))]
        public static extern uint millis();

        [DllImport(WiringPiLibrary, EntryPoint = nameof(micros))]
        public static extern uint micros();

        [DllImport(WiringPiLibrary, EntryPoint = nameof(delay))]
        public static extern void delay(uint howLong);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(delayMicroseconds))]
        public static extern void delayMicroseconds(uint howLong);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(piHiPri))]
        public static extern int piHiPri(int priority);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(waitForInterrupt))]
        public static extern int waitForInterrupt(int pin, int timeout);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiISR))]
        public static extern int wiringPiISR(int pin, int mode, InterrputServiceRequestCallback method);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(piBoardRev))]
        public static extern int piBoardRev();

        [DllImport(WiringPiLibrary, EntryPoint = nameof(wpiPinToGpio))]
        public static extern int wpiPinToGpio(int wPiPin);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(physPinToGpio))]
        public static extern int physPinToGpio(int physPin);

        [DllImport(WiringPiLibrary, EntryPoint = nameof(setPadDrive))]
        public static extern int setPadDrive(int group, int value);

        #endregion

        #region WiringPi SPI Library Calls

        [DllImport(WiringPiSpiLibrary, EntryPoint = nameof(wiringPiSPISetup))]
        public static extern int wiringPiSPISetup(int channel, int speed);

        [DllImport(WiringPiSpiLibrary, EntryPoint = nameof(wiringPiSPIDataRW))]
        public static extern int wiringPiSPIDataRW(int channel, IntPtr data, int len);

        #endregion

        #region WiringPi I2C Library Calls

        [DllImport(WiringPiI2CLibrary, EntryPoint = nameof(wiringPiI2CSetup))]
        public static extern int wiringPiI2CSetup(int devId);

        [DllImport(WiringPiI2CLibrary, EntryPoint = nameof(wiringPiI2CRead))]
        public static extern int wiringPiI2CRead(int fd);

        [DllImport(WiringPiI2CLibrary, EntryPoint = nameof(wiringPiI2CWrite))]
        public static extern int wiringPiI2CWrite(int fd, int data);

        [DllImport(WiringPiI2CLibrary, EntryPoint = nameof(wiringPiI2CWriteReg8))]
        public static extern int wiringPiI2CWriteReg8(int fd, int reg, int data);

        [DllImport(WiringPiI2CLibrary, EntryPoint = nameof(wiringPiI2CWriteReg16))]
        public static extern int wiringPiI2CWriteReg16(int fd, int reg, int data);

        [DllImport(WiringPiI2CLibrary, EntryPoint = nameof(wiringPiI2CReadReg8))]
        public static extern int wiringPiI2CReadReg8(int fd, int reg);

        [DllImport(WiringPiI2CLibrary, EntryPoint = nameof(wiringPiI2CReadReg16))]
        public static extern int wiringPiI2CReadReg16(int fd, int reg);

        #endregion

        #region LibC Calls

        [DllImport(LibCLibrary)]
        public static extern uint getuid();

        #endregion
    }
}
