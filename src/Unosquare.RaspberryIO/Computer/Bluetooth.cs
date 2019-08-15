namespace Unosquare.RaspberryIO.Computer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Swan;

    /// <summary>
    /// Represents the Bluetooth information.
    /// </summary>
    public class Bluetooth : SingletonBase<Bluetooth>
    {
        private const string BcCommand = "bluetoothctl";

        /// <summary>
        /// Turns on the Bluetooth adapter.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Returns true or false depending if the controller was turned on.
        /// </returns>
        /// <exception cref="BluetoothErrorException">Failed to power on:.</exception>
        public async Task<bool> PowerOn(CancellationToken cancellationToken = default)
        {
            try
            {
                var output = await ProcessRunner.GetProcessOutputAsync(BcCommand, "power on", cancellationToken).ConfigureAwait(false);
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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Returns true or false depending if the controller was turned off.
        /// </returns>
        /// <exception cref="BluetoothErrorException">Failed to power off:.</exception>
        public async Task<bool> PowerOff(CancellationToken cancellationToken = default)
        {
            try
            {
                var output = await ProcessRunner.GetProcessOutputAsync(BcCommand, "power off", cancellationToken).ConfigureAwait(false);
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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Returns the list of detected devices.
        /// </returns>
        /// <exception cref="BluetoothErrorException">Failed to retrieve devices:.</exception>
        public async Task<IEnumerable<string>> ListDevices(CancellationToken cancellationToken = default)
        {
            try
            {
                using (var cancellationTokenSource = new CancellationTokenSource(3000))
                {
                    await ProcessRunner.GetProcessOutputAsync(BcCommand, "scan on", cancellationTokenSource.Token).ConfigureAwait(false);
                    await ProcessRunner.GetProcessOutputAsync(BcCommand, "scan off", cancellationToken).ConfigureAwait(false);
                    var devices = await ProcessRunner.GetProcessOutputAsync(BcCommand, "devices", cancellationToken).ConfigureAwait(false);
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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Returns the list of bluetooth controllers.
        /// </returns>
        /// <exception cref="BluetoothErrorException">Failed to retrieve controllers:.</exception>
        public async Task<IEnumerable<string>> ListControllers(CancellationToken cancellationToken = default)
        {
            try
            {
                var controllers = await ProcessRunner.GetProcessOutputAsync(BcCommand, "list", cancellationToken).ConfigureAwait(false);
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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Returns true or false if the pair was successfully.
        /// </returns>
        /// <exception cref="BluetoothErrorException">Failed to Pair:.</exception>
        public async Task<bool> Pair(string controllerAddress, string deviceAddress, CancellationToken cancellationToken = default)
        {
            try
            {
                // Selects the controller to pair. Once you select the controller, all controller-related commands will apply to it for three minutes.
                await ProcessRunner.GetProcessOutputAsync(BcCommand, $"select {controllerAddress}", cancellationToken)
                    .ConfigureAwait(false);

                // Makes the controller visible to other devices.
                await ProcessRunner.GetProcessOutputAsync(BcCommand, "discoverable on", cancellationToken).ConfigureAwait(false); 

                // Readies the controller for pairing. Remember that you have three minutes after running this command to pair.
                await ProcessRunner.GetProcessOutputAsync(BcCommand, "pairable on", cancellationToken).ConfigureAwait(false); 

                // Pairs the device with the controller.
                var result = await ProcessRunner.GetProcessOutputAsync(BcCommand, $"pair {deviceAddress}", cancellationToken).ConfigureAwait(false); 

                // Hides the controller from other Bluetooth devices. Otherwise, any device that can detect it has access to it, leaving a major security hole.
                await ProcessRunner.GetProcessOutputAsync(BcCommand, "discoverable off", cancellationToken).ConfigureAwait(false); 

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
        /// <param name="controllerAddress">The mac address of the controller that will be used to make the connection.</param>
        /// <param name="deviceAddress">The mac address of the device that will be connected.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Returns true or false if the connection was successfully.
        /// </returns>
        /// <exception cref="BluetoothErrorException">Failed to connect:.</exception>
        public async Task<bool> Connect(string controllerAddress, string deviceAddress, CancellationToken cancellationToken = default)
        {
            try
            {
                // Selects the controller to pair. Once you select the controller, all controller-related commands will apply to it for three minutes.
                await ProcessRunner.GetProcessOutputAsync(BcCommand, $"select {controllerAddress}", cancellationToken).ConfigureAwait(false); 

                // Makes the controller visible to other devices.
                await ProcessRunner.GetProcessOutputAsync(BcCommand, "discoverable on", cancellationToken).ConfigureAwait(false); 

                // Readies the controller for pairing. Remember that you have three minutes after running this command to pair.
                await ProcessRunner.GetProcessOutputAsync(BcCommand, "pairable on", cancellationToken).ConfigureAwait(false); 

                // Readies the device for pairing.
                var result = await ProcessRunner.GetProcessOutputAsync(BcCommand, $"connect {deviceAddress}", cancellationToken).ConfigureAwait(false); 

                // Hides the controller from other Bluetooth devices. Otherwise, any device that can detect it has access to it, leaving a major security hole.
                await ProcessRunner.GetProcessOutputAsync(BcCommand, "discoverable off", cancellationToken).ConfigureAwait(false); 

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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Returns true or false if the operation was successful.
        /// </returns>
        /// <exception cref="BluetoothErrorException">Failed to add to trust devices list:.</exception>
        public async Task<bool> Trust(string controllerAddress, string deviceAddress, CancellationToken cancellationToken = default)
        {
            try
            {
                // Selects the controller to pair. Once you select the controller, all controller-related commands will apply to it for three minutes.
                await ProcessRunner.GetProcessOutputAsync(BcCommand, $"select {controllerAddress}", cancellationToken).ConfigureAwait(false); 

                // Makes the controller visible to other devices.
                await ProcessRunner.GetProcessOutputAsync(BcCommand, "discoverable on", cancellationToken).ConfigureAwait(false); 

                // Readies the controller for pairing. Remember that you have three minutes after running this command to pair.
                await ProcessRunner.GetProcessOutputAsync(BcCommand, "pairable on", cancellationToken).ConfigureAwait(false); 

                // Sets the device to re-pair automatically when it is turned on, which eliminates the need to pair all over again.
                var result = await ProcessRunner.GetProcessOutputAsync(BcCommand, $"trust {deviceAddress}", cancellationToken).ConfigureAwait(false); 

                // Hides the controller from other Bluetooth devices. Otherwise, any device that can detect it has access to it, leaving a major security hole.
                await ProcessRunner.GetProcessOutputAsync(BcCommand, "discoverable off", cancellationToken).ConfigureAwait(false); 

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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Returns the device info.
        /// </returns>
        /// <exception cref="BluetoothErrorException">Failed to retrieve  info for {deviceAddress}.</exception>
        public async Task<string> DeviceInfo(string deviceAddress, CancellationToken cancellationToken = default)
        {
            var info = await ProcessRunner.GetProcessOutputAsync(BcCommand, $"info {deviceAddress}", cancellationToken)
                .ConfigureAwait(false);

            return !string.IsNullOrEmpty(info)
                ? info
                : throw new BluetoothErrorException($"Failed to retrieve  info for {deviceAddress}");
        }
    }
}