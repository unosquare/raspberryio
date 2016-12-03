using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unosquare.RaspberryIO
{
    public enum PictureEncodingFormat
    {
        Jpg,
        Bmp,
        Gif,
        Png,
    }
    public enum PictureSensorMode
    {
        Auto = 0,

    }
    public class PictureArguments
    {
        public int Width { get; set; } = 640;
        public int Height { get; set; } = 480;
        public int Quality { get; set; } = 90;
        public bool AddRawBayerMetadata { get; set; } = false;
        public int TimeoutMilliseconds { get; set; } = 1;
        public PictureEncodingFormat Encoding { get; set; } = PictureEncodingFormat.Jpg;
        public Dictionary<string, string> ExtendedInfo { get; private set; } = new Dictionary<string, string>();
        public bool UseFullPreview { get; set; } = false;
        public bool Burst { get; set; } = false;
        public PictureSensorMode Mode { get; set; } = PictureSensorMode.Auto;
        public bool DisplayPreview { get; set; } = false;
        public int PictureSharpness { get; set; } = 0; // from -100 to 100
        public int PictureContrast { get; set; } = 0; // from -100 to 100
        public int PictureBrightness { get; set; } = 50; // from 0 to 100
        public int PictureSaturation { get; set; } = 0; // from -100 to 100

    }
}
