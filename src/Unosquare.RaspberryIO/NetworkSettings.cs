namespace Unosquare.RaspberryIO
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;
    using Unosquare.Swan;

    /// <summary>
    /// Represents the network information
    /// </summary>
    public class NetworkSettings
    {
        private static Lazy<NetworkSettings> m_Instance = new Lazy<NetworkSettings>(() => new NetworkSettings(), true);

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        public static NetworkSettings Instance { get { return m_Instance.Value; } }

        private ReadOnlyCollection<NetworkAdapter> m_Adapters;

        private NetworkSettings()
        {

        }

        /// <summary>
        /// Gets the local machine Host Name.
        /// </summary>
        public string HostName => Dns.GetHostName();

        /// <summary>
        /// Retrieves the network adapters.
        /// </summary>
        /// <returns></returns>
        public List<NetworkAdapter> RetrieveAdapters()
        {
            var result = new List<NetworkAdapter>();
            var interfacesOutput = ProcessHelper.GetProcessOutputAsync("ifconfig").Result;
            var wlanOutput = ProcessHelper.GetProcessOutputAsync("iwconfig").Result.Split('\n');
            var outputLines = interfacesOutput.Split('\n').Where(x => string.IsNullOrWhiteSpace(x) == false).ToList().ToArray();
            
            for (var i = 0; i < outputLines.Length; i++)
            {
                var line = outputLines[i];

                if (line[0] >= 'a' && line[0] <= 'z')
                {
                    var adapter = new NetworkAdapter
                    {
                        Name = line.Substring(0, line.IndexOf(' '))
                    };

                    if (line.IndexOf("HWaddr") > 0)
                    {
                        var startIndexHwd = line.IndexOf("HWaddr") + "HWaddr".Length;
                        adapter.MacAddress = line.Substring(startIndexHwd).Trim();
                    }

                    if (i + 1 >= outputLines.Length) break;

                    // move next line
                    line = outputLines[++i].Trim();

                    if (line.StartsWith("inet addr:"))
                    {
                        var tempIP = line.Replace("inet addr:", "").Trim();
                        tempIP = tempIP.Substring(0, tempIP.IndexOf(' '));
                        
                        IPAddress outValue;
                        if (IPAddress.TryParse(tempIP, out outValue))
                            adapter.IPv4 = outValue;

                        if (i + 1 >= outputLines.Length) break;
                        line = outputLines[++i].Trim();
                    }

                    if (line.StartsWith("inet6 addr:"))
                    {
                        var tempIP = line.Replace("inet6 addr:", "").Trim();
                        tempIP = tempIP.Substring(0, tempIP.IndexOf('/'));

                        IPAddress outValue;
                        if (IPAddress.TryParse(tempIP, out outValue))
                            adapter.IPv6 = outValue;
                    }

                    var wlanInfo = wlanOutput.FirstOrDefault(x => x.StartsWith(adapter.Name));

                    if (wlanInfo != null)
                    {
                        var startIndex = wlanInfo.IndexOf("ESSID:") + "ESSID:".Length;
                        adapter.AccessPointName = wlanInfo.Substring(startIndex).Replace("\"", "");
                    }

                    result.Add(adapter);
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Represents a Network Adapter
    /// </summary>
    public class NetworkAdapter
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the i PV4.
        /// </summary>
        public IPAddress IPv4 { get; set; }

        /// <summary>
        /// Gets or sets the i PV6.
        /// </summary>
        public IPAddress IPv6 { get; set; }

        /// <summary>
        /// Gets or sets the name of the access point.
        /// </summary>
        public string AccessPointName { get; set; }

        /// <summary>
        /// Gets or sets the mac address.
        /// </summary>
        /// <value>
        /// The mac address.
        /// </value>
        public string MacAddress { get; set; }
    }
}
