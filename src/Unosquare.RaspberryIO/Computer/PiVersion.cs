namespace Unosquare.RaspberryIO.Computer
{
    /// <summary>
    /// Defines the board revision codes of the different versions of the Raspberry Pi
    /// http://www.raspberrypi-spy.co.uk/2012/09/checking-your-raspberry-pi-board-version/
    /// </summary>
    public enum PiVersion
    {
        /// <summary>
        /// The unknown version
        /// </summary>
        Unknown = 0,
        
        /// <summary>
        /// The model b rev1
        /// </summary>
        ModelBRev1 = 0x0002,
        
        /// <summary>
        /// The model b rev1 ec N0001
        /// </summary>
        ModelBRev1ECN0001 = 0x0003,
        
        /// <summary>
        /// The model b rev2x04
        /// </summary>
        ModelBRev2x04 = 0x0004,
        
        /// <summary>
        /// The model b rev2x05
        /// </summary>
        ModelBRev2x05 = 0x0005,
        
        /// <summary>
        /// The model b rev2x06
        /// </summary>
        ModelBRev2x06 = 0x0006,
        
        /// <summary>
        /// The model ax07
        /// </summary>
        ModelAx07 = 0x0007,
        
        /// <summary>
        /// The model ax08
        /// </summary>
        ModelAx08 = 0x0008,
        
        /// <summary>
        /// The model ax09
        /// </summary>
        ModelAx09 = 0x0009,
        
        /// <summary>
        /// The model b rev2x0d
        /// </summary>
        ModelBRev2x0d,
        
        /// <summary>
        /// The model b rev2x0e
        /// </summary>
        ModelBRev2x0e,
        
        /// <summary>
        /// The model b rev2x0f
        /// </summary>
        ModelBRev2x0f = 0x000f,
        
        /// <summary>
        /// The model b plus0x10
        /// </summary>
        ModelBPlus0x10 = 0x0010,
        
        /// <summary>
        /// The model b plus0x13
        /// </summary>
        ModelBPlus0x13 = 0x0013,
        
        /// <summary>
        /// The compute module0x11
        /// </summary>
        ComputeModule0x11 = 0x0011,
        
        /// <summary>
        /// The compute module0x14
        /// </summary>
        ComputeModule0x14 = 0x0014,
        
        /// <summary>
        /// The model a plus0x12
        /// </summary>
        ModelAPlus0x12 = 0x0012,
        
        /// <summary>
        /// The model a plus0x15
        /// </summary>
        ModelAPlus0x15 = 0x0015,
        
        /// <summary>
        /// The pi2 model B1V1 sony
        /// </summary>
        Pi2ModelB1v1Sony = 0xa01041,
        
        /// <summary>
        /// The pi2 model B1V1 embest
        /// </summary>
        Pi2ModelB1v1Embest = 0xa21041,
        
        /// <summary>
        /// The pi2 model B1V2
        /// </summary>
        Pi2ModelB1v2 = 0xa22042,
        
        /// <summary>
        /// The pi zero1v2
        /// </summary>
        PiZero1v2 = 0x900092,
        
        /// <summary>
        /// The pi zero1v3
        /// </summary>
        PiZero1v3 = 0x900093,
        
        /// <summary>
        /// The pi3 model b sony
        /// </summary>
        Pi3ModelBSony = 0xa02082,
        
        /// <summary>
        /// The pi3 model b embest
        /// </summary>
        Pi3ModelBEmbest = 0xa22082
    }
}