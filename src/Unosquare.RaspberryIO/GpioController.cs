using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Unosquare.RaspberryIO
{
    public class GpioController
    {

        private const string WiringPiCodesEnvironmentVariable = "WIRINGPI_CODES";

        static private GpioController m_Instance = null;
        static private readonly ManualResetEventSlim OperationDone = new ManualResetEventSlim(true);
        static private readonly Dictionary<int, GpioPin> RegisteredPins = new Dictionary<int, GpioPin>();
        static private readonly object SyncLock = new object();

        private GpioController()
        {
            lock (SyncLock)
            {
                RegisteredPins[GpioPin.Pin00.PinNumber] = GpioPin.Pin00;
                RegisteredPins[GpioPin.Pin01.PinNumber] = GpioPin.Pin01;
                RegisteredPins[GpioPin.Pin02.PinNumber] = GpioPin.Pin02;
                RegisteredPins[GpioPin.Pin03.PinNumber] = GpioPin.Pin03;
                RegisteredPins[GpioPin.Pin04.PinNumber] = GpioPin.Pin04;
                RegisteredPins[GpioPin.Pin05.PinNumber] = GpioPin.Pin05;
                RegisteredPins[GpioPin.Pin06.PinNumber] = GpioPin.Pin06;
                RegisteredPins[GpioPin.Pin07.PinNumber] = GpioPin.Pin07;
                RegisteredPins[GpioPin.Pin08.PinNumber] = GpioPin.Pin08;
                RegisteredPins[GpioPin.Pin09.PinNumber] = GpioPin.Pin09;
                RegisteredPins[GpioPin.Pin10.PinNumber] = GpioPin.Pin10;
                RegisteredPins[GpioPin.Pin11.PinNumber] = GpioPin.Pin11;
                RegisteredPins[GpioPin.Pin12.PinNumber] = GpioPin.Pin12;
                RegisteredPins[GpioPin.Pin13.PinNumber] = GpioPin.Pin13;
                RegisteredPins[GpioPin.Pin14.PinNumber] = GpioPin.Pin14;
                RegisteredPins[GpioPin.Pin15.PinNumber] = GpioPin.Pin15;
                RegisteredPins[GpioPin.Pin16.PinNumber] = GpioPin.Pin16;
                RegisteredPins[GpioPin.Pin17.PinNumber] = GpioPin.Pin17;
                RegisteredPins[GpioPin.Pin18.PinNumber] = GpioPin.Pin18;
                RegisteredPins[GpioPin.Pin19.PinNumber] = GpioPin.Pin19;
                RegisteredPins[GpioPin.Pin20.PinNumber] = GpioPin.Pin20;
                RegisteredPins[GpioPin.Pin21.PinNumber] = GpioPin.Pin21;
                RegisteredPins[GpioPin.Pin22.PinNumber] = GpioPin.Pin22;
                RegisteredPins[GpioPin.Pin23.PinNumber] = GpioPin.Pin23;
                RegisteredPins[GpioPin.Pin24.PinNumber] = GpioPin.Pin24;
                RegisteredPins[GpioPin.Pin25.PinNumber] = GpioPin.Pin25;
                RegisteredPins[GpioPin.Pin26.PinNumber] = GpioPin.Pin26;
                RegisteredPins[GpioPin.Pin27.PinNumber] = GpioPin.Pin27;
                RegisteredPins[GpioPin.Pin28.PinNumber] = GpioPin.Pin28;
                RegisteredPins[GpioPin.Pin29.PinNumber] = GpioPin.Pin29;
                RegisteredPins[GpioPin.Pin30.PinNumber] = GpioPin.Pin30;
                RegisteredPins[GpioPin.Pin31.PinNumber] = GpioPin.Pin31;

            }
        }

        public GpioController Instance
        {
            get
            {
                lock (SyncLock)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = new GpioController();
                    }

                    return m_Instance;
                }
            }
        }

        public bool IsInitialized { get { return Mode != ControllerMode.NotInitialized; } }

        public ControllerMode Mode { get; private set; } = ControllerMode.NotInitialized;

        public bool Initialize(ControllerMode mode)
        {
            if (Utilities.IsLinuxOS == false)
                throw new PlatformNotSupportedException($"This library does not support the platform {Environment.OSVersion.ToString()}");

            lock (SyncLock)
            {
                if (IsInitialized)
                    throw new InvalidOperationException($"Cannot call {nameof(Initialize)} more than once.");

                Environment.SetEnvironmentVariable(WiringPiCodesEnvironmentVariable, "1", EnvironmentVariableTarget.Process);

                var result = -1;

                switch (mode)
                {
                    case ControllerMode.DirectWithMappedPins:
                        {
                            if (Utilities.IsRunningAsRoot == false)
                                throw new PlatformNotSupportedException($"This program must be started with root privileges for mode '{mode}'");

                            result = Interop.wiringPiSetup();
                            break;
                        }
                    case ControllerMode.DirectWithHardwarePins:
                        {
                            if (Utilities.IsRunningAsRoot == false)
                                throw new PlatformNotSupportedException($"This program must be started with root privileges for mode '{mode}'");

                            result = Interop.wiringPiSetupGpio();
                            break;
                        }
                    case ControllerMode.DirectWithNamedPins:
                        {
                            if (Utilities.IsRunningAsRoot == false)
                                throw new PlatformNotSupportedException($"This program must be started with root privileges for mode '{mode}'");

                            result = Interop.wiringPiSetupPhys();
                            break;
                        }
                    case ControllerMode.FileStreamWithHardwarePins:
                        {
                            result = Interop.wiringPiSetupSys();
                            break;
                        }
                    default:
                        {
                            throw new ArgumentException($"'{mode}' is not a valid initialization mode.");
                        }
                }

                Mode = result == 0 ? mode : ControllerMode.NotInitialized;
                return IsInitialized;
            }
        }

        #region Pin Addressing Methods

        public GpioPin ByPinNumber(int wiringPiPinNumber)
        {
            lock (SyncLock)
            {
                return RegisteredPins[wiringPiPinNumber];
            }
        }

        public GpioPin ByHeaderPinNumber(int headerPinNumber, GpioHeader header)
        {
            lock (SyncLock)
            {
                return RegisteredPins.Values.Single(p => p.Header == header && p.HeaderPinNumber == headerPinNumber);
            }
        }

        public GpioPin ByBcmPinNumber(int bcmPinNumber)
        {
            lock (SyncLock)
            {
                return RegisteredPins.Values.Single(p => p.BcmPinNumber == bcmPinNumber);
            }
        }

        public GpioPin[] AllPins
        {
            get
            {
                lock (SyncLock)
                {
                    return RegisteredPins.Values.ToArray();
                }
            }
        }

        #endregion

    }
}
