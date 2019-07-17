namespace Unosquare.RaspberryIO.Computer
{
    using System.Collections.Generic;
    using Unosquare.Swan.Abstractions;

    /// <summary>
    /// Represents the bluetooth information.
    /// </summary>
    public class Bluetooth : SingletonBase<Bluetooth>
    {
        public bool PowerOn()
        {
            return true;
        }

        public bool PowerOff()
        {
            return true;
        }

        public List<string> ListDevices()
        {
            return new List<string>();
        }
    }
}
