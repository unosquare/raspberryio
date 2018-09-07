namespace Unosquare.RaspberryIO.Peripherals
{
    using Gpio;
    using Swan;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Read and write different types of Radio-Frequency IDentification (RFID) cards on your
    /// Raspberry Pi using a RC522 based reader connected via the Serial Peripheral Interface (SPI) interface.
    /// </summary>
    public class RFIDControllerMfrc522
    {
        /// <summary>
        /// The default authentication key.
        /// </summary>
        public static readonly byte[] DefaultAuthKey = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

        #region Private Members

        private readonly SpiChannel _spiPort;
        private readonly GpioPin _outputPort;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RFIDControllerMfrc522"/> class.
        /// </summary>
        public RFIDControllerMfrc522()
            : this(Pi.Spi.Channel0, 500000, Pi.Gpio[22])
        {
            // placeholder
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RFIDControllerMfrc522" /> class.
        /// </summary>
        /// <param name="spiPort">The spi port.</param>
        /// <param name="spiFrquency">The spi frquency.</param>
        /// <param name="outputPort">The output port.</param>
        public RFIDControllerMfrc522(SpiChannel spiPort, int spiFrquency, GpioPin outputPort)
        {
            Pi.Spi.Channel0Frequency = spiFrquency;
            _spiPort = spiPort;
            _outputPort = outputPort;
            InitializeComponent();
        }

        #endregion

        #region Enumerations

        /// <summary>
        /// Contains constants for well-known commands.
        /// </summary>
        public enum Command : byte
        {
            /// <summary>
            /// The idle Command
            /// </summary>
            Idle = 0x00,

            /// <summary>
            /// The authenticate Command
            /// </summary>
            Authenticate = 0x0E,

            /// <summary>
            /// The receive Command
            /// </summary>
            Receive = 0x08,

            /// <summary>
            /// The transmit Command
            /// </summary>
            Transmit = 0x04,

            /// <summary>
            /// The transcieve Command
            /// </summary>
            Transcieve = 0x0C,

            /// <summary>
            /// The reset phase Command
            /// </summary>
            ResetPhase = 0x0F,

            /// <summary>
            /// The compute CRC Command
            /// </summary>
            ComputeCrc = 0x03,
        }

        /// <summary>
        /// Enumerates all 64 registers in the device.
        /// </summary>
        public enum Register : byte
        {
            /// <summary>
            /// The reserved00
            /// </summary>
            Reserved00 = 0x00,

            /// <summary>
            /// The command reg
            /// </summary>
            CommandReg = 0x01,

            /// <summary>
            /// The comm i en reg
            /// </summary>
            CommIEnReg = 0x02,

            /// <summary>
            /// The divl en reg
            /// </summary>
            DivlEnReg = 0x03,

            /// <summary>
            /// The comm irq reg
            /// </summary>
            CommIrqReg = 0x04,

            /// <summary>
            /// The div irq reg
            /// </summary>
            DivIrqReg = 0x05,

            /// <summary>
            /// The error reg
            /// </summary>
            ErrorReg = 0x06,

            /// <summary>
            /// The status1 reg
            /// </summary>
            Status1Reg = 0x07,

            /// <summary>
            /// The status2 reg
            /// </summary>
            Status2Reg = 0x08,

            /// <summary>
            /// The fifo data reg
            /// </summary>
            FIFODataReg = 0x09,

            /// <summary>
            /// The fifo level reg
            /// </summary>
            FIFOLevelReg = 0x0A,

            /// <summary>
            /// The water level reg
            /// </summary>
            WaterLevelReg = 0x0B,

            /// <summary>
            /// The control reg
            /// </summary>
            ControlReg = 0x0C,

            /// <summary>
            /// The bit framing reg
            /// </summary>
            BitFramingReg = 0x0D,

            /// <summary>
            /// The coll reg
            /// </summary>
            CollReg = 0x0E,

            /// <summary>
            /// The reserved01
            /// </summary>
            Reserved01 = 0x0F,

            /// <summary>
            /// The reserved10
            /// </summary>
            Reserved10 = 0x10,

            /// <summary>
            /// The mode reg
            /// </summary>
            ModeReg = 0x11,

            /// <summary>
            /// The tx mode reg
            /// </summary>
            TxModeReg = 0x12,

            /// <summary>
            /// The rx mode reg
            /// </summary>
            RxModeReg = 0x13,

            /// <summary>
            /// The tx control reg
            /// </summary>
            TxControlReg = 0x14,

            /// <summary>
            /// The tx automatic reg
            /// </summary>
            TxAutoReg = 0x15,

            /// <summary>
            /// The tx sel reg
            /// </summary>
            TxSelReg = 0x16,

            /// <summary>
            /// The rx sel reg
            /// </summary>
            RxSelReg = 0x17,

            /// <summary>
            /// The rx threshold reg
            /// </summary>
            RxThresholdReg = 0x18,

            /// <summary>
            /// The demod reg
            /// </summary>
            DemodReg = 0x19,

            /// <summary>
            /// The reserved11
            /// </summary>
            Reserved11 = 0x1A,

            /// <summary>
            /// The reserved12
            /// </summary>
            Reserved12 = 0x1B,

            /// <summary>
            /// The mifare reg
            /// </summary>
            MifareReg = 0x1C,

            /// <summary>
            /// The reserved13
            /// </summary>
            Reserved13 = 0x1D,

            /// <summary>
            /// The reserved14
            /// </summary>
            Reserved14 = 0x1E,

            /// <summary>
            /// The serial speed reg
            /// </summary>
            SerialSpeedReg = 0x1F,

            /// <summary>
            /// The reserved20
            /// </summary>
            Reserved20 = 0x20,

            /// <summary>
            /// The CRC result reg m
            /// </summary>
            CRCResultRegM = 0x21,

            /// <summary>
            /// The CRC result reg l
            /// </summary>
            CRCResultRegL = 0x22,

            /// <summary>
            /// The reserved21
            /// </summary>
            Reserved21 = 0x23,

            /// <summary>
            /// The mod width reg
            /// </summary>
            ModWidthReg = 0x24,

            /// <summary>
            /// The reserved22
            /// </summary>
            Reserved22 = 0x25,

            /// <summary>
            /// The rf CFG reg
            /// </summary>
            RFCfgReg = 0x26,

            /// <summary>
            /// The gs n reg
            /// </summary>
            GsNReg = 0x27,

            /// <summary>
            /// The cw gs p reg
            /// </summary>
            CWGsPReg = 0x28,

            /// <summary>
            /// The mod gs p reg
            /// </summary>
            ModGsPReg = 0x29,

            /// <summary>
            /// The t mode reg
            /// </summary>
            TModeReg = 0x2A,

            /// <summary>
            /// The t prescaler reg
            /// </summary>
            TPrescalerReg = 0x2B,

            /// <summary>
            /// The t reload reg h
            /// </summary>
            TReloadRegH = 0x2C,

            /// <summary>
            /// The t reload reg l
            /// </summary>
            TReloadRegL = 0x2D,

            /// <summary>
            /// The t counter value reg h
            /// </summary>
            TCounterValueRegH = 0x2E,

            /// <summary>
            /// The t counter value reg l
            /// </summary>
            TCounterValueRegL = 0x2F,

            /// <summary>
            /// The reserved30
            /// </summary>
            Reserved30 = 0x30,

            /// <summary>
            /// The test sel1 reg
            /// </summary>
            TestSel1Reg = 0x31,

            /// <summary>
            /// The test sel2 reg
            /// </summary>
            TestSel2Reg = 0x32,

            /// <summary>
            /// The test pin en reg
            /// </summary>
            TestPinEnReg = 0x33,

            /// <summary>
            /// The test pin value reg
            /// </summary>
            TestPinValueReg = 0x34,

            /// <summary>
            /// The test bus reg
            /// </summary>
            TestBusReg = 0x35,

            /// <summary>
            /// The automatic test reg
            /// </summary>
            AutoTestReg = 0x36,

            /// <summary>
            /// The version reg
            /// </summary>
            VersionReg = 0x37,

            /// <summary>
            /// The analog test reg
            /// </summary>
            AnalogTestReg = 0x38,

            /// <summary>
            /// The test da c1 reg
            /// </summary>
            TestDAC1Reg = 0x39,

            /// <summary>
            /// The test da c2 reg
            /// </summary>
            TestDAC2Reg = 0x3A,

            /// <summary>
            /// The test adc reg
            /// </summary>
            TestADCReg = 0x3B,

            /// <summary>
            /// The reserved31
            /// </summary>
            Reserved31 = 0x3C,

            /// <summary>
            /// The reserved32
            /// </summary>
            Reserved32 = 0x3D,

            /// <summary>
            /// The reserved33
            /// </summary>
            Reserved33 = 0x3E,

            /// <summary>
            /// The reserved34
            /// </summary>
            Reserved34 = 0x3F,
        }

        /// <summary>
        /// umerates the available request mode codes.
        /// </summary>
        public enum RequestMode : byte
        {
            /// <summary>
            /// The request idle mode
            /// </summary>
            RequestIdle = 0x26,

            /// <summary>
            /// The request all mode
            /// </summary>
            RequestAll = 0x52,

            /// <summary>
            /// The anti collision mode
            /// </summary>
            AntiCollision = 0x93,

            /// <summary>
            /// The select tag mode
            /// </summary>
            SelectTag = 0x93,

            /// <summary>
            /// The authenticate1 a mode
            /// </summary>
            Authenticate1A = 0x60,

            /// <summary>
            /// The authenticate1 b mode
            /// </summary>
            Authenticate1B = 0x61,

            /// <summary>
            /// The read mode
            /// </summary>
            Read = 0x30,

            /// <summary>
            /// The write mode
            /// </summary>
            Write = 0xA0,

            /// <summary>
            /// The decrement mode
            /// </summary>
            Decrement = 0xC0,

            /// <summary>
            /// The increment mode
            /// </summary>
            Increment = 0xC1,

            /// <summary>
            /// The restore mode
            /// </summary>
            Restore = 0xC2,

            /// <summary>
            /// The transfer mode
            /// </summary>
            Transfer = 0xB0,

            /// <summary>
            /// The halt mode
            /// </summary>
            Halt = 0x50,
        }

        /// <summary>
        /// Enumerates the different statuses.
        /// </summary>
        public enum Status : byte
        {
            /// <summary>
            /// All ok status
            /// </summary>
            AllOk = 0,

            /// <summary>
            /// The no tag error status
            /// </summary>
            NoTagError = 1,

            /// <summary>
            /// The error status
            /// </summary>
            Error = 2,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sends the reset phase command to the RFID Controller.
        /// </summary>
        public void Reset() => WriteCommand(Command.ResetPhase);

        /// <summary>
        /// Turns the RFID antenna on.
        /// </summary>
        public void TurnAntennaOn()
        {
            ReadRegister(Register.TxControlReg);

            // if (~(temp & 0x03) == 1)
            SetRegisterBits(Register.TxControlReg, 0x03);
        }

        /// <summary>
        /// Turns the RFID antenna off.
        /// </summary>
        public void TurnAntennaOff() => ClearRegisterBits(Register.TxControlReg, 0x03);

        /// <summary>
        /// Reads the register.
        /// </summary>
        /// <param name="register">The register.</param>
        /// <returns>The contents of the register.</returns>
        public byte ReadRegister(Register register)
        {
            var address = (byte)register;
            var readValue = _spiPort.SendReceive(new byte[] { (byte)(((address << 1) & 0x7E) | 0x80), 0 });
            return readValue[1];
        }

        /// <summary>
        /// Sends data to the RFID card.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="sendData">The send data.</param>
        /// <returns>A standard controller response.</returns>
        public RfidResponse CardSendData(Command command, byte[] sendData)
        {
            const byte maximumLength = 16;

            var backData = new List<byte>();
            byte receivedBitCount = 0;
            byte interruptEnableFlags = 0x00;
            byte waitInterruptFlags = 0x00;
            byte currentRegisterValue;
            var i = 0;

            switch (command)
            {
                case Command.Authenticate:
                    interruptEnableFlags = 0x12;
                    waitInterruptFlags = 0x10;
                    break;
                case Command.Transcieve:
                    interruptEnableFlags = 0x77;
                    waitInterruptFlags = 0x30;
                    break;
            }

            WriteRegister(Register.CommIEnReg, (byte)(interruptEnableFlags | 0x80));
            ClearRegisterBits(Register.CommIrqReg, 0x80);
            SetRegisterBits(Register.FIFOLevelReg, 0x80);

            WriteCommand(Command.Idle);

            while (i < sendData.Length)
            {
                WriteRegister(Register.FIFODataReg, sendData[i]);
                i++;
            }

            WriteCommand(command);

            if (command == Command.Transcieve)
            {
                SetRegisterBits(Register.BitFramingReg, 0x80);
            }

            i = 2000;
            do
            {
                currentRegisterValue = ReadRegister(Register.CommIrqReg);
            } while (--i != 0 && (currentRegisterValue & 0x01) == 0 &&
                     (currentRegisterValue & waitInterruptFlags) == 0);

            ClearRegisterBits(Register.BitFramingReg, 0x80);

            if (i == 0 || (ReadRegister(Register.ErrorReg) & 0x1B) != 0x00)
                return new RfidResponse(Status.Error, backData.ToArray(), receivedBitCount);

            var status = Convert.ToBoolean(currentRegisterValue & interruptEnableFlags & 0x01) ? Status.NoTagError : Status.AllOk;

            if (command == Command.Transcieve)
            {
                currentRegisterValue = ReadRegister(Register.FIFOLevelReg);
                var lastBits = ReadRegister(Register.ControlReg) & 0x07;
                receivedBitCount = (lastBits != 0)
                    ? (byte)(((currentRegisterValue - 1) * 8) + (byte)lastBits)
                    : (byte)(currentRegisterValue * 8);

                if (currentRegisterValue == 0)
                    currentRegisterValue = 1;

                if (currentRegisterValue > maximumLength)
                    currentRegisterValue = maximumLength;

                i = 0;
                while (i < currentRegisterValue)
                {
                    backData.Add(ReadRegister(Register.FIFODataReg));
                    i++;
                }
            }

            return new RfidResponse(status, backData.ToArray(), receivedBitCount);
        }

        /// <summary>
        /// Writes the specified data buffer to an RFID card.
        /// </summary>
        /// <param name="blockAddress">The block address.</param>
        /// <param name="writeData">The write data.</param>
        /// <returns>The resulting status.</returns>
        public Status CardWriteData(byte blockAddress, byte[] writeData)
        {
            var buff = new List<byte> { (byte)RequestMode.Write, blockAddress };
            var crc = CalulateCrc(buff.ToArray());
            buff.Add(crc[0]);
            buff.Add(crc[1]);

            var status = Status.AllOk;
            var transcieve = CardSendData(Command.Transcieve, buff.ToArray());

            if (transcieve.Status != Status.AllOk || transcieve.DataBitLength != 4 ||
                (transcieve.Data[0] & 0x0F) != 0x0A)
            {
                status = Status.Error;
            }

            $"{transcieve.DataBitLength} backdata &0x0F == 0x0A {transcieve.Data[0] & 0x0F}".Debug();

            if (status != Status.AllOk)
                return Status.Error;

            var i = 0;
            var buf = new List<byte>();
            while (i < 16)
            {
                buf.Add(writeData[i]);
                i++;
            }

            crc = CalulateCrc(buf.ToArray());
            buf.Add(crc[0]);
            buf.Add(crc[1]);

            var verify = CardSendData(Command.Transcieve, buf.ToArray());

            return verify.Status != Status.AllOk || verify.DataBitLength != 4 || (verify.Data[0] & 0x0F) != 0x0A
                ? Status.Error
                : status;
        }

        /// <summary>
        /// Reads the specified data block from an RFID card.
        /// </summary>
        /// <param name="blockAddress">The block address.</param>
        /// <returns>A standar response.</returns>
        public RfidResponse CardReadData(byte blockAddress)
        {
            var buff = new List<byte> { (byte)RequestMode.Read, blockAddress };
            var crc = CalulateCrc(buff.ToArray());
            buff.Add(crc[0]);
            buff.Add(crc[1]);

            var result = CardSendData(Command.Transcieve, buff.ToArray());

            return new RfidResponse(result.Status != Status.AllOk || result.Data.Length != 0x10 ? Status.Error : Status.AllOk, result);
        }

        /// <summary>
        /// Sends a request to the RFID controller.
        /// </summary>
        /// <param name="requestMode">The request mode.</param>
        /// <returns>A standard response.</returns>
        public RfidResponse SendControllerRequest(RequestMode requestMode)
        {
            var tagType = new List<byte> { (byte)requestMode };
            WriteRegister(Register.BitFramingReg, 0x07);

            var result = CardSendData(Command.Transcieve, tagType.ToArray());

            return new RfidResponse(result.Status != Status.AllOk || result.DataBitLength != 0x10 ? Status.Error : Status.AllOk, result);
        }

        /// <summary>
        /// Prepares the controller for card reading.
        /// Resturns an OK status if a card was detected.
        /// </summary>
        /// <returns>The status code.</returns>
        public Status DetectCard() => SendControllerRequest(RequestMode.RequestIdle).Status;

        /// <summary>
        /// Reads the card unique identifier.
        /// </summary>
        /// <returns>A standard response. The UID is in the Data.</returns>
        public RfidResponse ReadCardUniqueId()
        {
            WriteRegister(Register.BitFramingReg, 0x00);

            var serNum = new List<byte> { (byte)RequestMode.AntiCollision, 0x20 };

            var response = CardSendData(Command.Transcieve, serNum.ToArray());
            var status = response.Data.Length == 5 ? Status.AllOk : Status.Error;

            if (status == Status.AllOk)
            {
                byte serNumCheck = 0;
                var i = 0;

                while (i < 4)
                {
                    serNumCheck = (byte)(serNumCheck ^ response.Data[i]);
                    i = i + 1;
                }

                if (serNumCheck != response.Data[i])
                {
                    status = Status.Error;
                }
            }

            return new RfidResponse(status, response);
        }

        /// <summary>
        /// Selects the card UID before authentication.
        /// Returns 0 for error.
        /// </summary>
        /// <param name="cardUid">The serial number.</param>
        /// <returns>The size of the tag. 0 For error.</returns>
        public byte SelectCardUniqueId(byte[] cardUid)
        {
            var payloadBuffer = new List<byte> { (byte)RequestMode.SelectTag, 0x70 };
            var i = 0;
            while (i < 5)
            {
                payloadBuffer.Add(cardUid[i]);
                i++;
            }

            var crcHashCode = CalulateCrc(payloadBuffer.ToArray());
            payloadBuffer.Add(crcHashCode[0]);
            payloadBuffer.Add(crcHashCode[1]);
            var response = CardSendData(Command.Transcieve, payloadBuffer.ToArray());

            return response.Status != Status.AllOk || response.DataBitLength != 0x18 ? (byte)0 : response.Data[0];
        }

        /// <summary>
        /// Authenticates the previosuly selected card UID in 1A mode with default AuthKey.
        /// </summary>
        /// <param name="cardUid">The card uid.</param>
        /// <param name="blockAddress">The block address.</param>
        /// <returns>The status code.</returns>
        public Status AuthenticateCard1A(byte[] cardUid, byte blockAddress = (byte)Register.Status2Reg)
            => AuthenticateCard1A(DefaultAuthKey, cardUid, blockAddress);

        /// <summary>
        /// Authenticates the previosuly selected card UID in 1A mode.
        /// </summary>
        /// <param name="sectorkey">The sectorkey.</param>
        /// <param name="cardUid">The card uid.</param>
        /// <param name="blockAddress">The block address.</param>
        /// <returns>The status code.</returns>
        public Status AuthenticateCard1A(
            byte[] sectorkey,
            byte[] cardUid,
            byte blockAddress = (byte)Register.Status2Reg) => Authenticate(RequestMode.Authenticate1A, blockAddress, sectorkey, cardUid);

        /// <summary>
        /// Authenticates the previosuly selected card UID in 1B mode.
        /// </summary>
        /// <param name="sectorkey">The sectorkey.</param>
        /// <param name="cardUid">The card uid.</param>
        /// <param name="blockAddress">The block address.</param>
        /// <returns>The status code.</returns>
        public Status AuthenticateCard1B(
            byte[] sectorkey,
            byte[] cardUid,
            byte blockAddress = (byte)Register.Status2Reg) =>
            Authenticate(RequestMode.Authenticate1B, blockAddress, sectorkey, cardUid);

        /// <summary>
        /// Clears the card selection for authentication purposes.
        /// </summary>
        public void ClearCardSelection() => ClearRegisterBits(Register.Status2Reg, 0x08);

        /// <summary>
        /// Reads the authenticated registers.
        /// </summary>
        /// <param name="authKey">The authentication key.</param>
        /// <param name="cardUniqueId">The card unique identifier.</param>
        /// <returns>A byte array with the contents of the authenticated registers.</returns>
        public byte[] DumpRegisters(byte[] authKey, byte[] cardUniqueId)
        {
            const byte registerCount = 64;
            var result = new byte[registerCount];
            byte i = 0;

            while (i < registerCount)
            {
                // Check if authenticated
                if (Authenticate(RequestMode.Authenticate1A, i, authKey, cardUniqueId) == Status.AllOk)
                    result[i] = ReadRegister((Register)i);
                else
                    "Authentication error".Error(nameof(RFIDControllerMfrc522));

                i++;
            }

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent()
        {
            _outputPort.PinMode = GpioPinDriveMode.Output;
            _outputPort.Write(true);

            Reset();

            WriteRegister(Register.TModeReg, 0x8D);
            WriteRegister(Register.TPrescalerReg, 0x3E);
            WriteRegister(Register.TReloadRegL, 0x1E);
            WriteRegister(Register.TReloadRegH, 0x00);
            WriteRegister(Register.TxAutoReg, 0x40);
            WriteRegister(Register.ModeReg, 0x3D);

            TurnAntennaOn();
        }

        /// <summary>
        /// Writes the register.
        /// </summary>
        /// <param name="register">The register.</param>
        /// <param name="value">The value.</param>
        private void WriteRegister(byte register, byte value)
        {
            _spiPort.Write(new[]
            {
                (byte) ((register << 1) & 0x7E),
                value
            });
        }

        /// <summary>
        /// Writes the register.
        /// </summary>
        /// <param name="register">The register.</param>
        /// <param name="value">The value.</param>
        private void WriteRegister(Register register, byte value) => WriteRegister((byte)register, value);

        /// <summary>
        /// Writes the command.
        /// </summary>
        /// <param name="value">The value.</param>
        private void WriteCommand(Command value) => WriteRegister(Register.CommandReg, (byte)value);

        /// <summary>
        /// Sets the register bits.
        /// </summary>
        /// <param name="register">The register.</param>
        /// <param name="bitMask">The bit mask.</param>
        private void SetRegisterBits(Register register, byte bitMask) => WriteRegister(register, (byte)(ReadRegister(register) | bitMask));

        /// <summary>
        /// Clears the register bits.
        /// </summary>
        /// <param name="register">The register.</param>
        /// <param name="bitMask">The bit mask.</param>
        private void ClearRegisterBits(Register register, byte bitMask) => WriteRegister(register, (byte)(ReadRegister(register) & (~bitMask)));

        /// <summary>
        /// Calulates the CRC Hash as an array of bytes.
        /// Returns a 2-byte array.
        /// </summary>
        /// <param name="pIndata">The p indata.</param>
        /// <returns>a 2-byte array with the CRC.</returns>
        private byte[] CalulateCrc(byte[] pIndata)
        {
            ClearRegisterBits(Register.DivIrqReg, 0x04);
            SetRegisterBits(Register.FIFOLevelReg, 0x80);
            byte i = 0;
            while (i < pIndata.Length)
            {
                WriteRegister(Register.FIFODataReg, pIndata[i]);
                i++;
            }

            WriteCommand(Command.ComputeCrc);
            i = 0xFF;
            while (true)
            {
                var n = ReadRegister(Register.DivIrqReg);
                i--;
                if (!((i != 0) && !Convert.ToBoolean(n & 0x04)))
                {
                    break;
                }
            }

            return new[] { ReadRegister(Register.CRCResultRegL), ReadRegister(Register.CRCResultRegM) };
        }

        /// <summary>
        /// Authenticates the card using the specified authentication mode.
        /// </summary>
        /// <param name="authMode">The authentication mode.</param>
        /// <param name="blockAddress">The block address.</param>
        /// <param name="sectorkey">The sectorkey.</param>
        /// <param name="cardUid">The card uid.</param>
        /// <returns>A status code.</returns>
        private Status Authenticate(RequestMode authMode, byte blockAddress, byte[] sectorkey, byte[] cardUid)
        {
            // First byte should be the authMode (A or B) Second byte is the trailerBlock (usually 7)
            var buff = new List<byte> { (byte)authMode, blockAddress };

            // Now we need to append the authKey which usually is 6 bytes of 0xFF
            var i = 0;
            while (i < sectorkey.Length)
            {
                buff.Add(sectorkey[i]);
                i++;
            }

            i = 0;

            // Next we append the first 4 bytes of the UID
            while (i < 4)
            {
                buff.Add(cardUid[i]);
                i++;
            }

            // Now we start the authentication itself
            var response = CardSendData(Command.Authenticate, buff.ToArray());

            // Return the status
            return response.Status != Status.AllOk || (ReadRegister(Register.Status2Reg) & 0x08) == 0
                ? Status.Error
                : Status.AllOk;
        }

        #endregion

        #region Support Classes

        /// <summary>
        /// Holds the status and data of a controller response.
        /// </summary>
        public class RfidResponse
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RfidResponse"/> class.
            /// </summary>
            /// <param name="status">The status.</param>
            /// <param name="receivedData">The payload of the received data.</param>
            /// <param name="receivedBitCount">The received bit count.</param>
            internal RfidResponse(Status status, byte[] receivedData, byte receivedBitCount)
            {
                Status = status;
                Data = receivedData;
                DataBitLength = receivedBitCount;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="RfidResponse"/> class.
            /// </summary>
            /// <param name="status">The status.</param>
            /// <param name="previousResponse">The previous response.</param>
            internal RfidResponse(Status status, RfidResponse previousResponse)
            {
                Status = status;
                Data = previousResponse.Data;
                DataBitLength = previousResponse.DataBitLength;
            }

            /// <summary>
            /// Gets the status.
            /// </summary>
            public Status Status { get; }

            /// <summary>
            /// Gets the data.
            /// </summary>
            public byte[] Data { get; }

            /// <summary>
            /// Gets the length of the data.
            /// </summary>
            public byte DataBitLength { get; }
        }

        #endregion
    }
}