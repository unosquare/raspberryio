using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Unosquare.RaspberryIO
{
    /// <summary>
    /// Provides methods to help create external processes, capture the
    /// standard error and standard input streams.
    /// 
    /// This class belongs to SWAN project.
    /// </summary>
    internal static class ProcessHelper
    {
        /// <summary>
        /// Defines a delegate to handle binary data reception from the standard 
        /// output or standard error streams from a process
        /// </summary>
        /// <param name="processData">The process data.</param>
        /// <param name="process">The process.</param>
        public delegate void ProcessDataReceivedCallback(byte[] processData, Process process);

        /// <summary>
        /// Copies the stream asynchronously.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="baseStream">The source stream.</param>
        /// <param name="onDataCallback">The on data callback.</param>
        /// <param name="syncEvents">if set to <c>true</c> [synchronize events].</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        private static async Task<ulong> CopyStreamAsync(Process process, Stream baseStream, ProcessDataReceivedCallback onDataCallback,
            bool syncEvents, CancellationToken ct)
        {
            return await Task.Factory.StartNew(async () =>
            {
                // define some state variables
                var swapBuffer = new byte[2048]; // the buffer to copy data from one stream to the next
                var readCount = -1; // the bytes read in any given event
                ulong totalCount = 0; // the total amount of bytes read
                var hasExited = false;

                while (ct.IsCancellationRequested == false)
                {
                    try
                    {
                        // Check if process is no longer valid
                        // if this condition holds, simply read the last bits of data available.
                        if (process.HasExited || process.WaitForExit(1))
                        {
                            while (true)
                            {
                                try
                                {
                                    readCount = await baseStream.ReadAsync(swapBuffer, 0, swapBuffer.Length, ct);
                                    if (readCount > 0)
                                    {
                                        totalCount += (ulong)readCount;
                                        onDataCallback?.Invoke(swapBuffer, process);
                                    }
                                    else
                                    {
                                        hasExited = true;
                                        break;
                                    }
                                }
                                catch
                                {
                                    hasExited = true;
                                    break;
                                }
                            }
                        }

                        if (hasExited) break;

                        // Try reading from the stream. < 0 means no read occurred.
                        readCount = await baseStream.ReadAsync(swapBuffer, 0, swapBuffer.Length, ct);

                        // When no read is done, we need to let is rest for a bit
                        if (readCount <= 0)
                        {
                            await Task.Delay(1, ct); // do not hog CPU cycles doing nothing.
                            continue;
                        }

                        totalCount += (ulong)readCount;
                        if (onDataCallback != null)
                        {
                            // Create the buffer to pass to the callback
                            var eventBuffer = swapBuffer.Skip(0).Take(readCount).ToArray();

                            // Create the data processing callback invocation
                            var eventTask = Task.Factory.StartNew(() => { onDataCallback.Invoke(eventBuffer, process); }, ct);

                            // wait for the event to process before the next read occurs
                            if (syncEvents) eventTask.Wait(ct);
                        }
                    }
                    catch
                    {
                        break;
                    }
                }

                return totalCount;
            }).Unwrap();
        }

        /// <summary>
        /// Runs the process asynchronously and if the exit code is 0,
        /// returns all of the standard output text. If the exit code is something other than 0
        /// it returns the contents of standard error.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<string> GetProcessOutputAsync(string filename, string arguments = "", CancellationToken ct = default(CancellationToken))
        {
            var standardOutputBuilder = new StringBuilder();
            var standardErrorBuilder = new StringBuilder();
            var defaultEncoding = Encoding.GetEncoding(0);

            var processReturn = await RunProcessAsync(filename, arguments,
                (data, proc) => { standardOutputBuilder.Append(defaultEncoding.GetString(data)); },
                (data, proc) => { standardErrorBuilder.Append(defaultEncoding.GetString(data)); }, true, ct);

            return processReturn != 0 ? standardErrorBuilder.ToString() : standardOutputBuilder.ToString();
        }

        /// <summary>
        /// Runs an external process asynchronously.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="onOutputData">The on output data.</param>
        /// <param name="onErrorData">The on error data.</param>
        /// <param name="syncEvents">if set to <c>true</c> the next data callback will wait until the current one completes.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        public static async Task<int> RunProcessAsync(string filename, string arguments, ProcessDataReceivedCallback onOutputData, ProcessDataReceivedCallback onErrorData, bool syncEvents, CancellationToken ct)
        {
            var task = Task.Factory.StartNew(() =>
            {
                // Setup the process and its corresponding start info
                var process = new Process
                {
                    EnableRaisingEvents = false,
                    StartInfo = new ProcessStartInfo
                    {
                        Arguments = arguments,
                        CreateNoWindow = true,
                        FileName = filename,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    }
                };

                // Launch the process and discard any buffered data for standard error and standard output
                process.Start();
                process.StandardError.DiscardBufferedData();
                process.StandardOutput.DiscardBufferedData();

                // Launch the asynchronous stream reading tasks
                var readTasks = new Task[2];
                readTasks[0] = CopyStreamAsync(process, process.StandardOutput.BaseStream, onOutputData, syncEvents, ct);
                readTasks[1] = CopyStreamAsync(process, process.StandardError.BaseStream, onErrorData, syncEvents, ct);

                try
                {
                    // Wait for all tasks to complete
                    Task.WaitAll(readTasks, ct);
                }
                catch (TaskCanceledException)
                {
                    // ignore
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    // Wait for the process to exit
                    while (ct.IsCancellationRequested == false)
                    {
                        if (process.HasExited)
                            break;

                        if (process.WaitForExit(5))
                            break;
                    }

                    // Forcefully kill the process if it do not exit
                    try
                    {
                        if (process.HasExited == false)
                            process.Kill();
                    }
                    catch
                    {
                        // swallow
                    }
                }

                try
                {
                    // Retrieve and return the exit code.
                    // -1 signals error
                    if (process.HasExited)
                        return process.ExitCode;
                    else
                        return -1;
                }
                catch
                {
                    return -1;
                }
            });

            return await task;
        }
    }
}
