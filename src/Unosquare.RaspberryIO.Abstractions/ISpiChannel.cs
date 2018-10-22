namespace Unosquare.RaspberryIO.Abstractions
{
    /// <summary>
    /// Interfaces a SPI buses on the GPIO.
    /// </summary>
    public interface ISpiChannel
    {
        /// <summary>
        /// Gets the standard initialization file descriptor.
        /// anything negative means error.
        /// </summary>
        /// <value>
        /// The file descriptor.
        /// </value>
        int FileDescriptor { get; }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        int Channel { get; }

        /// <summary>
        /// Gets the frequency.
        /// </summary>
        int Frequency { get; }

        /// <summary>
        /// Sends data and simultaneously receives the data in the return buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>The read bytes from the ring-style bus.</returns>
        byte[] SendReceive(byte[] buffer);

        /// <summary>
        /// Writes the specified buffer the the underlying FileDescriptor.
        /// Do not use this method if you expect data back.
        /// This method is efficient if used in a fire-and-forget scenario
        /// like sending data over to those long RGB LED strips.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        void Write(byte[] buffer);
    }
}
