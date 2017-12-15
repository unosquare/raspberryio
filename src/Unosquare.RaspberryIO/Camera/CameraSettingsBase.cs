namespace Unosquare.RaspberryIO.Camera
{
    using Swan;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// A base class to implement raspistill and raspivid wrappers
    /// Full documentation available at
    /// https://www.raspberrypi.org/documentation/raspbian/applications/camera.md
    /// </summary>
    public abstract class CameraSettingsBase
    {
        /// <summary>
        /// The Invariant Culture shorthand
        /// </summary>
        protected static readonly CultureInfo Ci = CultureInfo.InvariantCulture;

        #region Capture Settings

        /// <summary>
        /// Gets or sets the timeout milliseconds.
        /// Default value is 5000
        /// Recommended value is at least 300 in order to let the light collectors open
        /// </summary>
        public int CaptureTimeoutMilliseconds { get; set; } = 5000;

        /// <summary>
        /// Gets or sets a value indicating whether or not to show a preview window on the screen
        /// </summary>
        public bool CaptureDisplayPreview { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether a preview window is shown in full screen  mode if enabled
        /// </summary>
        public bool CaptureDisplayPreviewInFullScreen { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether video stabilization should be enabled.
        /// </summary>
        public bool CaptureVideoStabilizationEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the display preview opacity only if the display preview property is enabled.
        /// </summary>
        public byte CaptureDisplayPreviewOpacity { get; set; } = 255;

        /// <summary>
        /// Gets or sets the capture sensor region of interest in relative coordinates.
        /// </summary>
        public CameraRect CaptureSensorRoi { get; set; } = CameraRect.Default;

        /// <summary>
        /// Gets or sets the capture shutter speed in microseconds.
        /// Default -1, Range 0 to 6000000 (equivalent to 6 seconds)
        /// </summary>
        public int CaptureShutterSpeedMicroseconds { get; set; } = -1;

        /// <summary>
        /// Gets or sets the exposure mode.
        /// </summary>
        public CameraExposureMode CaptureExposure { get; set; } = CameraExposureMode.Auto;

        /// <summary>
        /// Gets or sets the picture EV compensation. Default is 0, Range is -10 to 10
        /// Camera exposure compensation is commonly stated in terms of EV units; 
        /// 1 EV is equal to one exposure step (or stop), corresponding to a doubling of exposure. 
        /// Exposure can be adjusted by changing either the lens f-number or the exposure time; 
        /// which one is changed usually depends on the camera's exposure mode.
        /// </summary>
        public int CaptureExposureCompensation { get; set; } = 0;

        /// <summary>
        /// Gets or sets the capture metering mode.
        /// </summary>
        public CameraMeteringMode CaptureMeteringMode { get; set; } = CameraMeteringMode.Average;

        /// <summary>
        /// Gets or sets the automatic white balance mode. By default it is set to Auto
        /// </summary>
        public CameraWhiteBalanceMode CaptureWhiteBalanceControl { get; set; } = CameraWhiteBalanceMode.Auto;

        /// <summary>
        /// Gets or sets the capture white balance gain on the blue channel. Example: 1.25
        /// Only takes effect if White balance control is set to off.
        /// Default is 0
        /// </summary>
        public decimal CaptureWhiteBalanceGainBlue { get; set; } = 0M;

        /// <summary>
        /// Gets or sets the capture white balance gain on the red channel. Example: 1.75
        /// Only takes effect if White balance control is set to off.
        /// Default is 0
        /// </summary>
        public decimal CaptureWhiteBalanceGainRed { get; set; } = 0M;

        /// <summary>
        /// Gets or sets the dynamic range compensation.
        /// DRC changes the images by increasing the range of dark areas, and decreasing the brighter areas. This can improve the image in low light areas.
        /// </summary>
        public CameraDynamicRangeCompensation CaptureDynamicRangeCompensation { get; set; } =
            CameraDynamicRangeCompensation.Off;

        #endregion

        #region Image Properties

        /// <summary>
        /// Gets or sets the width of the picture to take.
        /// Less than or equal to 0 in either width or height means maximum resolution available.
        /// </summary>
        public int CaptureWidth { get; set; } = 640;

        /// <summary>
        /// Gets or sets the height of the picture to take.
        /// Less than or equal to 0 in either width or height means maximum resolution available.
        /// </summary>
        public int CaptureHeight { get; set; } = 480;

        /// <summary>
        /// Gets or sets the picture sharpness. Default is 0, Range form -100 to 100
        /// </summary>
        public int ImageSharpness { get; set; } = 0;

        /// <summary>
        /// Gets or sets the picture contrast. Default is 0, Range form -100 to 100
        /// </summary>
        public int ImageContrast { get; set; } = 0;

        /// <summary>
        /// Gets or sets the picture brightness. Default is 50, Range form 0 to 100
        /// </summary>
        public int ImageBrightness { get; set; } = 50; // from 0 to 100

        /// <summary>
        /// Gets or sets the picture saturation. Default is 0, Range form -100 to 100
        /// </summary>
        public int ImageSaturation { get; set; } = 0;

        /// <summary>
        /// Gets or sets the picture ISO. Default is -1 Range is 100 to 800
        /// The higher the value, the more light the sensor absorbs
        /// </summary>
        public int ImageISO { get; set; } = -1;

        /// <summary>
        /// Gets or sets the image capture effect to be applied.
        /// </summary>
        public CameraImageEffect ImageEffect { get; set; } = CameraImageEffect.None;

        /// <summary>
        /// Gets or sets the color effect U coordinates. 
        /// Default is -1, Range is 0 to 255
        /// 128:128 should be effectively a monochrome image.
        /// </summary>
        public int ImageColorEffectU { get; set; } = -1; // 0 to 255

        /// <summary>
        /// Gets or sets the color effect V coordinates. 
        /// Default is -1, Range is 0 to 255
        /// 128:128 should be effectively a monochrome image.
        /// </summary>
        public int ImageColorEffectV { get; set; } = -1; // 0 to 255

        /// <summary>
        /// Gets or sets the image rotation. Default is no rotation
        /// </summary>
        public CameraImageRotation ImageRotation { get; set; } = CameraImageRotation.None;

        /// <summary>
        /// Gets or sets a value indicating whether the image should be flipped horizontally.
        /// </summary>
        public bool ImageFlipHorizontally { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the image should be flipped vertically.
        /// </summary>
        public bool ImageFlipVertically { get; set; }

        /// <summary>
        /// Gets or sets the image annotations using a bitmask (or flags) notation.
        /// Apply a bitwise OR to the enumeration to include multiple annotations
        /// </summary>
        public CameraAnnotation ImageAnnotations { get; set; } = CameraAnnotation.None;

        /// <summary>
        /// Gets or sets the image annotations text.
        /// Text may include date/time placeholders by using the '%' character, as used by strftime.
        /// Example: ABC %Y-%m-%d %X will output ABC 2015-10-28 20:09:33
        /// </summary>
        public string ImageAnnotationsText { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the font size of the text annotations
        /// Default is -1, range is 6 to 160
        /// </summary>
        public int ImageAnnotationFontSize { get; set; } = -1;

        /// <summary>
        /// Gets or sets the color of the text annotations.
        /// </summary>
        /// <value>
        /// The color of the image annotation font.
        /// </value>
        public CameraColor ImageAnnotationFontColor { get; set; } = null;

        /// <summary>
        /// Gets or sets the background color for text annotations.
        /// </summary>
        /// <value>
        /// The image annotation background.
        /// </value>
        public CameraColor ImageAnnotationBackground { get; set; } = null;

        #endregion

        #region Interface

        /// <summary>
        /// Gets the command file executable.
        /// </summary>
        public abstract string CommandName { get; }

        /// <summary>
        /// Creates the process arguments.
        /// </summary>
        /// <returns></returns>
        public virtual string CreateProcessArguments()
        {
            var sb = new StringBuilder();
            sb.Append("-o -"); // output to standard output as opposed to a file.
            sb.Append($" -t {(CaptureTimeoutMilliseconds < 0 ? "0" : CaptureTimeoutMilliseconds.ToString(Ci))}");

            // Basic Width and height
            if (CaptureWidth > 0 && CaptureHeight > 0)
            {
                sb.Append($" -w {CaptureWidth.ToString(Ci)}");
                sb.Append($" -h {CaptureHeight.ToString(Ci)}");
            }

            // Display Preview
            if (CaptureDisplayPreview)
            {
                if (CaptureDisplayPreviewInFullScreen) sb.Append(" -f");
                if (CaptureDisplayPreviewOpacity != byte.MaxValue)
                    sb.Append($" -op {CaptureDisplayPreviewOpacity.ToString(Ci)}");
            }
            else
            {
                sb.Append(" -n"); // no preview
            }

            // Picture Settings
            if (ImageSharpness != 0)
                sb.Append($" -sh {ImageSharpness.Clamp(-100, 100).ToString(Ci)}");

            if (ImageContrast != 0)
                sb.Append($" -co {ImageContrast.Clamp(-100, 100).ToString(Ci)}");

            if (ImageBrightness != 50)
                sb.Append($" -br {ImageBrightness.Clamp(0, 100).ToString(Ci)}");

            if (ImageSaturation != 0)
                sb.Append($" -sa {ImageSaturation.Clamp(-100, 100).ToString(Ci)}");

            if (ImageISO >= 100)
                sb.Append($" -ISO {ImageISO.Clamp(100, 800).ToString(Ci)}");

            if (CaptureVideoStabilizationEnabled)
                sb.Append(" -vs");

            if (CaptureExposureCompensation != 0)
                sb.Append($" -ev {CaptureExposureCompensation.Clamp(-10, 10).ToString(Ci)}");

            if (CaptureExposure != CameraExposureMode.Auto)
                sb.Append($" -ex {CaptureExposure.ToString().ToLowerInvariant()}");

            if (CaptureWhiteBalanceControl != CameraWhiteBalanceMode.Auto)
                sb.Append($" -awb {CaptureWhiteBalanceControl.ToString().ToLowerInvariant()}");

            if (ImageEffect != CameraImageEffect.None)
                sb.Append($" -ifx {ImageEffect.ToString().ToLowerInvariant()}");

            if (ImageColorEffectU >= 0 && ImageColorEffectV >= 0)
            {
                sb.Append(
                    $" -cfx {ImageColorEffectU.Clamp(0, 255).ToString(Ci)}:{ImageColorEffectV.Clamp(0, 255).ToString(Ci)}");
            }

            if (CaptureMeteringMode != CameraMeteringMode.Average)
                sb.Append($" -mm {CaptureMeteringMode.ToString().ToLowerInvariant()}");

            if (ImageRotation != CameraImageRotation.None)
                sb.Append($" -rot {((int) ImageRotation).ToString(Ci)}");

            if (ImageFlipHorizontally)
                sb.Append(" -hf");

            if (ImageFlipVertically)
                sb.Append(" -vf");

            if (CaptureSensorRoi.IsDefault == false)
                sb.Append($" -roi {CaptureSensorRoi}");

            if (CaptureShutterSpeedMicroseconds > 0)
                sb.Append($" -ss {CaptureShutterSpeedMicroseconds.Clamp(0, 6000000).ToString(Ci)}");

            if (CaptureDynamicRangeCompensation != CameraDynamicRangeCompensation.Off)
                sb.Append($" -drc {CaptureDynamicRangeCompensation.ToString().ToLowerInvariant()}");

            if (CaptureWhiteBalanceControl == CameraWhiteBalanceMode.Off &&
                (CaptureWhiteBalanceGainBlue != 0M || CaptureWhiteBalanceGainRed != 0M))
                sb.Append($" -awbg {CaptureWhiteBalanceGainBlue.ToString(Ci)},{CaptureWhiteBalanceGainRed.ToString(Ci)}");

            if (ImageAnnotationFontSize > 0)
            {
                sb.Append($" -ae {ImageAnnotationFontSize.Clamp(6, 160).ToString(Ci)}");
                sb.Append($",{(ImageAnnotationFontColor == null ? "0xff" : ImageAnnotationFontColor.ToYuvHex(true))}");

                if (ImageAnnotationBackground != null)
                {
                    ImageAnnotations |= CameraAnnotation.SolidBackground;
                    sb.Append($",{ImageAnnotationBackground.ToYuvHex(true)}");
                }
            }

            if (ImageAnnotations != CameraAnnotation.None)
                sb.Append($" -a {((int) ImageAnnotations).ToString(Ci)}");

            if (string.IsNullOrWhiteSpace(ImageAnnotationsText) == false)
                sb.Append($" -a \"{ImageAnnotationsText.Replace("\"", "'")}\"");

            var result = sb.ToString();
            return result;
        }

        #endregion
    }
}