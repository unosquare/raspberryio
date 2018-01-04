namespace Unosquare.RaspberryIO.Gpio
{
    using Swan;
    using Native;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a GPIO Pin, its location and its capabilities.
    /// Full pin reference available here:
    /// http://pinout.xyz/pinout/pin31_gpio6 and  http://wiringpi.com/pins/
    /// </summary>
    public sealed partial class GpioPin
    {
        #region Property Backing

        private readonly object _syncLock = new object();
        private GpioPinDriveMode m_PinMode;
        private GpioPinResistorPullMode m_ResistorPullMode;
        private int m_PwmRegister = 0;
        private PwmMode m_PwmMode = PwmMode.Balanced;
        private uint m_PwmRange = 1024;
        private int m_PwmClockDivisor = 1;
        private int m_SoftPwmValue = -1;
        private int m_SoftPwmRange = -1;
        private int m_SoftToneFrequency = -1;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioPin"/> class.
        /// </summary>
        /// <param name="wiringPiPinNumber">The wiring pi pin number.</param>
        /// <param name="headerPinNumber">The header pin number.</param>
        private GpioPin(WiringPiPin wiringPiPinNumber, int headerPinNumber)
        {
            PinNumber = (int)wiringPiPinNumber;
            WiringPiPinNumber = wiringPiPinNumber;
            BcmPinNumber = GpioController.WiringPiToBcmPinNumber((int)wiringPiPinNumber);
            HeaderPinNumber = headerPinNumber;
            Header = (PinNumber >= 17 && PinNumber <= 20) ? GpioHeader.P5 : GpioHeader.P1;
        }

        #endregion

        #region Pin Properties

        /// <summary>
        /// Gets or sets the Wiring Pi pin number as an integer.
        /// </summary>
        public int PinNumber { get; }

        /// <summary>
        /// Gets the WiringPi Pin number
        /// </summary>
        public WiringPiPin WiringPiPinNumber { get; }

        /// <summary>
        /// Gets the BCM chip (hardware) pin number.
        /// </summary>
        public int BcmPinNumber { get; }

        /// <summary>
        /// Gets or the physical header (physical board) pin number.
        /// </summary>
        public int HeaderPinNumber { get; }

        /// <summary>
        /// Gets the pin's header (physical board) location.
        /// </summary>
        public GpioHeader Header { get; }

        /// <summary>
        /// Gets the friendly name of the pin.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the hardware mode capabilities of this pin.
        /// </summary>
        public PinCapability[] Capabilities { get; private set; }

        #endregion

        #region Hardware-Specific Properties

        /// <summary>
        /// Gets or sets the pin operating mode.
        /// </summary>
        /// <value>
        /// The pin mode.
        /// </value>
        /// <exception cref="System.NotSupportedException"></exception>
        public GpioPinDriveMode PinMode
        {
            get => m_PinMode;

            set
            {
                lock (_syncLock)
                {
                    var mode = value;
                    if ((mode == GpioPinDriveMode.GpioClock && Capabilities.Contains(PinCapability.GPCLK) == false) ||
                        (mode == GpioPinDriveMode.PwmOutput && Capabilities.Contains(PinCapability.PWM) == false) ||
                        (mode == GpioPinDriveMode.Input && Capabilities.Contains(PinCapability.GP) == false) ||
                        (mode == GpioPinDriveMode.Output && Capabilities.Contains(PinCapability.GP) == false))
                        throw new NotSupportedException(
                            $"Pin {WiringPiPinNumber} '{Name}' does not support mode '{mode}'. Pin capabilities are limited to: {string.Join(", ", Capabilities)}");

                    WiringPi.pinMode(PinNumber, (int)mode);
                    m_PinMode = mode;
                }
            }
        }

        #endregion

        #region Output Mode (Write) Members

        /// <summary>
        /// Writes the specified pin value.
        /// This method performs a digital write
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(GpioPinValue value)
        {
            lock (_syncLock)
            {
                if (PinMode != GpioPinDriveMode.Output)
                {
                    throw new InvalidOperationException(
                        $"Unable to write to pin {PinNumber} because operating mode is {PinMode}."
                        + $" Writes are only allowed if {nameof(PinMode)} is set to {GpioPinDriveMode.Output}");
                }

                WiringPi.digitalWrite(PinNumber, (int)value);
            }
        }

        /// <summary>
        /// Writes the value asynchronously.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The awaitable task</returns>
        public async Task WriteAsync(GpioPinValue value)
        {
            await Task.Run(() => { Write(value); });
        }

        /// <summary>
        /// Writes the specified bit value.
        /// This method performs a digital write
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public void Write(bool value)
            => Write(value ? GpioPinValue.High : GpioPinValue.Low);

        /// <summary>
        /// Writes the specified bit value.
        /// This method performs a digital write
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The awaitable task
        /// </returns>
        public async Task WriteAsync(bool value)
        {
            await Task.Run(() => { Write(value); });
        }

        /// <summary>
        /// Writes the specified value. 0 for low, any other value for high
        /// This method performs a digital write
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(int value)
        {
            Write(value != 0 ? GpioPinValue.High : GpioPinValue.Low);
        }

        /// <summary>
        /// Writes the specified value. 0 for low, any other value for high
        /// This method performs a digital write
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The awaitable task</returns>
        public async Task WriteAsync(int value)
        {
            await Task.Run(() => { Write(value); });
        }

        /// <summary>
        /// Writes the specified value as an analog level.
        /// You will need to register additional analog modules to enable this function for devices such as the Gertboard.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteLevel(int value)
        {
            lock (_syncLock)
            {
                if (PinMode != GpioPinDriveMode.Output)
                {
                    throw new InvalidOperationException(
                        $"Unable to write to pin {PinNumber} because operating mode is {PinMode}."
                        + $" Writes are only allowed if {nameof(PinMode)} is set to {GpioPinDriveMode.Output}");
                }

                WiringPi.analogWrite(PinNumber, value);
            }
        }

        /// <summary>
        /// Writes the specified value as an analog level.
        /// You will need to register additional analog modules to enable this function for devices such as the Gertboard.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The awaitable task</returns>
        public async Task WriteLevelAsync(int value)
        {
            await Task.Run(() => { WriteLevel(value); });
        }

        #endregion

        #region Input Mode (Read) Members

        /// <summary>
        /// This sets or gets the pull-up or pull-down resistor mode on the pin, which should be set as an input. 
        /// Unlike the Arduino, the BCM2835 has both pull-up an down internal resistors. 
        /// The parameter pud should be; PUD_OFF, (no pull up/down), PUD_DOWN (pull to ground) or PUD_UP (pull to 3.3v) 
        /// The internal pull up/down resistors have a value of approximately 50KΩ on the Raspberry Pi.
        /// </summary>
        public GpioPinResistorPullMode InputPullMode
        {
            get => PinMode == GpioPinDriveMode.Input ? m_ResistorPullMode : GpioPinResistorPullMode.Off;

            set
            {
                lock (_syncLock)
                {
                    if (PinMode != GpioPinDriveMode.Input)
                    {
                        m_ResistorPullMode = GpioPinResistorPullMode.Off;
                        throw new InvalidOperationException(
                            $"Unable to set the {nameof(InputPullMode)} for pin {PinNumber} because operating mode is {PinMode}."
                            + $" Setting the {nameof(InputPullMode)} is only allowed if {nameof(PinMode)} is set to {GpioPinDriveMode.Input}");
                    }

                    WiringPi.pullUpDnControl(PinNumber, (int)value);
                    m_ResistorPullMode = value;
                }
            }
        }

        /// <summary>
        /// Reads the digital value on the pin as a boolean value.
        /// </summary>
        /// <returns>The state of the pin</returns>
        public bool Read()
        {
            lock (_syncLock)
            {
                if (PinMode != GpioPinDriveMode.Input && PinMode != GpioPinDriveMode.Output)
                {
                    throw new InvalidOperationException(
                        $"Unable to read from pin {PinNumber} because operating mode is {PinMode}."
                        + $" Reads are only allowed if {nameof(PinMode)} is set to {GpioPinDriveMode.Input} or {GpioPinDriveMode.Output}");
                }

                return WiringPi.digitalRead(PinNumber) != 0;
            }
        }

        /// <summary>
        /// Reads the digital value on the pin as a boolean value.
        /// </summary>
        /// <returns>The state of the pin</returns>
        public async Task<bool> ReadAsync()
        {
            return await Task.Run(() => { return Read(); });
        }

        /// <summary>
        /// Reads the digital value on the pin as a High or Low value.
        /// </summary>
        /// <returns>The state of the pin</returns>
        public GpioPinValue ReadValue()
            => Read() ? GpioPinValue.High : GpioPinValue.Low;

        /// <summary>
        /// Reads the digital value on the pin as a High or Low value.
        /// </summary>
        /// <returns>The state of the pin</returns>
        public async Task<GpioPinValue> ReadValueAsync()
        {
            return await Task.Run(() => { return ReadValue(); });
        }

        /// <summary>
        /// Reads the analog value on the pin.
        /// This returns the value read on the supplied analog input pin. You will need to register 
        /// additional analog modules to enable this function for devices such as the Gertboard, 
        /// quick2Wire analog board, etc.
        /// </summary>
        /// <returns>The analog level</returns>
        /// <exception cref="System.InvalidOperationException">When the pin mode is not configured as an input.</exception>
        public int ReadLevel()
        {
            lock (_syncLock)
            {
                if (PinMode != GpioPinDriveMode.Input)
                {
                    throw new InvalidOperationException(
                        $"Unable to read from pin {PinNumber} because operating mode is {PinMode}."
                        + $" Reads are only allowed if {nameof(PinMode)} is set to {GpioPinDriveMode.Input}");
                }

                return WiringPi.analogRead(PinNumber);
            }
        }

        /// <summary>
        /// Reads the analog value on the pin.
        /// This returns the value read on the supplied analog input pin. You will need to register 
        /// additional analog modules to enable this function for devices such as the Gertboard, 
        /// quick2Wire analog board, etc.
        /// </summary>
        /// <returns>The analog level</returns>
        public async Task<int> ReadLevelAsync()
        {
            return await Task.Run(() => { return ReadLevel(); });
        }

        #endregion

        #region Hardware PWM Members

        /// <summary>
        /// Gets or sets the PWM register. Values should be between 0 and 1024
        /// </summary>
        /// <value>
        /// The PWM register.
        /// </value>
        public int PwmRegister
        {
            get => m_PwmRegister;

            set
            {
                lock (_syncLock)
                {
                    if (PinMode != GpioPinDriveMode.PwmOutput)
                    {
                        m_PwmRegister = 0;

                        throw new InvalidOperationException(
                            $"Unable to write PWM register for pin {PinNumber} because operating mode is {PinMode}."
                            + $" Writing the PWM register is only allowed if {nameof(PinMode)} is set to {GpioPinDriveMode.PwmOutput}");
                    }

                    var val = value.Clamp(0, 1024);

                    WiringPi.pwmWrite(PinNumber, val);
                    m_PwmRegister = val;
                }
            }
        }

        /// <summary>
        /// The PWM generator can run in 2 modes – “balanced” and “mark:space”. The mark:space mode is traditional, 
        /// however the default mode in the Pi is “balanced”.
        /// </summary>
        /// <value>
        /// The PWM mode.
        /// </value>
        /// <exception cref="System.InvalidOperationException">When pin mode is not set a Pwn output</exception>
        public PwmMode PwmMode
        {
            get => PinMode == GpioPinDriveMode.PwmOutput ? m_PwmMode : PwmMode.Balanced;

            set
            {
                lock (_syncLock)
                {
                    if (PinMode != GpioPinDriveMode.PwmOutput)
                    {
                        m_PwmMode = PwmMode.Balanced;

                        throw new InvalidOperationException(
                            $"Unable to set PWM mode for pin {PinNumber} because operating mode is {PinMode}."
                            + $" Setting the PWM mode is only allowed if {nameof(PinMode)} is set to {GpioPinDriveMode.PwmOutput}");
                    }

                    WiringPi.pwmSetMode((int)value);
                    m_PwmMode = value;
                }
            }
        }

        /// <summary>
        /// This sets the range register in the PWM generator. The default is 1024.
        /// </summary>
        /// <value>
        /// The PWM range.
        /// </value>
        /// <exception cref="System.InvalidOperationException"></exception>
        public uint PwmRange
        {
            get => PinMode == GpioPinDriveMode.PwmOutput ? m_PwmRange : 0;

            set
            {
                lock (_syncLock)
                {
                    if (PinMode != GpioPinDriveMode.PwmOutput)
                    {
                        m_PwmRange = 1024;

                        throw new InvalidOperationException(
                            $"Unable to set PWM range for pin {PinNumber} because operating mode is {PinMode}."
                            + $" Setting the PWM range is only allowed if {nameof(PinMode)} is set to {GpioPinDriveMode.PwmOutput}");
                    }

                    WiringPi.pwmSetRange(value);
                    m_PwmRange = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the PWM clock divisor.
        /// </summary>
        /// <value>
        /// The PWM clock divisor.
        /// </value>
        /// <exception cref="System.InvalidOperationException"></exception>
        public int PwmClockDivisor
        {
            get => PinMode == GpioPinDriveMode.PwmOutput ? m_PwmClockDivisor : 0;

            set
            {
                lock (_syncLock)
                {
                    if (PinMode != GpioPinDriveMode.PwmOutput)
                    {
                        m_PwmClockDivisor = 1;

                        throw new InvalidOperationException(
                            $"Unable to set PWM range for pin {PinNumber} because operating mode is {PinMode}."
                            + $" Setting the PWM range is only allowed if {nameof(PinMode)} is set to {GpioPinDriveMode.PwmOutput}");
                    }

                    WiringPi.pwmSetClock(value);
                    m_PwmClockDivisor = value;
                }
            }
        }

        #endregion

        #region Software PWM Members

        /// <summary>
        /// Gets a value indicating whether this pin is in software based PWM mode.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is in soft PWM mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsInSoftPwmMode => m_SoftPwmValue >= 0;

        /// <summary>
        /// Starts the software based PWM on this pin.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="range">The range.</param>
        /// <exception cref="System.NotSupportedException">When the pin does not suppoert PWM</exception>
        /// <exception cref="System.InvalidOperationException">StartSoftPwm
        /// or</exception>
        public void StartSoftPwm(int value, int range)
        {
            lock (_syncLock)
            {
                if (Capabilities.Contains(PinCapability.GP) == false)
                    throw new NotSupportedException($"Pin {PinNumber} does not support software PWM");

                if (IsInSoftPwmMode)
                    throw new InvalidOperationException($"{nameof(StartSoftPwm)} has already been called.");

                var startResult = WiringPi.softPwmCreate(PinNumber, value, range);
                if (startResult == 0)
                {
                    m_SoftPwmValue = value;
                    m_SoftPwmRange = range;
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Could not start software based PWM on pin {PinNumber}. Error code: {startResult}");
                }
            }
        }

        /// <summary>
        /// Gets or sets the software PWM value on the pin.
        /// </summary>
        /// <value>
        /// The soft PWM value.
        /// </value>
        /// <exception cref="System.InvalidOperationException">StartSoftPwm</exception>
        public int SoftPwmValue
        {
            get => m_SoftPwmValue;

            set
            {
                lock (_syncLock)
                {
                    if (IsInSoftPwmMode && value >= 0)
                    {
                        WiringPi.softPwmWrite(PinNumber, value);
                        m_SoftPwmValue = value;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Software PWM requires a call to {nameof(StartSoftPwm)}.");
                    }
                }
            }
        }

        /// <summary>
        /// Gets the software PWM range used upon starting the PWM.
        /// </summary>
        public int SoftPwmRange => m_SoftPwmRange;

        #endregion

        #region Software Tone Members

        /// <summary>
        /// Gets a value indicating whether this instance is in software based tone generator mode.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is in soft tone mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsInSoftToneMode => m_SoftToneFrequency >= 0;

        /// <summary>
        /// Gets or sets the soft tone frequency. 0 to 5000 Hz is typical
        /// </summary>
        /// <value>
        /// The soft tone frequency.
        /// </value>
        /// <exception cref="System.InvalidOperationException">When soft tones cannot be initialized on the pin</exception>
        public int SoftToneFrequency
        {
            get => m_SoftToneFrequency;

            set
            {
                lock (_syncLock)
                {
                    if (IsInSoftToneMode == false)
                    {
                        var setupResult = WiringPi.softToneCreate(PinNumber);
                        if (setupResult != 0)
                            throw new InvalidOperationException(
                                $"Unable to initialize soft tone on pin {PinNumber}. Error Code: {setupResult}");
                    }

                    WiringPi.softToneWrite(PinNumber, value);
                    m_SoftToneFrequency = value;
                }
            }
        }

        #endregion

        #region Interrupts

        /// <summary>
        /// Gets the interrupt callback. Returns null if no interrupt
        /// has been registered.
        /// </summary>
        public InterrputServiceRoutineCallback InterruptCallback { get; private set; }

        /// <summary>
        /// Gets the interrupt edge detection mode.
        /// </summary>
        public EdgeDetection InterruptEdgeDetection { get; private set; } = EdgeDetection.ExternalSetup;

        /// <summary>
        /// Registers the interrupt callback on the pin. Pin mode has to be set to Input.
        /// 
        /// </summary>
        /// <param name="edgeDetection">The edge detection.</param>
        /// <param name="callback">The callback.</param>
        /// <exception cref="System.ArgumentException">callback</exception>
        /// <exception cref="System.InvalidOperationException">
        /// An interrupt callback was already registered.
        /// or
        /// RegisterInterruptCallback
        /// </exception>
        /// <exception cref="System.InvalidProgramException"></exception>
        public void RegisterInterruptCallback(EdgeDetection edgeDetection, InterrputServiceRoutineCallback callback)
        {
            if (callback == null)
                throw new ArgumentException($"{nameof(callback)} cannot be null");

            if (InterruptCallback != null)
                throw new InvalidOperationException("An interrupt callback was already registered.");

            if (PinMode != GpioPinDriveMode.Input)
            {
                throw new InvalidOperationException(
                    $"Unable to {nameof(RegisterInterruptCallback)} for pin {PinNumber} because operating mode is {PinMode}."
                    + $" Calling {nameof(RegisterInterruptCallback)} is only allowed if {nameof(PinMode)} is set to {GpioPinDriveMode.Input}");
            }

            lock (_syncLock)
            {
                var registerResult = WiringPi.wiringPiISR(PinNumber, (int)edgeDetection, callback);
                if (registerResult == 0)
                {
                    InterruptEdgeDetection = edgeDetection;
                    InterruptCallback = callback;
                }
                else
                {
                    HardwareException.Throw(nameof(GpioPin), nameof(RegisterInterruptCallback));
                }
            }
        }

        #endregion
    }
}
