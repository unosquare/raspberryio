namespace Unosquare.RaspberryIO.Peripherals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using Abstractions;

    /// <summary>
    /// Provides an I2C interface to OLED displays
    /// using the SSD1306 driver.
    /// </summary>
    public sealed class OledDisplaySsd1306 : IDisposable
    {
        #region Private Fields

        private static readonly Dictionary<DisplayModel, byte> DisplayClockDivider = new Dictionary<DisplayModel, byte>
        {
            { DisplayModel.Display128X64, 0x80 },
            { DisplayModel.Display128X32, 0x80 },
            { DisplayModel.Display96X16, 0x60 },
        };

        private static readonly Dictionary<DisplayModel, byte> MultiplexSetting = new Dictionary<DisplayModel, byte>
        {
            { DisplayModel.Display128X64, 0x3F },
            { DisplayModel.Display128X32, 0x1F },
            { DisplayModel.Display96X16, 0x0F },
        };

        private static readonly Dictionary<DisplayModel, byte> ComPins = new Dictionary<DisplayModel, byte>
        {
            { DisplayModel.Display128X64, 0x12 },
            { DisplayModel.Display128X32, 0x02 },
            { DisplayModel.Display96X16, 0x02 },
        };

        private static readonly Bitmap FontBitmap;
        private static readonly byte[] FontBytemap;
        private static readonly int _fontBitmapCharWidth;
        private static readonly int _fontBitmapCharHeight;

        private readonly BitArray _bitBuffer;
        private readonly byte[] _byteBuffer;
        private readonly int _bufferPageCount;
        private readonly int _bitsPerPage;

        private bool _isDisposed;
        private byte _mContrast;
        private bool _mIsActive;

        #endregion

        #region Constructors

        static OledDisplaySsd1306()
        {
            var resourceNames = typeof(OledDisplaySsd1306).Assembly.GetManifestResourceNames();
            var fontResource = resourceNames.First(r => r.Contains("AsciiFontPng"));
            using (var stream = typeof(OledDisplaySsd1306).Assembly.GetManifestResourceStream(fontResource))
            {
                FontBitmap = new Bitmap(stream);
                _fontBitmapCharWidth = FontBitmap.Width / 257;
                _fontBitmapCharHeight = FontBitmap.Height;

                FontBytemap = new byte[FontBitmap.Width];

                // Parallel.For(0, FontBitmap.Width, (offsetX) =>
                for (var offsetX = 0; offsetX < FontBitmap.Width; offsetX++)
                {
                    var verticalByte = 0;
                    for (var bitIndex = 7; bitIndex >= 0; bitIndex--)
                    {
                        verticalByte = verticalByte << 1;
                        if (FontBitmap.GetPixel(offsetX, bitIndex).GetBrightness() > 0.5)
                            verticalByte |= 0x01;
                    }

                    FontBytemap[offsetX] = (byte)verticalByte;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OledDisplaySsd1306"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public OledDisplaySsd1306(DisplayModel model)
           : this(GetDefaultDevice(), model, VccSourceMode.Switching)
        {
            // placeholder
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OledDisplaySsd1306"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="model">The model.</param>
        /// <param name="vccSource">State of the VCC.</param>
        /// <exception cref="ArgumentException">Invalid display model - model.</exception>
        /// <exception cref="ArgumentNullException">device.</exception>
        public OledDisplaySsd1306(II2CDevice device, DisplayModel model, VccSourceMode vccSource)
        {
            switch (model)
            {
                case DisplayModel.Display128X32:
                    {
                        Width = 128;
                        Height = 32;
                        break;
                    }

                case DisplayModel.Display128X64:
                    {
                        Width = 128;
                        Height = 64;
                        break;
                    }

                case DisplayModel.Display96X16:
                    {
                        Width = 96;
                        Height = 16;
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("Invalid display model", nameof(model));
                    }
            }

            Model = model;
            Device = device ?? throw new ArgumentNullException(nameof(device));
            VccSource = vccSource;
            _bufferPageCount = Height / 8;
            _bitBuffer = new BitArray(Width * Height);
            _byteBuffer = new byte[1 + (_bitBuffer.Length / 8)];
            _byteBuffer[0] = 0x40; // The first byte signals data
            _bitsPerPage = 8 * Width;
            Initialize();
            IsActive = true;
        }

        #endregion

        #region Enumerations

        /// <summary>
        /// ENumerates the different OLED display models.
        /// </summary>
        public enum DisplayModel
        {
            /// <summary>
            /// The display 128x64
            /// </summary>
            Display128X64 = 0,

            /// <summary>
            /// The display 128x32 (pioled)
            /// </summary>
            Display128X32 = 1,

            /// <summary>
            /// The display 96x16
            /// </summary>
            Display96X16 = 2,
        }

        /// <summary>
        /// Enumerates the different Power Source modes.
        /// </summary>
        public enum VccSourceMode
        {
            /// <summary>
            /// External power supply
            /// </summary>
            External = 0x1,

            /// <summary>
            /// Integrated switching capacitor power supply
            /// </summary>
            Switching = 0x2,
        }

        /// <summary>
        /// Enumerates the different commands.
        /// </summary>
        private enum Command
        {
            SetContrast = 0x81,
            Resume = 0xA4,
            EntireDisplayOn = 0xA5,
            DisplayModeNormal = 0xA6,
            DisplayModeInvert = 0xA7,
            TurnOff = 0xAE,
            TurnOn = 0xAF,
            SetDisplayOffset = 0xD3,
            SetComPins = 0xDA,
            SetVoltageOutput = 0xDB,
            SetClockDivider = 0xD5,
            SetPrechargeMode = 0xD9,
            SetMultiplexer = 0xA8,
            SetLowColumn = 0x00,
            SetHighColumn = 0x10,
            SetStartLine = 0x40,
            SetMemoryMode = 0x20,
            SetColumnAddressRange = 0x21,
            SetPageAddressRange = 0x22,
            ScanDirectionModeIncrement = 0xC0,
            ScanDirectionModeDecrement = 0xC8,
            SegmentRemapModeOff = 0xA0,
            SegmentRemapModeOn = 0xA1,
            SetChargePumpMode = 0x8D,
        }

        #endregion

        /// <summary>
        /// Gets the device.
        /// </summary>
        public II2CDevice Device { get; }

        /// <summary>
        /// Gets the display pixel width.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the display pixel height.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the VCC source mode.
        /// </summary>
        public VccSourceMode VccSource { get; }

        /// <summary>
        /// Gets the display model.
        /// </summary>
        public DisplayModel Model { get; }

        /// <summary>
        /// Gets or sets the contrast from 0 to 255.
        /// </summary>
        public byte Contrast
        {
            get => _mContrast;
            set
            {
                SendCommand(Command.SetContrast, value);
                _mContrast = value;
            }
        }

        /// <summary>
        /// Gets or sets if the display is turned on.
        /// </summary>
        public bool IsActive
        {
            get => _mIsActive;
            set
            {
                SendCommand(value ? Command.TurnOn : Command.TurnOff);
                _mIsActive = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the specified pixel coordinate.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>The value (true or false).</returns>
        public bool this[int x, int y]
        {
            get => _bitBuffer[GetBitIndex(x, y)];
            set => _bitBuffer[GetBitIndex(x, y)] = value;
        }

        /// <summary>
        /// Gets the pixel buffer value at the given coordinates.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>If the bit is set or not.</returns>
        public bool GetPixel(int x, int y) => this[x, y];

        /// <summary>
        /// Sets the pixel buffer value at the given coordinates.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public void SetPixel(int x, int y, bool value) => this[x, y] = value;

        /// <summary>
        /// Gets the text bitmap using the default bitmap font.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>A bitmap containing the text using the bitmap font.</returns>
        public Bitmap GetTextBitmap(string text)
        {
            var chars = Encoding.ASCII.GetBytes(text);
            var bitmap = new Bitmap(chars.Length * _fontBitmapCharWidth, _fontBitmapCharHeight);
            var offsetX = 0;

            using (var g = Graphics.FromImage(bitmap))
            {
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.SmoothingMode = SmoothingMode.None;
                g.InterpolationMode = InterpolationMode.Low;
                g.Clear(Color.Black);

                foreach (var c in chars)
                {
                    var glyphRect = new Rectangle(c * _fontBitmapCharWidth, 0, _fontBitmapCharWidth, _fontBitmapCharHeight);
                    g.DrawImage(FontBitmap, offsetX, 0, glyphRect, GraphicsUnit.Pixel);
                    offsetX += _fontBitmapCharWidth;
                }

                g.Flush();
            }

            return bitmap;
        }

        /// <summary>
        /// Draws the text lines on an existing bitmap and loads it to the bitmap buffer.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="lines">The lines.</param>
        public void DrawText(Bitmap bitmap, params string[] lines) =>
            DrawText(bitmap, null, lines);

        /// <summary>
        /// Draws the text lines on an existing bitmap and loads it to the bitmap buffer.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="g">The existing graphics context.</param>
        /// <param name="lines">The lines.</param>
        public void DrawText(Bitmap bitmap, Graphics g, params string[] lines)
        {
            var disposeGraphics = false;
            if (g == null)
            {
                g = Graphics.FromImage(bitmap);
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.SmoothingMode = SmoothingMode.None;
                g.InterpolationMode = InterpolationMode.Low;
                disposeGraphics = true;
            }

            for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                var text = lines[lineIndex];
                var offsetY = lineIndex * _fontBitmapCharHeight;
                var textBitmap = GetTextBitmap(text);
                g.DrawImageUnscaled(textBitmap, 0, offsetY);
            }

            if (disposeGraphics)
            {
                g.Flush();
                g.Dispose();
            }
        }

        /// <summary>
        /// Clears all the back-buffer pixels.
        /// </summary>
        public void ClearPixels() => _bitBuffer.SetAll(false);

        /// <summary>
        /// Loads a bitmap into the bit buffer.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="brightnessThreshold">The brightness threshold 0 to 1.</param>
        /// <param name="offsetX">The offset x.</param>
        /// <param name="offsetY">The offset y.</param>
        /// <exception cref="ArgumentNullException">bitmap.</exception>
        public void LoadBitmap(Bitmap bitmap, double brightnessThreshold, int offsetX, int offsetY)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));
            ClearPixels();

            // for (var bitmapY = offsetY; bitmapY < offsetY + Height; bitmapY++)
            Parallel.For(offsetY, offsetY + Height, (bitmapY) =>
            {
                Color currentPixel;
                for (var bitmapX = offsetX; bitmapX < offsetX + Width; bitmapX++)
                {
                    currentPixel = bitmap.GetPixel(bitmapX, bitmapY);
                    if (currentPixel == Color.Black) continue;
                    if ((Math.Max(Math.Max(currentPixel.R, currentPixel.G), currentPixel.B) / 255d) >= brightnessThreshold)
                        _bitBuffer[GetBitIndex(bitmapX - offsetX, bitmapY - offsetY)] = true;
                }
            });
        }

        /// <summary>
        /// Renders tthe contents of the buffer.
        /// </summary>
        public void Render()
        {
            SendCommand(Command.SetColumnAddressRange, 0, (byte)(Width - 1));
            SendCommand(Command.SetPageAddressRange, 0, (byte)(_bufferPageCount - 1));
            _bitBuffer.CopyTo(_byteBuffer, 1);
            Device.Write(_byteBuffer);
        }

        /// <summary>
        /// Renders the specified text lines.
        /// This is the fastest way to render console text
        /// but it does not allow image composition.
        /// </summary>
        /// <param name="lines">The text lines.</param>
        public void Render(params string[] lines)
        {
            var outputBytes = new byte[1 + (_bufferPageCount * Width)];
            outputBytes[0] = 0x40; // The first byte signals data

            Parallel.For(0, lines.Length, (lineIndex) =>
            {
                if (lineIndex >= _bufferPageCount)
                    return;

                var chars = Encoding.ASCII.GetBytes(lines[lineIndex]);
                var currentChar = chars[0];
                var sourceX = 0;
                var targetB = 0;

                for (var charIndex = 0; charIndex < chars.Length; charIndex++)
                {
                    if ((charIndex + 1) * _fontBitmapCharWidth > Width)
                        break;

                    currentChar = chars[charIndex];
                    sourceX = currentChar * _fontBitmapCharWidth;
                    targetB = 1 + (lineIndex * Width) + (charIndex * _fontBitmapCharWidth);
                    Buffer.BlockCopy(FontBytemap, sourceX, outputBytes, targetB, _fontBitmapCharWidth);
                }
            });

            SendCommand(Command.SetColumnAddressRange, 0, (byte)(Width - 1));
            SendCommand(Command.SetPageAddressRange, 0, (byte)(_bufferPageCount - 1));
            Device.Write(outputBytes);
        }

        /// <inheritdoc />
        public void Dispose() => Dispose(true);

        #region Private Methods

        /// <summary>
        /// Gets the default I2C device.
        /// </summary>
        /// <returns>The default I2C device.</returns>
        private static II2CDevice GetDefaultDevice() => Pi.I2C.GetDeviceById(0x3c);

        /// <summary>
        /// Initializes the dipslay according to its model.
        /// </summary>
        private void Initialize()
        {
            SendCommand(Command.TurnOff);
            SendCommand(Command.SetClockDivider, DisplayClockDivider[Model]);
            SendCommand(Command.SetMultiplexer, MultiplexSetting[Model]);
            SendCommand(Command.SetDisplayOffset, 0x00);
            SendCommand(Command.SetStartLine, 0x00);
            SendCommand(Command.SetChargePumpMode, (byte)(VccSource == VccSourceMode.External ? 0x10 : 0x14));
            SendCommand(Command.SetMemoryMode, 0x00);
            SendCommand(Command.SegmentRemapModeOn);
            SendCommand(Command.ScanDirectionModeDecrement);
            SendCommand(Command.SetComPins, ComPins[Model]);

            _mContrast = Model == DisplayModel.Display128X64
                ? (byte) (VccSource == VccSourceMode.External ? 0x9F : 0xCF)
                : (byte) 0x8F;

            SendCommand(Command.SetContrast, _mContrast);
            SendCommand(Command.SetPrechargeMode, (byte)(VccSource == VccSourceMode.External ? 0x22 : 0xF1));
            SendCommand(Command.SetVoltageOutput, 0x40);
            SendCommand(Command.Resume);
            SendCommand(Command.DisplayModeNormal);
        }

        /// <summary>
        /// Gets bitarray index of the bitt buffer based on x and y coordinates.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The bit index for the given coordinates.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetBitIndex(int x, int y) =>
            ((y / 8) * _bitsPerPage) + (x * 8) + (y % 8);

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="argument">The argument.</param>
        private void SendCommand(Command command, byte argument)
        {
            SendCommand(command);
            SendCommand(argument);
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        private void SendCommand(Command command, byte arg0, byte arg1)
        {
            SendCommand(command);
            SendCommand(arg0);
            SendCommand(arg1);
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        private void SendCommand(byte command)
        {
            Device.Write(new byte[] { 0x00, command });
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        private void SendCommand(Command command) =>
            SendCommand((byte)command);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="alsoManaged"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool alsoManaged)
        {
            if (_isDisposed) return;
            _isDisposed = true;

            if (alsoManaged)
            {
                IsActive = false;
            }
        }

        #endregion
    }
}
