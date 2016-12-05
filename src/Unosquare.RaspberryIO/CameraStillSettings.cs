namespace Unosquare.RaspberryIO
{
    using System;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public class CameraStillSettings : CameraSettingsBase
    {
        protected override string CommandFile
        {
            get
            {
                return "raspistill";
            }
        }

        public override string CreateProcessArguments()
        {
            var sb = new StringBuilder();
            sb.Append($"-o -"); // output to standard output as opposed to a file.
            sb.Append($" -t { (CaptureTimeoutMilliseconds <= 0 ? "1" : CaptureTimeoutMilliseconds.ToString(CI))}");
            sb.Append($" -e {CaptureEncoding.ToString().ToLowerInvariant()}");

            // Basic Width and height
            if (ImageWidth > 0 && ImageHeight > 0)
            {
                sb.Append($" -w {ImageWidth.ToString(CI)}");
                sb.Append($" -h {ImageHeight.ToString(CI)}");
            }

            // JPEG Encoder specific arguments
            if (CaptureEncoding == CameraImageEncodingFormat.Jpg)
            {
                sb.Append($" -q {CaptureJpegQuality.Clamp(0, 100).ToString(CI)}");

                if (CaptureJpegIncludeRawBayerMetadata)
                    sb.Append($" -r");

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

            // Display Preview
            if (CaptureDisplayPreview)
            {
                if (CaptureDisplayPreviewAtResolution) sb.Append($" -fp");
                if (CaptureDisplayPreviewInFullScreen) sb.Append($" -f");
                if (CaptureDisplayPreviewOpacity != byte.MaxValue) sb.Append($" -op {CaptureDisplayPreviewOpacity.ToString(CI)}");
            }
            else
            {
                sb.Append($" -n"); // no preview
            }

            // Picture Settings
            if (ImageSharpness != 0)
                sb.Append($" -sh {ImageSharpness.Clamp(-100, 100).ToString(CI)}");

            if (ImageContrast != 0)
                sb.Append($" -co {ImageContrast.Clamp(-100, 100).ToString(CI)}");

            if (ImageBrightness != 50)
                sb.Append($" -br {ImageBrightness.Clamp(0, 100).ToString(CI)}");

            if (ImageSaturation != 0)
                sb.Append($" -sa {ImageSaturation.Clamp(-100, 100).ToString(CI)}");

            if (ImageISO >= 100)
                sb.Append($" -ISO {ImageISO.Clamp(100, 800).ToString(CI)}");

            if (CaptureVideoStabilizationEnabled)
                sb.Append($" -vs");

            if (CaptureExposureCompensation != 0)
                sb.Append($" -ev {CaptureExposureCompensation.Clamp(-10, 10).ToString(CI)}");

            if (CaptureExposure != CameraExposureMode.Auto)
                sb.Append($" -ex {CaptureExposure.ToString().ToLowerInvariant()}");

            if (CaptureWhiteBalanceControl != CameraWhiteBalanceMode.Auto)
                sb.Append($" -awb {CaptureWhiteBalanceControl.ToString().ToLowerInvariant()}");

            if (ImageEffect != CameraImageEffect.None)
                sb.Append($" -ifx {ImageEffect.ToString().ToLowerInvariant()}");

            if (ImageColorEffectU >= 0 && ImageColorEffectV >= 0)
                sb.Append($" -cfx {ImageColorEffectU.Clamp(0, 255).ToString(CI)}:{ImageColorEffectV.Clamp(0, 255).ToString(CI)}");

            if (CaptureMeteringMode != CameraMeteringMode.Average)
                sb.Append($" -mm {CaptureMeteringMode.ToString().ToLowerInvariant()}");

            if (ImageRotation != CameraImageRotation.None)
                sb.Append($" -rot {((int)ImageRotation).ToString(CI)}");

            if (ImageFlipHorizontally)
                sb.Append($" -hf");

            if (ImageFlipVertically)
                sb.Append($" -vf");

            if (CaptureSensorRoi.IsDefault == false)
                sb.Append($" -roi {CaptureSensorRoi.ToString()}");

            if (CaptureShutterSpeedMicroseconds > 0)
                sb.Append($" -ss {CaptureShutterSpeedMicroseconds.Clamp(0, 6000000).ToString(CI)}");

            if (CaptureDynamicRangeCompensation != CameraDynamicRangeCompensation.Off)
                sb.Append($" -drc {CaptureDynamicRangeCompensation.ToString().ToLowerInvariant()}");

            if (CaptureWhiteBalanceControl == CameraWhiteBalanceMode.Off && (CaptureWhiteBalanceGainBlue != 0M || CaptureWhiteBalanceGainRed != 0M))
                sb.Append($" -awbg {CaptureWhiteBalanceGainBlue.ToString(CI)},{CaptureWhiteBalanceGainRed.ToString(CI)}");

            if (ImageAnnotationFontSize > 0)
            {
                sb.Append($" -ae {ImageAnnotationFontSize.Clamp(6, 160).ToString(CI)}");
                sb.Append($",{(ImageAnnotationFontColor == null ? "0xff" : ImageAnnotationFontColor.ToYuvHex(true))}");

                if (ImageAnnotationBackground != null)
                {
                    ImageAnnotations |= CameraAnnotation.SolidBackground;
                    sb.Append($",{ImageAnnotationBackground.ToYuvHex(true)}");
                }

            }

            if (ImageAnnotations != CameraAnnotation.None)
                sb.Append($" -a {((int)ImageAnnotations).ToString(CI)}");

            if (string.IsNullOrWhiteSpace(ImageAnnotationsText) == false)
                sb.Append($" -a \"{ImageAnnotationsText.Replace("\"", "'")}\"");

            var result = sb.ToString();
            Console.WriteLine($"{CommandFile} {result}");
            return result;
        }


    }
}
