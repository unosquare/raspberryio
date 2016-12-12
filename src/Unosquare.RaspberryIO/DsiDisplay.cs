namespace Unosquare.RaspberryIO
{
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// 
    /// Some docs available here:
    /// http://forums.pimoroni.com/t/official-7-raspberry-pi-touch-screen-faq/959
    /// </summary>
    public class DsiDisplay
    {
        private const string BacklightFilename = "/sys/class/backlight/rpi_backlight/bl_power";
        private const string BrightnessFilename = "/sys/class/backlight/rpi_backlight/brightness";


        static private DsiDisplay m_Instance = null;

        /// <summary>
        /// Gets DSI Display Instance
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        static public DsiDisplay Instance
        {
            get
            {
                lock (Pi.SyncLock)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = new DsiDisplay();
                    }

                    return m_Instance;
                }
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="DsiDisplay"/> class from being created.
        /// </summary>
        private DsiDisplay()
        {
            // placeholder
        }

        /// <summary>
        /// Gets a value indicating whether the Pi Foundation Dsiplay files are present.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is present; otherwise, <c>false</c>.
        /// </value>
        public bool IsPresent
        {
            get
            {
                return File.Exists(BrightnessFilename);
            }
        }

        /// <summary>
        /// Gets or sets the brightness of the DSI display via filesystem.
        /// </summary>
        /// <value>
        /// The brightness.
        /// </value>
        public byte Brightness
        {
            get
            {
                if (IsPresent == false) return 0;
                byte brightness = 0;
                if (byte.TryParse(File.ReadAllText(BrightnessFilename).Trim(), out brightness))
                    return brightness;

                return 0;
            }
            set
            {
                if (IsPresent == false) return;
                File.WriteAllText(BrightnessFilename, value.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the backlight of the DSI display on.
        /// This operation is perfomed via the file system
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is backlight on; otherwise, <c>false</c>.
        /// </value>
        public bool IsBacklightOn
        {
            get
            {
                if (IsPresent == false) return false;
                int backlight = 0;
                if (int.TryParse(File.ReadAllText(BacklightFilename).Trim(), out backlight))
                    return backlight == 0;

                return false;
            }
            set
            {
                if (IsPresent == false) return;
                File.WriteAllText(BacklightFilename, (value ? "0" : "1"));
            }
        }


    }
}
