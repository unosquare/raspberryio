namespace Unosquare.RaspberryIO.Peripherals
{
    using Swan;
    using System;
    using System.Collections.Generic;
    using Gpio;

    public class Mfrc522Controller
    {
        const byte NRSTPD = 22;

        const byte MAX_LEN = 16;

        const byte PCD_IDLE = 0x00;
        const byte PCD_AUTHENT = 0x0E;
        const byte PCD_RECEIVE = 0x08;
        const byte PCD_TRANSMIT = 0x04;
        const byte PCD_TRANSCEIVE = 0x0C;
        const byte PCD_RESETPHASE = 0x0F;
        const byte PCD_CALCCRC = 0x03;

        public const byte PICC_REQIDL = 0x26;
        public const byte PICC_REQALL = 0x52;
        public const byte PICC_ANTICOLL = 0x93;
        public const byte PICC_SElECTTAG = 0x93;
        public const byte PICC_AUTHENT1A = 0x60;
        public const byte PICC_AUTHENT1B = 0x61;
        public const byte PICC_READ = 0x30;
        public const byte PICC_WRITE = 0xA0;
        public const byte PICC_DECREMENT = 0xC0;
        public const byte PICC_INCREMENT = 0xC1;
        public const byte PICC_RESTORE = 0xC2;
        public const byte PICC_TRANSFER = 0xB0;
        public const byte PICC_HALT = 0x50;

        public const byte MI_OK = 0;
        public const byte MI_NOTAGERR = 1;
        public const byte MI_ERR = 2;

        const byte Reserved00 = 0x00;
        const byte CommandReg = 0x01;
        const byte CommIEnReg = 0x02;
        const byte DivlEnReg = 0x03;
        const byte CommIrqReg = 0x04;
        const byte DivIrqReg = 0x05;
        const byte ErrorReg = 0x06;
        const byte Status1Reg = 0x07;
        const byte Status2Reg = 0x08;
        const byte FIFODataReg = 0x09;
        const byte FIFOLevelReg = 0x0A;
        const byte WaterLevelReg = 0x0B;
        const byte ControlReg = 0x0C;
        const byte BitFramingReg = 0x0D;
        const byte CollReg = 0x0E;
        const byte Reserved01 = 0x0F;

        const byte Reserved10 = 0x10;
        const byte ModeReg = 0x11;
        const byte TxModeReg = 0x12;
        const byte RxModeReg = 0x13;
        const byte TxControlReg = 0x14;
        const byte TxAutoReg = 0x15;
        const byte TxSelReg = 0x16;
        const byte RxSelReg = 0x17;
        const byte RxThresholdReg = 0x18;
        const byte DemodReg = 0x19;
        const byte Reserved11 = 0x1A;
        const byte Reserved12 = 0x1B;
        const byte MifareReg = 0x1C;
        const byte Reserved13 = 0x1D;
        const byte Reserved14 = 0x1E;
        const byte SerialSpeedReg = 0x1F;

        const byte Reserved20 = 0x20;
        const byte CRCResultRegM = 0x21;
        const byte CRCResultRegL = 0x22;
        const byte Reserved21 = 0x23;
        const byte ModWidthReg = 0x24;
        const byte Reserved22 = 0x25;
        const byte RFCfgReg = 0x26;
        const byte GsNReg = 0x27;
        const byte CWGsPReg = 0x28;
        const byte ModGsPReg = 0x29;
        const byte TModeReg = 0x2A;
        const byte TPrescalerReg = 0x2B;
        const byte TReloadRegH = 0x2C;
        const byte TReloadRegL = 0x2D;
        const byte TCounterValueRegH = 0x2E;
        const byte TCounterValueRegL = 0x2F;

        const byte Reserved30 = 0x30;
        const byte TestSel1Reg = 0x31;
        const byte TestSel2Reg = 0x32;
        const byte TestPinEnReg = 0x33;
        const byte TestPinValueReg = 0x34;
        const byte TestBusReg = 0x35;
        const byte AutoTestReg = 0x36;
        const byte VersionReg = 0x37;
        const byte AnalogTestReg = 0x38;
        const byte TestDAC1Reg = 0x39;
        const byte TestDAC2Reg = 0x3A;
        const byte TestADCReg = 0x3B;
        const byte Reserved31 = 0x3C;
        const byte Reserved32 = 0x3D;
        const byte Reserved33 = 0x3E;
        const byte Reserved34 = 0x3F;

        private readonly SpiChannel _spi;

        public Mfrc522Controller()
        {
            Pi.Spi.Channel0Frequency = 500000;
            _spi = Pi.Spi.Channel0;

            var nrstdpPin = Pi.Gpio[NRSTPD];
            nrstdpPin.PinMode = GpioPinDriveMode.Output;
            nrstdpPin.Write(true);

            Init();
        }

        public void Init()
        {
            var nrstdpPin = Pi.Gpio[NRSTPD];
            nrstdpPin.PinMode = GpioPinDriveMode.Output;
            nrstdpPin.Write(true);

            Reset();

            WriteSpi(TModeReg, 0x8D);
            WriteSpi(TPrescalerReg, 0x3E);
            WriteSpi(TReloadRegL, 30);
            WriteSpi(TReloadRegH, 0);

            WriteSpi(TxAutoReg, 0x40);
            WriteSpi(ModeReg, 0x3D);

            AntennaOn();
        }

        private void Reset()
        {
            WriteSpi(CommandReg, PCD_RESETPHASE);
        }

        public void WriteSpi(byte addr, byte val)
        {
            _spi.Write(new byte[]
            {
                (byte) ((addr << 1) & 0x7E),
                val
            });
        }

        public byte ReadSpi(byte addr)
        {
            var val = _spi.SendReceive(new byte[]
            {
                (byte) (((addr << 1) & 0x7E) | 0x80),
                0
            });
            return val[1];
        }

        private void SetBitMask(byte reg, byte mask)
        {
            var tmp = ReadSpi(reg);
            WriteSpi(reg, (byte)(tmp | mask));
        }

        private void ClearBitMask(byte reg, byte mask)
        {
            var tmp = ReadSpi(reg);
            WriteSpi(reg, (byte)(tmp & (~mask)));
        }

        private void AntennaOn()
        {
            var temp = ReadSpi(TxControlReg);
            //if (~(temp & 0x03) == 1)
            //{
            SetBitMask(TxControlReg, 0x03);
            //}
        }

        private void AntennaOff()
        {
            ClearBitMask(TxControlReg, 0x03);
        }

        private (byte status, byte[] backData, byte backLen) ToCard(byte command, byte[] sendData)
        {
            var backData = new List<byte>();
            byte backLen = 0;
            var status = MI_ERR;
            byte irqEn = 0x00;
            byte waitIRq = 0x00;
            byte n = 0;
            var i = 0;

            if (command == PCD_AUTHENT)
            {
                irqEn = 0x12;
                waitIRq = 0x10;
            }

            if (command == PCD_TRANSCEIVE)
            {
                irqEn = 0x77;
                waitIRq = 0x30;
            }

            WriteSpi(CommIEnReg, (byte)(irqEn | 0x80));
            ClearBitMask(CommIrqReg, 0x80);
            SetBitMask(FIFOLevelReg, 0x80);

            WriteSpi(CommandReg, PCD_IDLE);

            while (i < sendData.Length)
            {
                WriteSpi(FIFODataReg, sendData[i]);
                i++;
            }

            WriteSpi(CommandReg, command);

            if (command == PCD_TRANSCEIVE)
            {
                SetBitMask(BitFramingReg, 0x80);
            }

            i = 2000;
            while (true)
            {
                n = ReadSpi(CommIrqReg);
                i -= 1;
                if (Convert.ToBoolean(
                    ~Convert.ToInt32(((i != 0) && Convert.ToBoolean(~(n & 0x01)) &&
                                      Convert.ToBoolean(~(n & waitIRq))))))
                {
                    break;
                }
            }

            ClearBitMask(BitFramingReg, 0x80);

            if (i == 0) return new Tuple<byte, byte[], byte>(status, backData.ToArray(), backLen);

            if ((ReadSpi(ErrorReg) & 0x1B) == 0x00)
            {
                status = MI_OK;

                if (Convert.ToBoolean(n & irqEn & 0x01))
                {
                    status = MI_NOTAGERR;
                }

                if (command == PCD_TRANSCEIVE)
                {
                    n = ReadSpi(FIFOLevelReg);
                    byte? lastBits = (byte)(ReadSpi(ControlReg) & 0x07);
                    if (lastBits != 0)
                    {
                        backLen = (byte)(((n - 1) * 8) + (byte)lastBits);
                    }
                    else
                    {
                        backLen = (byte)(n * 8);
                    }

                    if (n == 0)
                    {
                        n = 1;
                    }

                    if (n > MAX_LEN)
                    {
                        n = MAX_LEN;
                    }

                    i = 0;
                    while (i < n)
                    {
                        backData.Add(ReadSpi(FIFODataReg));
                        i++;
                    }
                }
            }
            else
            {
                status = MI_ERR;
            }

            return (status, backData.ToArray(), backLen);
        }

        public Tuple<byte, int> Request(byte reqMode)
        {
            var tagType = new List<byte> {reqMode};

            var result = ToCard(PCD_TRANSCEIVE, tagType.ToArray());
            var status = result.Item1;
            var backBits = result.Item3;

            if ((status != MI_OK) | (backBits != 0x10))
            {
                status = MI_ERR;
            }

            return new Tuple<byte, int>(status, backBits);
        }

        public Tuple<byte, byte[]> Anticoll()
        {
            byte serNumCheck = 0;

            var serNum = new List<byte>();

            WriteSpi(BitFramingReg, 0x00);

            serNum.Add(PICC_ANTICOLL);
            serNum.Add(0x20);

            var result = ToCard(PCD_TRANSCEIVE, serNum.ToArray());
            var status = result.Item1;
            var backData = result.Item2;

            var i = 0;
            if (status == MI_OK)
            {
                i = 0;
            }

            if (backData.Length == 5)
            {
                while (i < 4)
                {
                    serNumCheck = (byte)(serNumCheck ^ backData[i]);
                    i = i + 1;
                }

                if (serNumCheck != backData[i])
                {
                    status = MI_ERR;
                }
            }
            else
            {
                status = MI_ERR;
            }

            return new Tuple<byte, byte[]>(status, backData);
        }

        private byte[] CalulateCRC(byte[] pIndata)
        {
            ClearBitMask(DivIrqReg, 0x04);
            SetBitMask(FIFOLevelReg, 0x80);
            byte i = 0;
            while (i < pIndata.Length)
            {
                WriteSpi(FIFODataReg, pIndata[i]);
                i++;
            }

            WriteSpi(CommandReg, PCD_CALCCRC);
            i = 0xFF;
            while (true)
            {
                var n = ReadSpi(DivIrqReg);
                i--;
                if (!((i != 0) && !Convert.ToBoolean(n & 0x04)))
                {
                    break;
                }
            }

            var pOutData = new List<byte> {ReadSpi(CRCResultRegL), ReadSpi(CRCResultRegM)};
            return pOutData.ToArray();
        }

        public byte SelectTag(byte[] serNum)
        {
            var buf = new List<byte> { PICC_SElECTTAG, 0x70 };
            var i = 0;
            while (i < 5)
            {
                buf.Add(serNum[i]);
                i++;
            }

            var pOut = CalulateCRC(buf.ToArray());
            buf.Add(pOut[0]);
            buf.Add(pOut[1]);
            var result = ToCard(PCD_TRANSCEIVE, buf.ToArray());
            var status = result.Item1;
            var backData = result.Item2;
            var backBits = result.Item3;

            if (status != MI_OK || backBits != 0x18) 
                return 0;

            $"Size: {backData[0]}".Debug();
            return backData[0];

        }

        public byte Auth(byte authMode, byte blockAddr, byte[] sectorkey, byte[] serNum)
        {
            // First byte should be the authMode (A or B) Second byte is the trailerBlock (usually 7)
            var buff = new List<byte> { authMode, blockAddr };

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
                buff.Add(serNum[i]);
                i++;
            }

            // Now we start the authentication itself
            var result = ToCard(PCD_AUTHENT, buff.ToArray());
            var status = result.Item1;

            // Check if an error occurred
            if (status != MI_OK)
            {
                "AUTH ERROR!!".Error();
            }

            if ((ReadSpi(Status2Reg) & 0x08) == 0)
            {
                "AUTH ERROR(status2reg & 0x08) != 0".Info();
            }

            // Return the status
            return status;
        }

        public void StopCrypto1()
        {
            ClearBitMask(Status2Reg, 0x08);
        }

        private void Write(byte blockAddr, byte[] writeData)
        {
            var buff = new List<byte> {PICC_WRITE, blockAddr};
            var crc = CalulateCRC(buff.ToArray());
            buff.Add(crc[0]);
            buff.Add(crc[1]);
            var result = ToCard(PCD_TRANSCEIVE, buff.ToArray());
            var status = result.Item1;
            var backData = result.Item2;
            var backLen = result.Item3;

            if (status != MI_OK || backLen != 4 || (backData[0] & 0x0F) != 0x0A)
            {
                status = MI_ERR;
            }

            $"{backLen} backdata &0x0F == 0x0A {backData[0] & 0x0F}".Debug();

            if (status != MI_OK) return;

            var i = 0;
            var buf = new List<byte>();
            while (i < 16)
            {
                buf.Add(writeData[i]);
                i++;
            }

            crc = CalulateCRC(buf.ToArray());
            buf.Add(crc[0]);
            buf.Add(crc[1]);
            result = ToCard(PCD_TRANSCEIVE, buf.ToArray());
            status = result.Item1;
            backData = result.Item2;
            backLen = result.Item3;
            if (status != MI_OK || backLen != 4 || (backData[0] & 0x0F) != 0x0A)
            {
                "Error while writing".Error();
            }

            if (status == MI_OK)
            {
                "Data written".Info();
            }
        }

        public void DumpClassic1K(byte[] key, byte[] uid)
        {
            byte i = 0;

            while (i < 64)
            {
                var status = Auth(PICC_AUTHENT1A, i, key, uid);

                // Check if authenticated
                if (status == MI_OK)
                {
                    ReadSpi(i);
                }
                else
                {
                    "Authentication error".Error();
                }

                i++;
            }
        }
    }
}
