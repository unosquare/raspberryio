namespace Unosquare.RaspberryIO.Computer
{
    using Models;
    using Swan;
    using Swan.Abstractions;
    using Swan.Components;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;

    /// <summary>
    /// Represents the network information
    /// </summary>
    public class NetworkSettings : SingletonBase<NetworkSettings>
    {
        private const string EssidTag = "ESSID:";
        private const string HWaddr = "HWaddr";

        /// <summary>
        /// Gets the local machine Host Name.
        /// </summary>
        public string HostName => Network.HostName;

        /// <summary>
        /// Retrieves the wireless networks.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <returns></returns>
        public List<WirelessNetworkInfo> RetrieveWirelessNetworks(string adapter)
        {
            return RetrieveWirelessNetworks(new[] {adapter});
        }

        /// <summary>
        /// Retrieves the wireless networks.
        /// </summary>
        /// <param name="adapters">The adapters.</param>
        /// <returns></returns>
        public List<WirelessNetworkInfo> RetrieveWirelessNetworks(string[] adapters = null)
        {
            var result = new List<WirelessNetworkInfo>();

            foreach (var networkAdapter in adapters ?? RetrieveAdapters().Where(x => x.IsWireless).Select(x => x.Name))
            {
                var wirelessOutput = ProcessRunner.GetProcessOutputAsync("iwlist", $"{networkAdapter} scanning").Result;
                var outputLines =
                    wirelessOutput.Split('\n')
                        .Select(x => x.Trim())
                        .Where(x => string.IsNullOrWhiteSpace(x) == false)
                        .ToArray();

                for (var i = 0; i < outputLines.Length; i++)
                {
                    var line = outputLines[i];
                                                          
                    if (line.StartsWith(EssidTag) == false) continue;

                    var network = new WirelessNetworkInfo()
                    {
                        Name = line.Replace(EssidTag, string.Empty).Replace("\"", string.Empty)
                    };

                    while (true)
                    {
                        if (i + 1 >= outputLines.Length) break;

                        // should look for two lines before the ESSID acording to the scan
                        line = outputLines[i-2];

                        if (line.StartsWith("Quality="))
                        {
                            network.Quality = line.Replace("Quality=", string.Empty);
                            break;
                        }
                    }

                    while (true)
                    {
                        if (i + 1 >= outputLines.Length) break;

                        // should look for a line before the ESSID  acording to the scan
                        line = outputLines[i-1];

                        if (line.StartsWith("Encryption key:"))
                        {
                            network.IsEncrypted = line.Replace("Encryption key:", string.Empty).Trim() == "on";
                            break;
                        }
                    }
                    
                    if (result.Any(x => x.Name == network.Name) == false)
                        result.Add(network);
                }
            }

            return result.OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Setups the wireless network.
        /// </summary>
        /// <param name="adapterName">Name of the adapter.</param>
        /// <param name="networkSsid">The network ssid.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public bool SetupWirelessNetwork(string adapterName, string networkSsid, string password = null)
        {
            // TODO: Get the country where the device is located to set 'country' param in payload var
            var payload = "country=MX\nctrl_interface=DIR=/var/run/wpa_supplicant GROUP=netdev\nupdate_config=1\n";
            payload += string.IsNullOrEmpty(password)
                ? $"network={{\n\tssid=\"{networkSsid}\"\n\t}}\n"
                : $"network={{\n\tssid=\"{networkSsid}\"\n\tpsk=\"{password}\"\n\t}}\n";
            try
            {
                File.WriteAllText("/etc/wpa_supplicant/wpa_supplicant.conf", payload);
                ProcessRunner.GetProcessOutputAsync("pkill", "-f wpa_supplicant").Wait();
                ProcessRunner.GetProcessOutputAsync("ifdown", adapterName).Wait();
                ProcessRunner.GetProcessOutputAsync("ifup", adapterName).Wait();
            }
            catch (Exception ex)
            {
                ex.Log(nameof(NetworkSettings));
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
            var wlanOutput =
                ProcessRunner.GetProcessOutputAsync("iwconfig")
                    .Result.Split('\n')
                    .Where(x => x.Contains("no wireless extensions.") == false)
                    .ToArray();
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

                    if (line.IndexOf(HWaddr) > 0)
                    {
                        var startIndexHwd = line.IndexOf(HWaddr) + HWaddr.Length;
                        adapter.MacAddress = line.Substring(startIndexHwd).Trim();
                    }

                    if (i + 1 >= outputLines.Length) break;

                    // move next line
                    line = outputLines[++i].Trim();

                    if (line.StartsWith("inet addr:"))
                    {
                        var tempIP = line.Replace("inet addr:", string.Empty).Trim();
                        tempIP = tempIP.Substring(0, tempIP.IndexOf(' '));

                        if (IPAddress.TryParse(tempIP, out var outValue))
                            adapter.IPv4 = outValue;

                        if (i + 1 >= outputLines.Length) break;
                        line = outputLines[++i].Trim();
                    }

                    if (line.StartsWith("inet6 addr:"))
                    {
                        var tempIP = line.Replace("inet6 addr:", string.Empty).Trim();
                        tempIP = tempIP.Substring(0, tempIP.IndexOf('/'));

                        if (IPAddress.TryParse(tempIP, out var outValue))
                            adapter.IPv6 = outValue;
                    }

                    var wlanInfo = wlanOutput.FirstOrDefault(x => x.StartsWith(adapter.Name));

                    if (wlanInfo != null)
                    {
                        adapter.IsWireless = true;

                        var startIndex = wlanInfo.IndexOf(EssidTag) + EssidTag.Length;
                        adapter.AccessPointName = wlanInfo.Substring(startIndex).Replace("\"", string.Empty);
                    }

                    result.Add(adapter);
                }
            }

            return result.OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Retrieves current wireless connected network
        /// </summary>
        /// <returns></returns>
        public string GetWirelessNetworkName() => ProcessRunner.GetProcessOutputAsync("iwgetid", "-r").Result;
    }
}
