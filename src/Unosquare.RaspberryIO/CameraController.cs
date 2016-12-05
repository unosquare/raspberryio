namespace Unosquare.RaspberryIO
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The Raspberry Pi's camera controller wrapping raspistill and raspivid programs.
    /// This class is a singleton
    /// </summary>
    public class CameraController
    {
        #region Private Declarations

        private static CameraController m_Instance = null;
        private static readonly ManualResetEventSlim OperationDone = new ManualResetEventSlim(true);
        private static readonly object SyncLock = new object();
        private static Thread VideoWorker = null;
        private static Process VideoProcess = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance of the Pi's camera controller.
        /// </summary>
        internal static CameraController Instance
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
        /// Gets a value indicating whether the camera module is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy => OperationDone.IsSet == false;

        #endregion

        #region Helpers

        /// <summary>
        /// Copies the standard output to another stream asynchronously.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        private static async Task<int> CopyStandardOutputAsync(Process process, Stream outputStream, CancellationToken ct)
        {
            var swapBuffer = new byte[2048];
            var readCount = -1;
            var totalCount = 0;

            process.StandardOutput.DiscardBufferedData();
            while (ct.IsCancellationRequested == false)
            {
                readCount = await process.StandardOutput.BaseStream.ReadAsync(swapBuffer, 0, swapBuffer.Length, ct);
                if (readCount <= 0) break;
                await outputStream.WriteAsync(swapBuffer, 0, readCount, ct);
            }

            return totalCount;
        }

        #endregion

        #region Image Capture Methods

        /// <summary>
        /// Captures an image asynchronously.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Cannot use camera module because it is currently busy.</exception>
        public async Task<byte[]> CaptureImageAsync(CameraStillSettings settings, CancellationToken ct)
        {
            if (Instance.IsBusy)
                throw new InvalidOperationException("Cannot use camera module because it is currently busy.");

            if (settings.CaptureTimeoutMilliseconds <= 0)
                throw new ArgumentException($"{nameof(settings.CaptureTimeoutMilliseconds)} needs to be greater than 0");

            try
            {
                OperationDone.Reset();
                var process = settings.CreateProcess();
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

        /// <summary>
        /// Captures an image asynchronously.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public async Task<byte[]> CaptureImageAsync(CameraStillSettings settings)
        {
            var cts = new CancellationTokenSource();
            return await CaptureImageAsync(settings, cts.Token);
        }

        /// <summary>
        /// Captures an image.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public byte[] CaptureImage(CameraStillSettings settings)
        {
            return CaptureImageAsync(settings).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Captures a JPEG encoded image asynchronously at 90% quality.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        public async Task<byte[]> CaptureImageJpegAsync(int width, int height, CancellationToken ct)
        {
            var settings = new CameraStillSettings()
            {
                CaptureWidth = width,
                CaptureHeight = height,
                CaptureJpegQuality = 90,
                CaptureTimeoutMilliseconds = 300,
            };

            return await CaptureImageAsync(settings, ct);
        }

        /// <summary>
        /// Captures a JPEG encoded image at 90% quality.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public byte[] CaptureImageJpeg(int width, int height)
        {
            var cts = new CancellationTokenSource();
            return CaptureImageJpegAsync(width, height, cts.Token).GetAwaiter().GetResult();
        }

        #endregion

        #region Video Capture Methods

        /// <summary>
        /// Performs a continous read of the standard output and fires the corresponding events.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="onDataCallback">The on data callback.</param>
        /// <param name="onExitCallback">The on exit callback.</param>
        private static void VideoWorkerDoWork(CameraVideoSettings settings, Action<byte[]> onDataCallback, Action onExitCallback)
        {

            var readBuffer = new byte[1024 * 8];
            var readCount = 0;
            var totalRead = 0;
            var lastReceived = DateTime.UtcNow;
            var timeout = TimeSpan.FromMilliseconds(1000);

            VideoProcess = settings.CreateProcess();
            VideoProcess.Exited += (s, e) => { onExitCallback?.Invoke(); };

            try
            {
                VideoProcess.Start();
                VideoProcess.StandardOutput.DiscardBufferedData();

                while (true)
                {
                    if (DateTime.UtcNow.Subtract(lastReceived) > timeout) break;

                    try
                    {
                        readCount = VideoProcess.StandardOutput.BaseStream.Read(readBuffer, 0, readBuffer.Length);
                        totalRead += readCount;
                        if (readCount > 0)
                        {
                            lastReceived = DateTime.UtcNow;
                            onDataCallback?.Invoke(readBuffer.Skip(0).Take(readCount).ToArray());
                        }
                    }
                    catch
                    {
                        // Stop reading if an error occurs
                        break;
                    }

                }
            }
            catch
            {
                // swallow
            }
            finally
            {
                Instance.CloseVideoStream();
                OperationDone.Set();
            }

        }

        /// <summary>
        /// Opens the video stream with a timeout of 0 (running indefinitely) at 1080p resolution, variable bitrate and 25 FPS.
        /// No preview is shown
        /// </summary>
        /// <param name="onDataCallback">The on data callback.</param>
        public void OpenVideoStream(Action<byte[]> onDataCallback)
        {
            OpenVideoStream(onDataCallback, null);
        }

        /// <summary>
        /// Opens the video stream with a timeout of 0 (running indefinitely) at 1080p resolution, variable bitrate and 25 FPS.
        /// No preview is shown
        /// </summary>
        /// <param name="onDataCallback">The on data callback.</param>
        /// <param name="onExitCallback">The on exit callback.</param>
        public void OpenVideoStream(Action<byte[]> onDataCallback, Action onExitCallback)
        {
            var settings = new CameraVideoSettings()
            {
                CaptureTimeoutMilliseconds = 0,
                CaptureDisplayPreview = false,
                CaptureWidth = 1920,
                CaptureHeight = 1080
            };

            OpenVideoStream(settings, onDataCallback, onExitCallback);
        }

        /// <summary>
        /// Opens the video stream with the supplied settings. Capture Timeout Milliseconds has to be 0 or greater
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="onDataCallback">The on data callback.</param>
        /// <param name="onExitCallback">The on exit callback.</param>
        /// <exception cref="System.InvalidOperationException">Cannot use camera module because it is currently busy.</exception>
        /// <exception cref="System.ArgumentException">CaptureTimeoutMilliseconds</exception>
        public void OpenVideoStream(CameraVideoSettings settings, Action<byte[]> onDataCallback, Action onExitCallback)
        {
            if (Instance.IsBusy)
                throw new InvalidOperationException("Cannot use camera module because it is currently busy.");

            if (settings.CaptureTimeoutMilliseconds < 0)
                throw new ArgumentException($"{nameof(settings.CaptureTimeoutMilliseconds)} needs to be greater than or equal to 0");

            try
            {
                OperationDone.Reset();
                VideoWorker = new Thread(() => { VideoWorkerDoWork(settings, onDataCallback, onExitCallback); });
                VideoWorker.IsBackground = true;
                VideoWorker.Start();
            }
            catch (Exception ex)
            {
                OperationDone.Set();
                throw ex;
            }

        }

        /// <summary>
        /// Closes the video stream of a video stream is open.
        /// </summary>
        public void CloseVideoStream()
        {
            lock (SyncLock)
            {
                if (IsBusy == false || VideoProcess == null || VideoWorker == null)
                    return;

                if (VideoProcess.HasExited == false)
                    VideoProcess.Kill();

                VideoWorker = null;
                VideoProcess = null;
            }

        }

        #endregion
    }
}