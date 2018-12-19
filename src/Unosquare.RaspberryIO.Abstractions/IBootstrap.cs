namespace Unosquare.RaspberryIO.Abstractions
{
    /// <summary>
    /// Interface for bootstrapping an <see cref="Abstractions"/> implementation.
    /// </summary>
    public interface IBootstrap
    {
        /// <summary>
        /// Bootstraps an <see cref="Abstractions"/> implementation.
        /// </summary>
        void Bootstrap();
    }
}
