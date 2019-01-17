namespace Unosquare.RaspberryIO.Peripherals
{
    using Swan;
    using System;
    using System.Collections.Generic;
    using Abstractions;

    /// <summary>
    /// Read and write different types of Radio-Frequency IDentification (RFID) cards on your
    /// Raspberry Pi using a RC522 based reader connected via the Serial Peripheral Interface (SPI) interface.
    /// </summary>
    public partial class RFIDControllerMfrc522
    {
        /// <summary>
        /// The default authentication key.
        /// </summary>
        public static readonly byte[] DefaultAuthKey = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

        private readonly ISpiChannel _spiPort;
        private readonly IGpioPin _outputPort;

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
        /// <param name="spiFrequency">The spi frequency.</param>
        /// <param name="outputPort">The output port.</param>
        public RFIDControllerMfrc522(ISpiChannel spiPort, int spiFrequency, IGpioPin outputPort)
        {
            Pi.Spi.Channel0Frequency = spiFrequency;
            _spiPort = spiPort;
            _outputPort = outputPort;
            InitializeComponent();
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
            }
            while (--i != 0 && (currentRegisterValue & 0x01) == 0 &&
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
            var crc = CalculateCrc(buff.ToArray());
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

            crc = CalculateCrc(buf.ToArray());
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
            var crc = CalculateCrc(buff.ToArray());
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

            var crcHashCode = CalculateCrc(payloadBuffer.ToArray());
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
                (byte)((register << 1) & 0x7E),
                value,
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
        /// Calculates the CRC Hash as an array of bytes.
        /// Returns a 2-byte array.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>a 2-byte array with the CRC.</returns>
        private byte[] CalculateCrc(byte[] data)
        {
            ClearRegisterBits(Register.DivIrqReg, 0x04);
            SetRegisterBits(Register.FIFOLevelReg, 0x80);
            byte i = 0;
            while (i < data.Length)
            {
                WriteRegister(Register.FIFODataReg, data[i]);
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