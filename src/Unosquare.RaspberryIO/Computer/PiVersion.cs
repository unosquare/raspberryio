namespace Unosquare.RaspberryIO.Computer
{
    /// <summary>
    /// Defines the board revision codes of the different versions of the Raspberry Pi
    /// http://www.raspberrypi-spy.co.uk/2012/09/checking-your-raspberry-pi-board-version/.
    /// https://www.raspberrypi.org/documentation/hardware/raspberrypi/revision-codes/README.md.
    /// </summary>
    public enum PiVersion
    {
        /// <summary>
        /// The unknown version
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The model B Rev1
        /// </summary>
        ModelBRev1 = 0x0002,

        /// <summary>
        /// The model B Rev1 ECN0001
        /// </summary>
        ModelBRev1ECN0001 = 0x0003,

        /// <summary>
        /// The model B Rev2 Sony
        /// </summary>
        ModelBRev2x04 = 0x0004,

        /// <summary>
        /// The model B Rev2 Qisda
        /// </summary>
        ModelBRev2x05 = 0x0005,

        /// <summary>
        /// The model B Rev2 Egoman
        /// </summary>
        ModelBRev2x06 = 0x0006,

        /// <summary>
        /// The model A Rev2 Egoman
        /// </summary>
        ModelAx07 = 0x0007,

        /// <summary>
        /// The model A Rev2 Sony
        /// </summary>
        ModelAx08 = 0x0008,

        /// <summary>
        /// The model A Rev2 Qisda
        /// </summary>
        ModelAx09 = 0x0009,

        /// <summary>
        /// The model B Rev2 (512MB) Egoman
        /// </summary>
        ModelBRev2x0d = 0x000d,

        /// <summary>
        /// The model B Rev2 (512MB) Sony
        /// </summary>
        ModelBRev2x0e = 0x000e,

        /// <summary>
        /// The model B Rev2 (512MB) Egoman
        /// </summary>
        ModelBRev2x0f = 0x000f,

        /// <summary>
        /// The model B+ Rev1 Sony
        /// </summary>
        ModelBPlus0x10 = 0x0010,

        /// <summary>
        /// The compute module 1 Sony
        /// </summary>
        ComputeModule0x11 = 0x0011,

        /// <summary>
        /// The model A+ Rev1.1 Sony
        /// </summary>
        ModelAPlus0x12 = 0x0012,

        /// <summary>
        /// The model B+ Rev1.2 Embest
        /// </summary>
        ModelBPlus0x13 = 0x0013,

        /// <summary>
        /// The compute module 1 Embest
        /// </summary>
        ComputeModule0x14 = 0x0014,

        /// <summary>
        /// The model A+ Rev1.1 Embest
        /// </summary>
        ModelAPlus0x15 = 0x0015,

        /// <summary>
        /// The model A+ Rev1.1 (512MB) Sony
        /// </summary>
        ModelAPlus1v1Sony = 900021,
        
        /// <summary>
        /// The model B+ Rev1.2 sony
        /// </summary>
        ModelBPlus1v2Sony = 900032,

        /// <summary>
        /// The Pi Zero Rev1.2 Sony
        /// </summary>
        PiZero1v2 = 0x900092,

        /// <summary>
        /// The Pi Zero Rev1.3 SOny
        /// </summary>
        PiZero1v3 = 0x900093,

        /// <summary>
        /// The Pi Zero W Rev1.1
        /// </summary>
        PiZeroW = 0x9000c1,

        /// <summary>
        /// The Pi 3 model A+ Sony
        /// </summary>
        Pi3ModelAPlus = 0x9020e0,

        /// <summary>
        /// The Pi Zero Rev1.2 Embest
        /// </summary>
        PiZero1v2Embest = 0x920092,

        /// <summary>
        /// The Pi Zero Rev1.3 Embest
        /// </summary>
        PiZero1v3Embest = 0x920093,

        /// <summary>
        /// The Pi 2 model B Rev1.0 Sony
        /// </summary>
        Pi2ModelB1v0Sony = 0xa01040,

        /// <summary>
        /// The Pi 2 model B Rev1.1 Sony
        /// </summary>
        Pi2ModelB1v1Sony = 0xa01041,

        /// <summary>
        /// The Pi 3 model B Rev1.2 Sony
        /// </summary>
        Pi3ModelBSony = 0xa02082,

        /// <summary>
        /// The compute module 3 Rev1.0 Sony
        /// </summary>
        ComputeModule3Sony = 0xa020a0,

        /// <summary>
        /// The Pi 3 model B+ Rev1.3 Sony
        /// </summary>
        Pi3ModelBPlusSony = 0xa020d3,

        /// <summary>
        /// The Pi 2 model B Rev1.1 Embest
        /// </summary>
        Pi2ModelB1v1Embest = 0xa21041,

        /// <summary>
        /// The Pi 2 model B Rev1.2 Embest
        /// </summary>
        Pi2ModelB1v2 = 0xa22042,

        /// <summary>
        /// The Pi 3 model B Rev1.2 Embest
        /// </summary>
        Pi3ModelBEmbest = 0xa22082,

        /// <summary>
        /// The compute module 3 Rev1.0 Embest
        /// </summary>
        ComputeModule3Embest = 0xa220a0,

        /// <summary>
        /// The Pi 3 model B Rev1.2 Sony Japan
        /// </summary>
        Pi3ModelBSonyJapan = 0xa32082,

        /// <summary>
        /// The Pi 3 model B Rev1.2 Stadium
        /// </summary>
        Pi3ModelBStadium = 0xa52082,

        /// <summary>
        /// The compute module 3+ Rev 1.0 Sony
        /// </summary>
        ComputeModule3PlusSony = 0xa02100,
    }
}