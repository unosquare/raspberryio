using Unosquare.Swan;
using Unosquare.Swan.Components;

namespace Unosquare.RaspberryIO.Computer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Unosquare.Swan.Abstractions;

    /// <summary>
    /// Represents the bluetooth information.
    /// </summary>
    public class Bluetooth : SingletonBase<Bluetooth>
    {
        public async Task<bool> PowerOn()
        {
            var openBluetoothTool = await ProcessRunner.GetProcessOutputAsync("bluetoothctl").ConfigureAwait(false); //Open bluetooth tool
            $"{openBluetoothTool}".Info();

            var turnOnBluetoothTool = await ProcessRunner.GetProcessOutputAsync("power on").ConfigureAwait(false); //turn on bluetooth
            $"{turnOnBluetoothTool}".Info();

            return true;
        }

        public async Task<bool> PowerOff()
        {

            var openBluetoothTool = await ProcessRunner.GetProcessOutputAsync("bluetoothctl").ConfigureAwait(false); //Open bluetooth tool
            $"{openBluetoothTool}".Info();

            var turnOnBluetoothTool = await ProcessRunner.GetProcessOutputAsync("power off").ConfigureAwait(false); //turn on bluetooth
            $"{turnOnBluetoothTool}".Info();

            return true;
        }

        public async Task<List<string>> ListDevices()
        {
            var openBluetoothTool = await ProcessRunner.GetProcessOutputAsync("list").ConfigureAwait(false); //Open bluetooth tool
            $"{openBluetoothTool}".Info();
            return new List<string>();

        }
    }
}
