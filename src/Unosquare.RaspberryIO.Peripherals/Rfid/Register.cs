namespace Unosquare.RaspberryIO.Peripherals
{
    public partial class RFIDControllerMfrc522
    {
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
    }
}