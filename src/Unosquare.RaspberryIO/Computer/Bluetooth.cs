namespace Unosquare.RaspberryIO.Computer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Unosquare.RaspberryIO.Abstractions;
    using Unosquare.Swan.Abstractions;
    using Unosquare.Swan.Components;

    /// <summary>
    /// Represents the bluetooth information.
    /// </summary>
    public class Bluetooth : SingletonBase<Bluetooth>
    {

        private const string BC = "bluetoothctl";

        /// <summary>
        /// Turns on the bluetooth adapter.
        /// </summary>
        /// <returns>Returns true or false depending if the controller was turned on.</returns>
        public async Task<bool> PowerOn()
        {
            try
            {
                var output = await ProcessRunner.GetProcessOutputAsync(BC, "power on").ConfigureAwait(false);
                return output.Contains("succeeded");
            }
            catch (Exception ex)
            {
                throw new BluetoothErrorException($"Failed to power on: {ex.Message}");
            }
        }

        /// <summary>
        /// Turns off the bluetooth adapter.
        /// </summary>
        /// <returns>Returns true or false depending if the controller was turned off.</returns>
        public async Task<bool> PowerOff()
        {
            try
            {
                var output = await ProcessRunner.GetProcessOutputAsync(BC, "power off").ConfigureAwait(false);
                return output.Contains("succeeded");
            }
            catch (Exception ex)
            {
                throw new BluetoothErrorException($"Failed to power off: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the list of detected devices.
        /// </summary>
        /// <returns> Returns the list of detected devices. </returns>
        public async Task<IEnumerable<string>> ListDevices()
        {
            try
            {
                using (var cancellationTokenSource = new CancellationTokenSource(3000))
                {
                    ProcessRunner.GetProcessOutputAsync(BC, "scan on", cancellationTokenSource.Token);
                    await ProcessRunner.GetProcessOutputAsync(BC, "scan off").ConfigureAwait(false);
                    var devices = await ProcessRunner.GetProcessOutputAsync(BC, "devices").ConfigureAwait(false);
                    return devices.Trim().Split('\n').Select(x => x.Trim());
                }
            }
            catch (Exception ex)
            {
                throw new BluetoothErrorException($"Failed to retrieve devices: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the list of bluetooth controllers.
        /// </summary>
        /// <returns> Returns the list of bluetooth controllers. </returns>
        public async Task<IEnumerable<string>> ListControllers()
        {
            try
            {
                var controllers = await ProcessRunner.GetProcessOutputAsync(BC, "list").ConfigureAwait(false);
                return controllers.Trim().Split('\n').Select(x => x.Trim());
            }
            catch (Exception ex)
            {
                throw new BluetoothErrorException($"Failed to retrieve controllers: {ex.Message}");
            }
        }

        /// <summary>
        /// Pairs a specific device with a specific controller.
        /// </summary>
        /// <param name="controllerAddress">The mac address of the controller that will be used to pair.</param>
        /// <param name="deviceAddress">The mac address of the device that will be paired.</param>
        /// <returns> Returns true or false if the pair was succesfully.</returns>
        public async Task<bool> Pair(string controllerAddress, string deviceAddress)
        {
            try
            {
                await ProcessRunner.GetProcessOutputAsync(BC, $"select {controllerAddress}").ConfigureAwait(false); // Selects the controller to pair. Once you select the controller, all controller-related commands will apply to it for three minutes.
                await ProcessRunner.GetProcessOutputAsync(BC, "discoverable on").ConfigureAwait(false); // Makes the controller visible to other devices.
                await ProcessRunner.GetProcessOutputAsync(BC, "pairable on").ConfigureAwait(false); // Readies the controller for pairing. Remember that you have three minutes after running this command to pair.

                var result = await ProcessRunner.GetProcessOutputAsync(BC, $"pair {deviceAddress}").ConfigureAwait(false); // Pairs the device with the controller.
                await ProcessRunner.GetProcessOutputAsync(BC, "discoverable off").ConfigureAwait(false); // Hides the controller from other Bluetooth devices. Otherwise, any device that can detect it has access to it, leaving a major security hole.

                return result.Contains("Paired: yes");
            }
            catch (Exception ex)
            {
                throw new BluetoothErrorException($"Failed to Pair: {ex.Message}");
            }
        }

        /// <summary>
        /// Performs a connection of a given controller with a given device.
        /// </summary>
        /// <param name="controllerAddress">The mac address of the controller that will be used to make the connection. </param>
        /// <param name="deviceAddress">The mac address of the device that will be connected. </param>
        /// <returns> Returns true or false if the connection was successfully. </returns>
        public async Task<bool> Connect(string controllerAddress, string deviceAddress)
        {
            try
            {
                await ProcessRunner.GetProcessOutputAsync(BC, $"select {controllerAddress}").ConfigureAwait(false); // Selects the controller to pair. Once you select the controller, all controller-related commands will apply to it for three minutes.
                await ProcessRunner.GetProcessOutputAsync(BC, "discoverable on").ConfigureAwait(false); // Makes the controller visible to other devices.
                await ProcessRunner.GetProcessOutputAsync(BC, "pairable on").ConfigureAwait(false); // Readies the controller for pairing. Remember that you have three minutes after running this command to pair.

                var result = await ProcessRunner.GetProcessOutputAsync(BC, $"connect {deviceAddress}").ConfigureAwait(false); // Readies the device for pairing.

                await ProcessRunner.GetProcessOutputAsync(BC, "discoverable off").ConfigureAwait(false); // Hides the controller from other Bluetooth devices. Otherwise, any device that can detect it has access to it, leaving a major security hole.

                return result.Contains("Connected: yes");
            }
            catch (Exception ex)
            {
                throw new BluetoothErrorException($"Failed to connect: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets the device to re-pair automatically when it is turned on, which eliminates the need to pair all over again.
        /// </summary>
        /// <param name="controllerAddress">The mac address of the controller will be used.</param>
        /// <param name="deviceAddress">The mac address of the device will be added to the trust list devices.</param>
        /// <returns>Returns true or false if the operation was successful.</returns>
        public async Task<bool> Trust(string controllerAddress, string deviceAddress)
        {
            try
            {

                await ProcessRunner.GetProcessOutputAsync(BC, $"select {controllerAddress}").ConfigureAwait(false); // Selects the controller to pair. Once you select the controller, all controller-related commands will apply to it for three minutes.
                await ProcessRunner.GetProcessOutputAsync(BC, "discoverable on").ConfigureAwait(false); // Makes the controller visible to other devices.
                await ProcessRunner.GetProcessOutputAsync(BC, "pairable on").ConfigureAwait(false); // Readies the controller for pairing. Remember that you have three minutes after running this command to pair.

                var result = await ProcessRunner.GetProcessOutputAsync(BC, $"trust {deviceAddress}").ConfigureAwait(false); // Sets the device to re-pair automatically when it is turned on, which eliminates the need to pair all over again.

                await ProcessRunner.GetProcessOutputAsync(BC, "discoverable off").ConfigureAwait(false); // Hides the controller from other Bluetooth devices. Otherwise, any device that can detect it has access to it, leaving a major security hole.

                return result.Contains("Trusted: yes");
            }
            catch (Exception ex)
            {
                throw new BluetoothErrorException($"Failed to add to trust devices list: {ex.Message}");
            }
        }

        /// <summary>
        /// Displays information about a particular device.
        /// </summary>
        /// <param name="deviceAddress">The mac address of the device which info will be retrieved.</param>
        /// <returns> Returns the device info.</returns>
        public async Task<string> DeviceInfo(string deviceAddress)
        {
            try
            {
                var info = await ProcessRunner.GetProcessOutputAsync(BC, $"info {deviceAddress}").ConfigureAwait(false);
                return !string.IsNullOrEmpty(info) ? info : $"Device {deviceAddress} not available";
            }
            catch (Exception ex)
            {
                throw new BluetoothErrorException($"Failed to retrieve  info for {deviceAddress}: {ex.Message}");
            }
        }
    }
}
