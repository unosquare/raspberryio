namespace Unosquare.RaspberryIO.Camera
{
    using Swan;
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines a wrapper for the raspistill program and its settings (command-line arguments)
    /// </summary>
    /// <seealso cref="Unosquare.RaspberryIO.Camera.CameraSettingsBase" />
    public class CameraStillSettings : CameraSettingsBase
    {
        private int _rotate;

        /// <summary>
        /// Gets the command file executable.
        /// </summary>
        public override string CommandName => "raspistill";

        /// <summary>
        /// Gets or sets a value indicating whether the preview window (if enabled) uses native capture resolution
        /// This may slow down preview FPS
        /// </summary>
        public bool CaptureDisplayPreviewAtResolution { get; set; } = false;

        /// <summary>
        /// Gets or sets the encoding format the hardware will use for the output.
        /// </summary>
        public CameraImageEncodingFormat CaptureEncoding { get; set; } = CameraImageEncodingFormat.Jpg;

        /// <summary>
        /// Gets or sets the quality for JPEG only encoding mode.
        /// Value ranges from 0 to 100
        /// </summary>
        public int CaptureJpegQuality { get; set; } = 90;

        /// <summary>
        /// Gets or sets a value indicating whether the JPEG encoder should add raw bayer metadata.
        /// </summary>
        public bool CaptureJpegIncludeRawBayerMetadata { get; set; } = false;

        /// <summary>
        /// JPEG EXIF data
        /// Keys and values must be already properly escaped. Otherwise the command will fail.
        /// </summary>
        public Dictionary<string, string> CaptureJpegExtendedInfo { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets a value indicating whether [horizontal flip].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [horizontal flip]; otherwise, <c>false</c>.
        /// </value>
        public bool HorizontalFlip { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether [vertical flip].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [vertical flip]; otherwise, <c>false</c>.
        /// </value>
        public bool VerticalFlip { get; set; } = false;

        public int Rotate
        {
            get => _rotate;
            set
            {
                if (value < 0 || value > 359)
                {
                    throw new ArgumentOutOfRangeException("Valid range 0-359");
                }

                _rotate = value;
            }
        }

        /// <summary>
        /// Creates the process arguments.
        /// </summary>
        /// <returns>The process command line string</returns>
        public override string CreateProcessArguments()
        {
            var sb = new StringBuilder(base.CreateProcessArguments());
            sb.Append($" -e {CaptureEncoding.ToString().ToLowerInvariant()}");

            // JPEG Encoder specific arguments
            if (CaptureEncoding == CameraImageEncodingFormat.Jpg)
            {
                sb.Append($" -q {CaptureJpegQuality.Clamp(0, 100).ToString(Ci)}");

                if (CaptureJpegIncludeRawBayerMetadata)
                    sb.Append(" -r");

                // JPEG EXIF data
                if (CaptureJpegExtendedInfo.Count > 0)
                {
                    foreach (var kvp in CaptureJpegExtendedInfo)
                    {
                        if (string.IsNullOrWhiteSpace(kvp.Key) || string.IsNullOrWhiteSpace(kvp.Value))
                            continue;

                        sb.Append($" -x \"{kvp.Key.Replace("\"", "'")}={kvp.Value.Replace("\"", "'")}\"");
                    }
                }
            }

            // Display preview settings
            if (CaptureDisplayPreview && CaptureDisplayPreviewAtResolution) sb.Append(" -fp");

            if (Rotate != 0) sb.Append($" -rot {Rotate}");

            if (HorizontalFlip) sb.Append(" -hf");

            if (VerticalFlip) sb.Append(" -vf");

            var commandArgs = sb.ToString();
            $"{CommandName} {commandArgs}".Trace(Pi.LoggerSource);
            return commandArgs;
        }
    }
}