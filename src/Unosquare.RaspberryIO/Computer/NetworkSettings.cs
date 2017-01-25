namespace Unosquare.RaspberryIO.Computer
{
    using Swan.Components;
    using Swan.Abstractions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.IO;

    /// <summary>
    /// Represents the network information
    /// </summary>
    public class NetworkSettings : SingletonBase<NetworkSettings>
    {
        private NetworkSettings()
        {

        }

        /// <summary>
        /// Gets the local machine Host Name.
        /// </summary>
        public string HostName => Dns.GetHostName();

        /// <summary>
        /// Retrieves the wireless networks.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <returns></returns>
        public List<WirelessNetworkInfo> RetrieveWirelessNetworks(string adapter)
        {
            return RetrieveWirelessNetworks(new[] { adapter });
        }

        /// <summary>
        /// Retrieves the wireless networks.
        /// </summary>
        /// <returns></returns>
        public List<WirelessNetworkInfo> RetrieveWirelessNetworks(string[] adapters = null)
        {
            var result = new List<WirelessNetworkInfo>();

            foreach (var networkAdapter in adapters ?? RetrieveAdapters().Where(x => x.IsWireless).Select(x => x.Name))
            {
                var wirelessOutput = ProcessRunner.GetProcessOutputAsync("iwlist", $"{networkAdapter} scanning").Result;
                var outputLines = wirelessOutput.Split('\n').Select(x => x.Trim()).Where(x => string.IsNullOrWhiteSpace(x) == false).ToArray();

                for (var i = 0; i < outputLines.Length; i++)
                {
                    var line = outputLines[i];

                    if (line.StartsWith("ESSID:") == false) continue;

                    var network = new WirelessNetworkInfo()
                    {
                        Name = line.Replace("ESSID:", "").Replace("\"", string.Empty)
                    };

                    while (true)
                    {
                        if (i + 1 >= outputLines.Length) break;

                        // move next line
                        line = outputLines[++i];

                        if (line.StartsWith("Quality="))
                        {
                            network.Quality = line.Replace("Quality=", "");
                            break;
                        }
                    }

                    while (true)
                    {
                        if (i + 1 >= outputLines.Length) break;

                        // move next line
                        line = outputLines[++i];

                        if (line.StartsWith("Encryption key:"))
                        {
                            network.IsEncrypted = line.Replace("Encryption key:", string.Empty).Trim() == "on";
                            break;
                        }
                    }

                    result.Add(network);
                }
            }

            return result;
        }

        /// <summary>
        /// Setups the wireless network.
        /// </summary>
        /// <param name="networkSsid">The network ssid.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public bool SetupWirelessNetwork(string networkSsid, string password = null)
        {
            var payload = "ctrl_interface=DIR=/var/run/wpa_supplicant GROUP=netdev\r\nupdate_config = 1\r\n";

            payload += string.IsNullOrEmpty(password) ?  
                $"network={{\n\tssid=\"{networkSsid}\"\n\t}}" :
                $"network={{\n\tssid=\"{networkSsid}\"\n\tpsk=\"{password}\"\n\t}}";

            try
            {
                File.WriteAllText("/etc/wpa_supplicant/wpa_supplicant.conf", payload);
            }
            catch
            {
                return false;
            }

            return true;
        }

    /// <summary>
    /// Retrieves the network adapters.
    /// </summary>
    /// <returns></returns>
    public List<NetworkAdapter> RetrieveAdapters()
        {
            var result = new List<NetworkAdapter>();
            var interfacesOutput = ProcessRunner.GetProcessOutputAsync("ifconfig").Result;
            var wlanOutput = ProcessRunner.GetProcessOutputAsync("iwconfig").Result.Split('\n').Where(x => x.Contains("no wireless extensions.") == false).ToArray();
            var outputLines = interfacesOutput.Split('\n').Where(x => string.IsNullOrWhiteSpace(x) == false).ToArray();

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
                        var tempIP = line.Replace("inet addr:", string.Empty).Trim();
                        tempIP = tempIP.Substring(0, tempIP.IndexOf(' '));

                        IPAddress outValue;
                        if (IPAddress.TryParse(tempIP, out outValue))
                            adapter.IPv4 = outValue;

                        if (i + 1 >= outputLines.Length) break;
                        line = outputLines[++i].Trim();
                    }

                    if (line.StartsWith("inet6 addr:"))
                    {
                        var tempIP = line.Replace("inet6 addr:", string.Empty).Trim();
                        tempIP = tempIP.Substring(0, tempIP.IndexOf('/'));

                        IPAddress outValue;
                        if (IPAddress.TryParse(tempIP, out outValue))
                            adapter.IPv6 = outValue;
                    }

                    var wlanInfo = wlanOutput.FirstOrDefault(x => x.StartsWith(adapter.Name));

                    if (wlanInfo != null)
                    {
                        adapter.IsWireless = true;

                        var startIndex = wlanInfo.IndexOf("ESSID:") + "ESSID:".Length;
                        adapter.AccessPointName = wlanInfo.Substring(startIndex).Replace("\"", string.Empty);
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
        public string MacAddress { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is wireless.
        /// </summary>
        public bool IsWireless { get; set; }
    }

    /// <summary>
    /// Represents a wireless network information
    /// </summary>
    public class WirelessNetworkInfo
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the network quality.
        /// </summary> 
        public string Quality { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is encrypted.
        /// </summary>
        public bool IsEncrypted { get; set; }
    }
}