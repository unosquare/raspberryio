using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Unosquare.RaspberryIO
{
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

        private DsiDisplay()
        {
            // placeholder
        }

        public bool IsPresent
        {
            get
            {
                return File.Exists(BrightnessFilename);
            }
        }

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

        public bool IsBacklightOn
        {
            get
            {
                if (IsPresent == false) return false;
                int backlight = 0;
                if (int.TryParse(File.ReadAllText(BacklightFilename).Trim(), out backlight))
                    return backlight != 0;

                return false;
            }
            set
            {
                if (IsPresent == false) return;
                File.WriteAllText(BacklightFilename, (value ? "1" : "0"));
            }
        }


    }
}
