namespace Unosquare.RaspberryIO.Camera
{
    using Swan;
    using System.Text;

    /// <summary>
    /// Represents the raspivid camera settings for video capture functionality
    /// </summary>
    /// <seealso cref="CameraSettingsBase" />
    public class CameraVideoSettings : CameraSettingsBase
    {
        /// <summary>
        /// Gets the command file executable.
        /// </summary>
        public override string CommandName => "raspivid";

        /// <summary>
        /// Use bits per second, so 10Mbits/s would be -b 10000000. For H264, 1080p30 a high quality bitrate would be 15Mbits/s or more. 
        /// Maximum bitrate is 25Mbits/s (-b 25000000), but much over 17Mbits/s won't show noticeable improvement at 1080p30.
        /// Default -1
        /// </summary>
        public int CaptureBitrate { get; set; } = -1;

        /// <summary>
        /// Gets or sets the framerate.
        /// Default 25, range 2 to 30
        /// </summary>
        public int CaptureFramerate { get; set; } = 25;

        /// <summary>
        /// Sets the intra refresh period (GoP) rate for the recorded video. H264 video uses a complete frame (I-frame) every intra 
        /// refresh period, from which subsequent frames are based. This option specifies the number of frames between each I-frame. 
        /// Larger numbers here will reduce the size of the resulting video, and smaller numbers make the stream less error-prone.
        /// </summary>
        public int CaptureKeyframeRate { get; set; } = 25;

        /// <summary>
        /// Sets the initial quantisation parameter for the stream. Varies from approximately 10 to 40, and will greatly affect 
        /// the quality of the recording. Higher values reduce quality and decrease file size. Combine this setting with a 
        /// bitrate of 0 to set a completely variable bitrate.
        /// </summary>
        public int CaptureQuantisation { get; set; } = 23;

        /// <summary>
        /// Gets or sets the profile.
        /// Sets the H264 profile to be used for the encoding.
        /// Default is Main mode
        /// </summary>
        public CameraH264Profile CaptureProfile { get; set; } = CameraH264Profile.Main;

        /// <summary>
        /// Forces the stream to include PPS and SPS headers on every I-frame. Needed for certain streaming cases 
        /// e.g. Apple HLS. These headers are small, so don't greatly increase the file size.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [interleave headers]; otherwise, <c>false</c>.
        /// </value>
        public bool CaptureInterleaveHeaders { get; set; } = true;

        /// <summary>
        /// Switch on an option to display the preview after compression. This will show any compression artefacts in the preview window. In normal operation, 
        /// the preview will show the camera output prior to being compressed. This option is not guaranteed to work in future releases.
        /// </summary>
        /// <value>
        /// <c>true</c> if [capture display preview encoded]; otherwise, <c>false</c>.
        /// </value>
        public bool CaptureDisplayPreviewEncoded { get; set; } = false;

        /// <summary>
        /// Creates the process arguments.
        /// </summary>
        /// <returns>A string representing the arguments</returns>
        public override string CreateProcessArguments()
        {
            var sb = new StringBuilder(base.CreateProcessArguments());

            sb.Append($" -pf {CaptureProfile.ToString().ToLowerInvariant()}");
            if (CaptureBitrate < 0)
                sb.Append($" -b {CaptureBitrate.Clamp(0, 25000000).ToString(Ci)}");

            if (CaptureFramerate >= 2)
                sb.Append($" -fps {CaptureFramerate.Clamp(2, 30).ToString(Ci)}");

            if (CaptureDisplayPreview && CaptureDisplayPreviewEncoded)
                    sb.Append($" -e");

            if (CaptureKeyframeRate > 0)
                sb.Append($" -g {CaptureKeyframeRate.ToString(Ci)}");

            if (CaptureQuantisation >= 0)
                sb.Append($" -qp {CaptureQuantisation.Clamp(0, 40).ToString(Ci)}");

            if (CaptureInterleaveHeaders)
                sb.Append(" -ih");

            var commandArgs = sb.ToString();
            $"{CommandName} {commandArgs}".Trace(Pi.LoggerSource);
            return commandArgs;
        }
    }
}