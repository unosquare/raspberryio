using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        static private readonly CultureInfo CI = CultureInfo.InvariantCulture;
        private const string Executable = "raspistill";

        public int Width { get; set; } = 640;
        public int Height { get; set; } = 480;
        public int Quality { get; set; } = 90;
        public bool AddRawBayerMetadata { get; set; } = false;
        public int TimeoutMilliseconds { get; set; } = 1; // let the lens open
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

        public string GetProcessArguments()
        {
            return $"-o - -t {TimeoutMilliseconds.ToString(CI)} -w {Width.ToString(CI)} -h {Height.ToString(CI)}";
        }

        public Process CreateProcess()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Arguments = GetProcessArguments(),
                    CreateNoWindow = true,
                    FileName = Executable,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                },
            };

            return process;
        }

    }
}
