namespace Unosquare.RaspberryIO.Peripherals
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a buffer of bytes containing pixels in BGRA byte order
    /// loaded from an image that is passed on to the constructor.
    /// Data contains all the raw bytes (without scanline left-over bytes)
    /// where they can be quickly changed and then a new bitmap
    /// can be created from the byte data.
    /// </summary>
    public class BitmapBuffer
    {
        /// <summary>
        /// A constant representing the number of
        /// bytes per pixel in the pixel data. This is
        /// always 4 but it is kept here for readability.
        /// </summary>
        public const int BytesPerPixel = 4;

        /// <summary>
        /// The blue byte offset within a pixel offset. This is 0.
        /// </summary>
        public const int BOffset = 0;

        /// <summary>
        /// The green byte offset within a pixel offset.  This is 1.
        /// </summary>
        public const int GOffset = 1;

        /// <summary>
        /// The red byte offset within a pixel offset.  This is 2.
        /// </summary>
        public const int ROffset = 2;

        /// <summary>
        /// The alpha byte offset within a pixel offset.  This is 3.
        /// </summary>
        public const int AOffset = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapBuffer"/> class.
        /// Data will not contain left-over stride bytes.
        /// </summary>
        /// <param name="sourceImage">The source image.</param>
        public BitmapBuffer(Image sourceImage)
        {
            // Acquire or create the source bitmap in a manageable format
            var disposeSourceBitmap = false;
            if (!(sourceImage is Bitmap sourceBitmap) || sourceBitmap.PixelFormat != PixelFormat)
            {
                sourceBitmap = new Bitmap(sourceImage.Width, sourceImage.Height, PixelFormat);
                using (var g = Graphics.FromImage(sourceBitmap))
                {
                    g.DrawImage(sourceImage, 0, 0);
                }

                // We created this bitmap. Make sure we clear it from memory
                disposeSourceBitmap = true;
            }

            // Lock the bits
            var sourceDataLocker = sourceBitmap.LockBits(
                new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
                ImageLockMode.ReadOnly,
                sourceBitmap.PixelFormat);

            // Set basic properties
            ImageWidth = sourceBitmap.Width;
            ImageHeight = sourceBitmap.Height;
            LineStride = sourceDataLocker.Stride;

            // State variables
            LineLength = sourceBitmap.Width * BytesPerPixel; // may or may not be equal to the Stride
            Data = new byte[LineLength * sourceBitmap.Height];

            // copy line by line in order to ignore the useless left-over stride
            Parallel.For(0, sourceBitmap.Height, y =>
            {
                var sourceAddress = sourceDataLocker.Scan0 + (sourceDataLocker.Stride * y);
                var targetAddress = y * LineLength;
                Marshal.Copy(sourceAddress, Data, targetAddress, LineLength);
            });

            // finally unlock the bitmap
            sourceBitmap.UnlockBits(sourceDataLocker);

            // dispose the source bitmap if we had to create it
            if (disposeSourceBitmap)
            {
                sourceBitmap.Dispose();
            }
        }

        /// <summary>
        /// Contains all the bytes of the pixel data
        /// Each horizontal scanline is represented by LineLength
        /// rather than by LinceStride. The left-over stride bytes
        /// are removed.
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
        /// Gets the pixel format. This will always be Format32bppArgb.
        /// </summary>
        public PixelFormat PixelFormat { get; } = PixelFormat.Format32bppArgb;

        /// <summary>
        /// Gets the length in bytes of a line of pixel data.
        /// Basically the same as Line Length except Stride might be a little larger as
        /// some bitmaps might be DWORD-algned.
        /// </summary>
        public int LineStride { get; }

        /// <summary>
        /// Gets the length in bytes of a line of pixel data.
        /// Basically the same as Stride except Stride might be a little larger as
        /// some bitmaps might be DWORD-algned.
        /// </summary>
        public int LineLength { get; }

        /// <summary>
        /// Gets the index of the first byte in the BGRA pixel data for the given image coordinates.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>Index of the first byte in the BGRA pixel.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// x
        /// or
        /// y.
        /// </exception>
        public int GetPixelOffset(int x, int y)
        {
            if (x < 0 || x > ImageWidth) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y > ImageHeight) throw new ArgumentOutOfRangeException(nameof(y));

            return (y * LineLength) + (x * BytesPerPixel);
        }

        /// <summary>
        /// Converts the pixel data bytes held in the buffer
        /// to a 32-bit RGBA bitmap.
        /// </summary>
        /// <returns>Pixel data for a graphics image and its attribute.</returns>
        public Bitmap ToBitmap()
        {
            var bitmap = new Bitmap(ImageWidth, ImageHeight, PixelFormat);
            var bitLocker = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

            Parallel.For(0, bitmap.Height, y =>
            {
                var sourceOffset = GetPixelOffset(0, y);
                var targetOffset = bitLocker.Scan0 + (y * bitLocker.Stride);
                Marshal.Copy(Data, sourceOffset, targetOffset, bitLocker.Width);
            });

            bitmap.UnlockBits(bitLocker);

            return bitmap;
        }
    }
}