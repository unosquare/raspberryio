namespace Unosquare.RaspberryIO.Computer
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Unosquare.Swan.Abstractions;
    using Unosquare.Swan.Components;

    /// <summary>
    /// Represents the bluetooth information.
    /// </summary>
    public class Bluetooth : SingletonBase<Bluetooth>
    {
        /// <summary>
        /// Turns on the bluetooth adapter.
        /// </summary>
        /// <returns>Returns true or false depending if the controller was turned on.</returns>
        public async Task<bool> PowerOn()
        {
            var output = await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "power on").ConfigureAwait(false);
            return output.Contains("succeeded") ? true : false;
        }

        /// <summary>
        /// Turns off the bluetooth adapter.
        /// </summary>
        /// <returns>Returns true or false depending if the controller was turned off.</returns>
        public async Task<bool> PowerOff()
        {
            var output = await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "power off").ConfigureAwait(false);
            return output.Contains("succeeded") ? true : false;
        }

        /// <summary>
        /// Gets the list of bluetooth controllers.
        /// </summary>
        /// <returns>Returns the list of bluetooth controllers.</returns>
        public async Task<List<string>> ListDevices()
        {
            var devices = await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "list").ConfigureAwait(false);
            return devices.Trim().Split('\n').Select(x => x.Trim()).ToList();
        }
    }
}
