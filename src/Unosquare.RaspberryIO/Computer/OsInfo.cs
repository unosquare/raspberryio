namespace Unosquare.RaspberryIO.Computer
{
    /// <summary>
    /// Represents the OS Information
    /// </summary>
    public class OsInfo
    {
        /// <summary>
        /// System name
        /// </summary>
        public string SysName;
        /// <summary>
        /// Node name
        /// </summary>
        public string NodeName;
        /// <summary>
        /// Release level
        /// </summary>
        public string Release;
        /// <summary>
        /// Version level
        /// </summary>
        public string Version;
        /// <summary>
        /// Hardware level
        /// </summary>
        public string Machine;
        /// <summary>
        /// Domain name
        /// </summary>
        public string DomainName;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{SysName} {Release} {Version}";
        }
    }
}
