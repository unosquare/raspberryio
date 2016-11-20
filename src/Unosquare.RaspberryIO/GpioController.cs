using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Unosquare.RaspberryIO
{
    public class GpioController
    {

        static private GpioController m_Instance = null;
        static private readonly ManualResetEventSlim OperationDone = new ManualResetEventSlim(true);
        static private readonly object SyncLock = new object();

        

        private GpioController()
        {
            // placeholder
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
            lock (SyncLock)
            {
                if (IsInitialized)
                    throw new InvalidOperationException($"Cannot call {nameof(Initialize)} more than once.");

                var result = -1;

                switch (mode)
                {
                    case ControllerMode.DirectWithMappedPins:
                        {
                            result = Interop.wiringPiSetup();
                            break;
                        }
                    case ControllerMode.DirectWithHardwarePins:
                        {
                            result = Interop.wiringPiSetupGpio();
                            break;
                        }
                    case ControllerMode.DirectWithNamedPins:
                        {
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

    }
}
