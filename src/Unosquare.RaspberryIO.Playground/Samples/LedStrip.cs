namespace Unosquare.RaspberryIO.Playground.Samples
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

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
                    if (value < 0f) value = 0f;
                    if (value > 1f) value = 1f;

                    var brightnessByte = (byte)(value * 31);
                    Owner.FrameBuffer[BaseAddress] = (byte)(brightnessByte | BrightnessSetMask);
                }
            }

            /// <summary>
            /// The Red Buye
            /// </summary>
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

            /// <summary>
            /// The green
            /// </summary>
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

            /// <summary>
            /// The blue
            /// </summary>
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

        #endregion

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
        public void SetPixels(PixelData pixels, int sourceOffsetX, int sourceOffsetY, float brightness = 1f, int targetOffset = 0, int targetLength = 0)
        {
            var brightnessByte = default(byte);

            { // Parameter validation
                if (pixels == null)
                    throw new ArgumentNullException(nameof(pixels));

                if (sourceOffsetX < 0 || sourceOffsetX > (pixels.ImageWidth - targetLength) - 1)
                    throw new ArgumentOutOfRangeException(nameof(sourceOffsetX));

                if (sourceOffsetY < 0 || sourceOffsetY >= pixels.ImageHeight)
                    throw new ArgumentOutOfRangeException(nameof(sourceOffsetY), 
                        $"{nameof(sourceOffsetY)} was '{sourceOffsetY}' but it must be between '0' and '{pixels.ImageHeight - 1}'");

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

            }

            // Offset settings
            var BOffset = ReverseRgb ? 1 : 3; var GOffset = 2; var ROffset = ReverseRgb ? 3 : 1; var TOfsset = 0;

            // Pixel copying
            lock (SyncLock)
            {
                var bmpOffsetBase = pixels.GetPixelOffset(sourceOffsetX, sourceOffsetY);
                var bmpOffsetLimit = bmpOffsetBase + (targetLength * PixelData.BytesPerPixel);
                var setCount = 0;

                var frameBufferOffset = StartFrame.Length + (targetOffset * StartFrame.Length);

                for (var bmpOffset = bmpOffsetBase; bmpOffset < bmpOffsetLimit; bmpOffset += PixelData.BytesPerPixel)
                {
                    FrameBuffer[frameBufferOffset + TOfsset] = brightnessByte;
                    FrameBuffer[frameBufferOffset + ROffset] = pixels.Data[bmpOffset + PixelData.ROffset]; // R
                    FrameBuffer[frameBufferOffset + GOffset] = pixels.Data[bmpOffset + PixelData.GOffset]; // G
                    FrameBuffer[frameBufferOffset + BOffset] = pixels.Data[bmpOffset + PixelData.BOffset]; // B
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

    /// <summary>
    /// Represents a buffer of bytes containing pixels in BGRA byte order
    /// loaded from an image that is passed on to the constructor
    /// This class should be promoted to Swan maybe?
    /// https://github.com/unosquare/swan
    /// </summary>
    public class PixelData
    {

        #region Constant Definitions

        public const int BytesPerPixel = 4;

        public const int BOffset = 0;
        public const int GOffset = 1;
        public const int ROffset = 2;
        public const int AOffset = 3;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PixelData"/> class.
        /// Data will not contain left-over stride bytes
        /// </summary>
        /// <param name="sourceImage">The source image.</param>
        public PixelData(Image sourceImage)
        {

            // Acquire or create the source bitmap in a manageable format
            var sourceBitmap = sourceImage as Bitmap;
            var disposeSourceBitmap = false;
            if (sourceBitmap == null || sourceBitmap.PixelFormat != PixelFormat)
            {
                sourceBitmap = new Bitmap(sourceImage.Width, sourceImage.Height, PixelFormat);
                using (var g = Graphics.FromImage(sourceBitmap))
                {
                    //g.Clear(Color.Black);
                    g.DrawImage(sourceImage, 0, 0);
                }

                // We created this bitmap. Make sure we clear it from memory
                disposeSourceBitmap = true;
            }

            // Lock the bits
            var sourceDataLocker = sourceBitmap.LockBits(
                new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
                ImageLockMode.ReadOnly, sourceBitmap.PixelFormat);

            // Set basic properties
            ImageWidth = sourceBitmap.Width;
            ImageHeight = sourceBitmap.Height;
            LineStride = sourceDataLocker.Stride;

            // State variables
            LineLength = sourceBitmap.Width * BytesPerPixel; // may or may not be equal to the Stride
            Data = new byte[LineLength * sourceBitmap.Height];
            var scanLineAddress = sourceDataLocker.Scan0; // get a pointer to the first pixel of the image

            // copy line by line in order to ignore the useless left-over stride
            for (var y = 0; y < sourceBitmap.Height; y++)
            {
                scanLineAddress = sourceDataLocker.Scan0 + (sourceDataLocker.Stride * y);
                Marshal.Copy(scanLineAddress, Data, (y * LineLength), LineLength);
            }

            // finally unlock the bitmap
            sourceBitmap.UnlockBits(sourceDataLocker);

            // dispose the source bitmap if we had to create it
            if (disposeSourceBitmap)
            {
                sourceBitmap.Dispose();
                sourceBitmap = null;
            }

        }

        #endregion

        #region Properties

        /// <summary>
        /// Contains all the bytes of the pixel data
        /// Manual manipulation is not recommended
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// Gets the width of the image.
        /// </summary>
        public int ImageWidth { get; }

        /// <summary>
        /// Gets the height of the image.
        /// </summary>
        public int ImageHeight { get; }

        /// <summary>
        /// Gets the pixel format. This will always be Format32bppArgb
        /// </summary>
        public PixelFormat PixelFormat { get; } = PixelFormat.Format32bppArgb;

        /// <summary>
        /// Gets the length in bytes of a line of pixel data.
        /// Basically the same as Line Length except Stride might be a little larger as
        /// some bitmaps might be DWORD-algned
        /// </summary>
        public int LineStride { get; }

        /// <summary>
        /// Gets the length in bytes of a line of pixel data.
        /// Basically the same as Stride except Stride might be a little larger as
        /// some bitmaps might be DWORD-algned
        /// </summary>
        public int LineLength { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the index of the first byte in the BGRA pixel data for the given image coordinates.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// x
        /// or
        /// y
        /// </exception>
        public int GetPixelOffset(int x, int y)
        {
            if (x < 0 || x > ImageWidth) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y > ImageHeight) throw new ArgumentOutOfRangeException(nameof(y));

            return (y * LineLength) + (x * BytesPerPixel);
        }

        #endregion

    }

}
