namespace Unosquare.RaspberryIO
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// The Raspberry Pi's camera controller wrapping RaspiStill and RaspiVid programs.
    /// This class is a singleton
    /// </summary>
    public class CameraController
    {
        private static CameraController m_Instance = null;

        private static readonly ManualResetEventSlim OperationDone = new ManualResetEventSlim(true);
        private static readonly object SyncLock = new object();
        private const string RaspiVideo = "raspivid";

        #region Properties

        /// <summary>
        /// Gets the instance of the Pi's camera controller.
        /// </summary>
        static internal CameraController Instance
        {
            get
            {
                lock (SyncLock)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = new CameraController();
                    }

                    return m_Instance;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the camera is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy { get { return OperationDone.IsSet == false; } }

        #endregion

        #region Helpers

        static private async Task<int> CopyStandardOutputAsync(Process process, Stream outputStream, CancellationToken ct)
        {
            var swapBuffer = new byte[2048];
            var readCount = -1;
            var totalCount = 0;

            process.StandardOutput.DiscardBufferedData();
            while (ct.IsCancellationRequested == false)
            {
                readCount = await process.StandardOutput.BaseStream.ReadAsync(swapBuffer, 0, swapBuffer.Length, ct);
                if (readCount <= 0) break;
                await outputStream.WriteAsync(swapBuffer, 0, readCount);
            }

            return totalCount;
        }

        #endregion

        #region Image Capture Methods

        public async Task<byte[]> CapturePictureAsync(PictureArguments arguments, CancellationToken ct)
        {
            if (Instance.IsBusy)
                throw new InvalidOperationException("Cannot use camera module because it is currently busy.");

            try
            {
                OperationDone.Reset();
                var process = arguments.CreateProcess();
                if (process.Start() == false)
                    return new byte[] { };

                var outputStream = new MemoryStream();
                await CopyStandardOutputAsync(process, outputStream, ct);

                process.WaitForExit();
                return outputStream.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                OperationDone.Set();
            }
        }

        public async Task<byte[]> CaptureImageAsync(PictureArguments arguments)
        {
            var cts = new CancellationTokenSource();
            return await CapturePictureAsync(arguments, cts.Token);
        }

        public byte[] CaptureImage(PictureArguments arguments)
        {
            return CaptureImageAsync(arguments).GetAwaiter().GetResult();
        }

        public async Task<byte[]> CaptureJpegAsync(int width, int height, CancellationToken ct)
        {
            var arguments = new PictureArguments()
            {
                
                ImageWidth = width,
                ImageHeight = height,
                CaptureJpegQuality = 90,
                CaptureDisplayPreview = true,
                CaptureTimeoutMilliseconds = 2000,
                CaptureExposure = CameraExposureMode.Night,
                CaptureEncoding = CameraImageEncodingFormat.Jpg,
                ImageAnnotationsText = "Hello, this is some nice text",
                ImageAnnotations = CameraAnnotation.Date | CameraAnnotation.Time,
                ImageAnnotationFontSize = 30,
                ImageAnnotationFontColor = Color.Red,
            };

            return await CapturePictureAsync(arguments, ct);
        }

        public byte[] CaptureJpeg(int width, int height)
        {
            var cts = new CancellationTokenSource();
            return CaptureJpegAsync(width, height, cts.Token).GetAwaiter().GetResult();
        }

        #endregion
    }
}
