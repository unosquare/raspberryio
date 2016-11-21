using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unosquare.RaspberryIO
{
    public static class Utilities
    {
        private static bool? m_IsLinuxOS = new Nullable<bool>();
        private static bool? m_IsRunningAsRoot = new Nullable<bool>();

        public static bool IsLinuxOS
        {
            get
            {
                if (m_IsLinuxOS.HasValue == false)
                {
                    m_IsLinuxOS = Environment.OSVersion.Platform == PlatformID.Unix;
                }

                return m_IsLinuxOS.Value;
            }
        }

        public static bool IsRunningAsRoot
        {
            get
            {
                if (m_IsRunningAsRoot.HasValue == false)
                {
                    m_IsRunningAsRoot = Interop.getuid() == 0;
                }

                return m_IsRunningAsRoot.Value;
            }
            
        }
    }
}
