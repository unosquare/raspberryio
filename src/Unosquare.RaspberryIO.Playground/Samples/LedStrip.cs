namespace Unosquare.RaspberryIO.Playground.Samples
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an SPI addressable strip of RGB LEDs
    /// Model number APA102C
    /// </summary>
    public class LedStrip
    {
        /// <summary>
        /// Represents an LED in a strip of LEDs
        /// </summary>
        public class LedStripPixel
        {
            private readonly int BaseAddress;
            private readonly LedStrip Owner;

            public LedStripPixel(LedStrip owner, int baseAddress)
            {
                Owner = owner;
                BaseAddress = baseAddress;
            }

            public float Brightness
            {
                get
                {
                    var brightnessByte = (byte)(BrightnessGetMask & Owner.FrameBuffer[BaseAddress]);
                    return brightnessByte / 32f;
                }
                set
                {
                    if (value < 0f) value = 0f;
                    if (value > 1f) value = 1f;

                    Owner.FrameBuffer[BaseAddress] =
                        (byte)((byte)(value * 32) | BrightnessSetMask);
                }
            }
            public byte R
            {
                get
                {
                    return Owner.ReverseRgb ? Owner.FrameBuffer[BaseAddress + 3] : Owner.FrameBuffer[BaseAddress + 1];
                }
                set
                {
                    if (Owner.ReverseRgb)
                        Owner.FrameBuffer[BaseAddress + 3] = value;
                    else
                        Owner.FrameBuffer[BaseAddress + 1] = value;
                }
            }
            public byte G
            {
                get
                {
                    return Owner.FrameBuffer[BaseAddress + 2];
                }
                set
                {
                    Owner.FrameBuffer[BaseAddress + 2] = value;
                }
            }
            public byte B
            {
                get
                {
                    return Owner.ReverseRgb ? Owner.FrameBuffer[BaseAddress + 1] : Owner.FrameBuffer[BaseAddress + 3];
                }
                set
                {
                    if (Owner.ReverseRgb)
                        Owner.FrameBuffer[BaseAddress + 1] = value;
                    else
                        Owner.FrameBuffer[BaseAddress + 3] = value;
                }
            }
        }


        #region Static Declarations

        private const byte BrightnessSetMask = 0xE0;
        private const byte BrightnessGetMask = 0x1F;

        private static readonly byte[] StartFrame = new byte[4];
        private static readonly byte[] EndFrame = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };

        #endregion

        #region State Variables

        private readonly object SyncLock = new object(); // for thread safety
        private readonly SpiChannel Channel; // will be set in the constructor
        private readonly byte[] ClearBuffer; // Contains clear pixel data which is a set of 0xE0 bytes
        private readonly byte[] FrameBuffer; // Contains what needs to be written over the SPI channel
        private readonly byte[] PixelHolder = new byte[4]; // used heavily to manipulate pixels
        private readonly Dictionary<int, LedStripPixel> Pixels = new Dictionary<int, LedStripPixel>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LedStrip"/> class.
        /// </summary>
        /// <param name="ledCount">The led count.</param>
        /// <param name="spiChannel">The spi channel.</param>
        /// <param name="spiFrequency">The spi frequency.</param>
        /// <param name="reverseRgb">if set to <c>true</c> [reverse RGB].</param>
        public LedStrip(int ledCount = 60, int spiChannel = 1, int spiFrequency = 12000000, bool reverseRgb = true)
        {
            // Basic properties
            LedCount = ledCount;
            ReverseRgb = reverseRgb;

            // Create the frame buffer
            FrameBuffer = new byte[(PixelHolder.Length * LedCount) + (StartFrame.Length + EndFrame.Length)];
            Buffer.BlockCopy(StartFrame, 0, FrameBuffer, 0, StartFrame.Length);
            Buffer.BlockCopy(EndFrame, 0, FrameBuffer, FrameBuffer.Length - EndFrame.Length, EndFrame.Length);

            // Create ther Clear buffer
            ClearBuffer = new byte[PixelHolder.Length * LedCount];
            for (var baseAddress = 0; baseAddress < LedCount * PixelHolder.Length; baseAddress += PixelHolder.Length)
            {
                Buffer.SetByte(ClearBuffer, baseAddress, BrightnessSetMask);
            }

            // Set all the pixels to no value
            ClearPixels();

            // Select the SPI channel
            if (spiChannel == 0)
            {
                Pi.Spi.Channel0Frequency = spiFrequency;
                Channel = Pi.Spi.Channel0;
            }
            else
            {
                Pi.Spi.Channel1Frequency = spiFrequency;
                Channel = Pi.Spi.Channel1;
            }

            Render();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether RGB values are sent as GBR
        /// </summary>
        public bool ReverseRgb { get; private set; }

        /// <summary>
        /// Gets the LED count.
        /// </summary>
        public int LedCount { get; private set; }


        /// <summary>
        /// Gets the <see cref="LedStripPixel"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="LedStripPixel"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public LedStripPixel this[int index]
        {
            get { return GetPixel(index); }
        }

        #endregion

        #region Pixel Methods and Rendering

        /// <summary>
        /// Clears all the pixels.
        /// </summary>
        public void ClearPixels()
        {
            lock (SyncLock)
            {
                Buffer.BlockCopy(ClearBuffer, 0, FrameBuffer, StartFrame.Length, ClearBuffer.Length);
            }
        }

        /// <summary>
        /// Gets the pixel.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public LedStripPixel GetPixel(int index)
        {
            if (index < 0 || index > LedCount - 1)
                return null;

            lock (SyncLock)
            {
                if (Pixels.ContainsKey(index) == false)
                    Pixels[index] = new LedStripPixel(this, StartFrame.Length + (index * PixelHolder.Length));

                return Pixels[index];
            }
        }

        /// <summary>
        /// Sets the pixel.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="brightness">The brightness.</param>
        /// <param name="r">The Red.</param>
        /// <param name="g">The Green.</param>
        /// <param name="b">The Blue.</param>
        public void SetPixel(int index, float brightness, byte r, byte g, byte b)
        {
            lock (SyncLock)
            {
                if (index < 0 || index > LedCount - 1)
                    return;

                var pixel = this[index];
                pixel.R = r;
                pixel.G = g;
                pixel.B = b;
                pixel.Brightness = brightness;
            }

        }

        /// <summary>
        /// Renders all the pixels.
        /// </summary>
        public void Render()
        {
            lock (SyncLock)
            {
                Channel.Write(FrameBuffer);
            }
        }

        #endregion

    }
}
