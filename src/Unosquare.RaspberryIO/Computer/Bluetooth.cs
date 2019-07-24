using Unosquare.Swan;

namespace Unosquare.RaspberryIO.Computer
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
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
            using (var cancellationTokenSource = new CancellationTokenSource(3000))
            {
                ScanOn(cancellationTokenSource.Token);
                await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "scan off").ConfigureAwait(false);
                var devices = await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "devices").ConfigureAwait(false);
                return devices.Trim().Split('\n').Select(x => x.Trim()).ToList();
            }
        }

        public async Task<List<string>> ListControllers()
        {
            var controllers = await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "list").ConfigureAwait(false);
            return controllers.Trim().Split('\n').Select(x => x.Trim()).ToList();
        }

        public async Task<bool> Pair(string controllerAddress, string deviceAddress)
        {
            await ProcessRunner.GetProcessOutputAsync("bluetoothctl", $"select {controllerAddress}").ConfigureAwait(false); // Selects the controller to pair. Once you select the controller, all controller-related commands will apply to it for three minutes.
            await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "discoverable on").ConfigureAwait(false); // Makes the controller visible to other devices.
            await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "pairable on").ConfigureAwait(false); // Readies the controller for pairing. Remember that you have three minutes after running this command to pair.

            await ProcessRunner.GetProcessOutputAsync("bluetoothctl", $"pair {deviceAddress}").ConfigureAwait(false); // Pairs the device with the controller.
        
            await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "discoverable off").ConfigureAwait(false); // Hides the controller from other Bluetooth devices. Otherwise, any device that can detect it has access to it, leaving a major security hole.

            return true;
        }

        public async Task<bool> Connect(string controllerAddress, string deviceAddress)
        {
            await ProcessRunner.GetProcessOutputAsync("bluetoothctl", $"select {controllerAddress}").ConfigureAwait(false); // Selects the controller to pair. Once you select the controller, all controller-related commands will apply to it for three minutes.
            await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "discoverable on").ConfigureAwait(false); // Makes the controller visible to other devices.
            await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "pairable on").ConfigureAwait(false); // Readies the controller for pairing. Remember that you have three minutes after running this command to pair.

            await ProcessRunner.GetProcessOutputAsync("bluetoothctl", $"connect {deviceAddress}").ConfigureAwait(false); // Readies the device for pairing.

            await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "discoverable off").ConfigureAwait(false); // Hides the controller from other Bluetooth devices. Otherwise, any device that can detect it has access to it, leaving a major security hole.

            return true;
        }

        public async Task<bool> Trust(string controllerAddress, string deviceAddress)
        {
            await ProcessRunner.GetProcessOutputAsync("bluetoothctl", $"select {controllerAddress}").ConfigureAwait(false); // Selects the controller to pair. Once you select the controller, all controller-related commands will apply to it for three minutes.
            await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "discoverable on").ConfigureAwait(false); // Makes the controller visible to other devices.
            await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "pairable on").ConfigureAwait(false); // Readies the controller for pairing. Remember that you have three minutes after running this command to pair.

            await ProcessRunner.GetProcessOutputAsync("bluetoothctl", $"trust {deviceAddress}").ConfigureAwait(false); // Sets the device to re-pair automatically when it is turned on, which eliminates the need to pair all over again.

            await ProcessRunner.GetProcessOutputAsync("bluetoothctl", "discoverable off").ConfigureAwait(false); // Hides the controller from other Bluetooth devices. Otherwise, any device that can detect it has access to it, leaving a major security hole.

            return true;
        }

        public async Task<bool> Info(string deviceAddress)
        {
            await ProcessRunner.GetProcessOutputAsync("bluetoothctl", $"info {deviceAddress}").ConfigureAwait(false); // 
            return true;
        }

        private static void ScanOn(CancellationToken token) => Task.Run(() =>
            ProcessRunner.GetProcessOutputAsync("bluetoothctl", "scan on", token));
    }
}
