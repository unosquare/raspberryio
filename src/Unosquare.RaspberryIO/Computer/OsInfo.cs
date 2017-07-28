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
        public string SysName { get; set; }

        /// <summary>
        /// Node name
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// Release level
        /// </summary>
        public string Release { get; set; }

        /// <summary>
        /// Version level
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Hardware level
        /// </summary>
        public string Machine { get; set; }

        /// <summary>
        /// Domain name
        /// </summary>
        public string DomainName { get; set; }

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
