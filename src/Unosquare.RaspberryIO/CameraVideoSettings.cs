namespace Unosquare.RaspberryIO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CameraVideoSettings : CameraSettingsBase
    {
        protected override string CommandFile
        {
            get
            {
                return "raspivid";
            }
        }

        public override string CreateProcessArguments()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use bits per second, so 10Mbits/s would be -b 10000000. For H264, 1080p30 a high quality bitrate would be 15Mbits/s or more. 
        /// Maximum bitrate is 25Mbits/s (-b 25000000), but much over 17Mbits/s won't show noticeable improvement at 1080p30.
        /// Default -1
        /// </summary>
        public int Bitrate { get; set; } = -1;

    }
}
