#if NET452
namespace Unosquare.RaspberryIO.Playground.Samples
{
    using Gpio;
    using Swan;
    using Swan.Formatters;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an SPI addressable strip of RGB LEDs
    /// Model number APA102C
    /// This strip is also known as Adafruit DotStar LED strip (just a new name for an existing item)
    /// You can get it here: https://www.adafruit.com/products/2239
    /// or here: https://www.aliexpress.com/wholesale?SearchText=APA102C
    /// </summary>
    public class LedStrip
    {
        #region Support Classes

        /// <summary>
        /// Represents an LED in a strip of LEDs
        /// This class is not meant to be instantiated by the user.
        /// </summary>
        public class LedStripPixel
        {
            private readonly int BaseAddress;
            private readonly LedStrip Owner;

            /// <summary>
            /// Initializes a new instance of the <see cref="LedStripPixel"/> class.
            /// </summary>
            /// <param name="owner">The owner.</param>
            /// <param name="baseAddress">The base address.</param>
            public LedStripPixel(LedStrip owner, int baseAddress)
            {
                Owner = owner;
                BaseAddress = baseAddress;
            }

            /// <summary>
            /// Gets or sets the brightness, from 0 to 1.
            /// </summary>
            public float Brightness
            {
                get
                {
                    var brightnessByte = (byte)(BrightnessGetMask & Owner.FrameBuffer[BaseAddress]);
                    return brightnessByte / 31f;
                }
                set
                {
                    // clamp value
                    value = value.Clamp(0f, 1f);
                    var brightnessByte = (byte)(value * 31);
                    Owner.FrameBuffer[BaseAddress] = (byte)(brightnessByte | BrightnessSetMask);
                }
            }

            /// <summary>
            /// The Red Buye
            /// </summary>
            public byte R
            {
                get => Owner.ReverseRgb ? Owner.FrameBuffer[BaseAddress + 3] : Owner.FrameBuffer[BaseAddress + 1];
                set
                {
                    if (Owner.ReverseRgb)
                        Owner.FrameBuffer[BaseAddress + 3] = value;
                    else
                        Owner.FrameBuffer[BaseAddress + 1] = value;
                }
            }

            /// <summary>
            /// The green
            /// </summary>
            public byte G
            {
                get => Owner.FrameBuffer[BaseAddress + 2];
                set => Owner.FrameBuffer[BaseAddress + 2] = value;
            }

            /// <summary>
            /// The blue
            /// </summary>
            public byte B
            {
                get => Owner.ReverseRgb ? Owner.FrameBuffer[BaseAddress + 1] : Owner.FrameBuffer[BaseAddress + 3];
                set
                {
                    if (Owner.ReverseRgb)
                        Owner.FrameBuffer[BaseAddress + 1] = value;
                    else
                        Owner.FrameBuffer[BaseAddress + 3] = value;
                }
            }
        }

        #endregion

        #region Static Declarations

        private const byte BrightnessSetMask = 0xE0;
        private const byte BrightnessGetMask = 0x1F;

        private static readonly byte[] StartFrame = new byte[4];
        private static readonly byte[] EndFrame = { 0xFF, 0xFF, 0xFF, 0xFF };

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
        /// <param name="ledCount">The length of the stip.</param>
        /// <param name="spiChannel">The SPI channel.</param>
        /// <param name="spiFrequency">The SPI frequency.</param>
        /// <param name="reverseRgb">if set to <c>true</c> colors will be sent to the strip as BGR, otherwise as RGB.</param>
        public LedStrip(int ledCount = 60, int spiChannel = 1, int spiFrequency = SpiChannel.DefaultFrequency, bool reverseRgb = true)
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
        /// This is typically true
        /// </summary>
        public bool ReverseRgb { get; }

        /// <summary>
        /// Gets the LED count.
        /// </summary>
        public int LedCount { get; }

        /// <summary>
        /// Gets the <see cref="LedStripPixel"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="LedStripPixel"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public LedStripPixel this[int index] => GetPixel(index);

        #endregion

        #region Pixel Methods and Rendering

        /// <summary>
        /// Clears all the pixels. Call the Render method to apply the changes.
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
        /// Sets the pixel brightness, R, G and B at the given index.
        /// And invalid index has no effect
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
        /// Sets a number of pixels from loaded pixel data directly into the frame buffer
        /// This is the fastest method to set a number of pixels.
        /// Call the Render Method to apply!
        /// </summary>
        /// <param name="pixels">The pixel data of a previously loaded bitmap.</param>
        /// <param name="sourceOffsetX">The x offset in the bitmap to start copying pixels from</param>
        /// <param name="sourceOffsetY">The row index (y) from which to take the pixel data</param>
        /// <param name="brightness">The brightness from 0.0 to 1.0. The underlying precision is from 0 to 31</param>
        /// <param name="targetOffset">The target offset where to start setting the pixels.</param>
        /// <param name="targetLength">The number of pixels to set. 0 or less means the entire LedCount</param>
        /// <exception cref="ArgumentOutOfRangeException">startX
        /// or
        /// y</exception>
        /// <exception cref="ArgumentNullException">bitmap</exception>
        public void SetPixels(BitmapBuffer pixels, int sourceOffsetX, int sourceOffsetY, float brightness = 1f, int targetOffset = 0, int targetLength = 0)
        {
            var brightnessByte = default(byte);

            // Parameter validation
            if (pixels == null)
                throw new ArgumentNullException(nameof(pixels));

            if (sourceOffsetX < 0 || sourceOffsetX > (pixels.ImageWidth - targetLength) - 1)
                throw new ArgumentOutOfRangeException(nameof(sourceOffsetX));

            if (sourceOffsetY < 0 || sourceOffsetY >= pixels.ImageHeight)
            {
                throw new ArgumentOutOfRangeException(nameof(sourceOffsetY),
                    $"{nameof(sourceOffsetY)} was '{sourceOffsetY}' but it must be between '0' and '{pixels.ImageHeight - 1}'");
            }

            if (targetOffset < 0) targetOffset = 0;
            if (targetOffset > LedCount - 1)
                throw new ArgumentOutOfRangeException(nameof(targetOffset));

            if (targetLength <= 0)
                targetLength = LedCount;

            if (targetOffset + targetLength > LedCount)
                throw new ArgumentOutOfRangeException(nameof(targetLength));

            // Brightness Setting
            if (brightness < 0f) brightness = 0f;
            if (brightness > 1f) brightness = 1f;
            brightnessByte = (byte)(brightness * 31);
            brightnessByte = (byte)(brightnessByte | BrightnessSetMask);

            // Offset settings
            var bOffset = ReverseRgb ? 1 : 3;
            const int GOffset = 2;
            var ROffset = ReverseRgb ? 3 : 1;
            var TOfsset = 0;

            // Pixel copying
            lock (SyncLock)
            {
                var bmpOffsetBase = pixels.GetPixelOffset(sourceOffsetX, sourceOffsetY);
                var bmpOffsetLimit = bmpOffsetBase + (targetLength * BitmapBuffer.BytesPerPixel);
                var setCount = 0;

                var frameBufferOffset = StartFrame.Length + (targetOffset * StartFrame.Length);

                for (var bmpOffset = bmpOffsetBase; bmpOffset < bmpOffsetLimit; bmpOffset += BitmapBuffer.BytesPerPixel)
                {
                    FrameBuffer[frameBufferOffset + TOfsset] = brightnessByte;
                    FrameBuffer[frameBufferOffset + ROffset] = pixels.Data[bmpOffset + BitmapBuffer.ROffset]; // R
                    FrameBuffer[frameBufferOffset + GOffset] = pixels.Data[bmpOffset + BitmapBuffer.GOffset]; // G
                    FrameBuffer[frameBufferOffset + bOffset] = pixels.Data[bmpOffset + BitmapBuffer.BOffset]; // B
                    frameBufferOffset += StartFrame.Length;
                    setCount += 1;

                    if (setCount >= targetLength)
                        break;
                }
            }
        }

        /// <summary>
        /// Renders all the pixels in the FrameBuffer
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
#endif