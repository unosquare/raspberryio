using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Unosquare.RaspberryIO
{
    public class NetworkSettings
    {

        private static Lazy<NetworkSettings> m_Instance = new Lazy<NetworkSettings>(() => new NetworkSettings(), true);

        public static NetworkSettings Instance { get { return m_Instance.Value; } }

        private ReadOnlyCollection<NetworkAdapter> m_Adapters;

        private NetworkSettings()
        {
            
        }


        private static List<NetworkAdapter> RetrieveAdapters()
        {
            throw new NotImplementedException();
            var result = new List<NetworkAdapter>();
            var interfacesOutput = ProcessHelper.GetProcessOutputAsync("ifconfig").Result;
        }

    }

    public class NetworkAdapter
    {

    }

}
