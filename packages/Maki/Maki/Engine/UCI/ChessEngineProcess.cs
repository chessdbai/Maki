//-----------------------------------------------------------------------
// <copyright file="ChessEngineProcess.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Engine.UCI
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// A low-level class for interacting with a running chess engine process.
    /// </summary>
    public class ChessEngineProcess : IDisposable
    {
        private readonly Process process;
        private readonly StringBuilder outputBuffer;
        private readonly StringBuilder errorBuffer;

        private volatile bool isWaiting = false;
        private string waitingFor = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChessEngineProcess"/> class.
        /// </summary>
        /// <param name="engineFile">The file location of the engine.</param>
        /// <param name="additionalArgs">Any optional command line arguments to pass when starting the engine.</param>
        public ChessEngineProcess(string engineFile, string additionalArgs = null)
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = engineFile,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                Arguments = additionalArgs!,
                UseShellExecute = false,
            };
            this.process = Process.Start(startInfo);
            this.process!.OutputDataReceived += this.ProcessOnOutputDataReceived;
            this.process.ErrorDataReceived += this.ProcessOnErrorDataReceived;
            this.process.BeginOutputReadLine();
            this.outputBuffer = new StringBuilder();
            this.errorBuffer = new StringBuilder();
        }

        /// <summary>
        /// Gets or sets the action to take whenever data is received from the engine process
        /// on StandardOutput.
        /// </summary>
        public Action<string> OutputReceivedAction { get; set; }

        /// <summary>
        /// Gets or sets the action to take whenever data is received from the engine process
        /// on StandardError.
        /// </summary>
        public Action<string> ErrorReceivedAction { get; set; }

        /// <summary>
        /// Writes a command to the engine via StandardInput.
        /// </summary>
        /// <param name="line">The UCI command to write.</param>
        public void WriteCommand(string line)
        {
            this.process.StandardInput.WriteLine(line);
            this.process.StandardInput.Flush();
        }

        /// <summary>
        /// Peek at the output buffer without popping it.
        /// </summary>
        /// <returns>The output buffer.</returns>
        public string PeekOutputBuffer()
        {
            return this.outputBuffer.ToString();
        }

        /// <summary>
        /// Return and clear the output buffer.
        /// </summary>
        /// <returns>The data on the output buffer.</returns>
        public string PopOutputBuffer()
        {
            lock (this.outputBuffer)
            {
                try
                {
                    string data = this.outputBuffer.ToString();
                    this.outputBuffer.Clear();
                    return data;
                }
                catch (ArgumentOutOfRangeException)
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Peek at the error buffer without popping it.
        /// </summary>
        /// <returns>The data in the error buffer.</returns>
        public string PeekErrorBuffer()
        {
            return this.errorBuffer.ToString();
        }

        /// <summary>
        /// Return and clear the error buffer.
        /// </summary>
        /// <returns>The data in the error buffer.</returns>
        public string PopErrorBuffer()
        {
            lock (this.errorBuffer)
            {
                string data = this.errorBuffer.ToString();
                this.errorBuffer.Clear();
                return data;
            }
        }

        /// <summary>
        /// Throw an exception if the error buffer is not empty.
        /// </summary>
        /// <exception cref="UciCommandErrorException">The UCI exception thrown if the error buffer is not empty.</exception>
        public void ThrowOnNonEmptyErrorBuffer()
        {
            string errorText = this.PopErrorBuffer();
            if (errorText.Length > 0)
            {
                throw new UciCommandErrorException(errorText);
            }
        }

        /// <summary>
        /// Blocks the thread until the given text appears in the error buffer.
        /// </summary>
        /// <param name="text">The text to halt on.</param>
        /// <param name="timeoutMs">The maximum time to wait.</param>
        /// <exception cref="UciProcessTimeoutException">Thrown when the timeout is reached without seeing the text.</exception>
        public void WaitForText(string text, int timeoutMs)
        {
            this.waitingFor = text;
            this.isWaiting = true;

            var startTime = DateTime.Now;
            while (this.isWaiting && (DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
            {
                Thread.Sleep(10);
                this.ThrowOnNonEmptyErrorBuffer();
            }

            if (this.isWaiting)
            {
                if (!this.PeekOutputBuffer().Contains(text))
                {
                    throw new UciProcessTimeoutException($"The provided wait text '{text}' was not seen in " +
                                                         $"the process output or error string before the given {timeoutMs}ms timeout period.");
                }
            }
        }

        /// <summary>
        /// Dispose this object.
        /// </summary>
        public void Dispose()
        {
            this.process.Dispose();
        }

        private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e?.Data == null)
            {
                return;
            }

            lock (this.errorBuffer)
            {
                if (this.errorBuffer.Length != 0)
                {
                    this.errorBuffer.Append("\n");
                }

                this.errorBuffer.AppendLine(e.Data);

                if (this.waitingFor != null && this.isWaiting && this.errorBuffer.ToString().Contains(this.waitingFor.ToLower()))
                {
                    this.isWaiting = false;
                }
            }

            try
            {
                this.ErrorReceivedAction?.Invoke(e.Data);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Failed to invoke output received action for message: '{e.Data}'");
                Console.WriteLine($"Due to an exception:\n{exception}");
            }
        }

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e?.Data == null)
            {
                return;
            }

            lock (this.outputBuffer)
            {
                if (this.outputBuffer.Length != 0)
                {
                    this.outputBuffer.Append("\n");
                }

                this.outputBuffer.Append(e.Data);

                if (this.waitingFor != null && this.isWaiting && this.outputBuffer.ToString().ToLower().Contains(this.waitingFor.ToLower()))
                {
                    this.isWaiting = false;
                }
            }

            try
            {
                this.OutputReceivedAction?.Invoke(e?.Data);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Failed to invoke output received action for message: '{e.Data}'");
                Console.WriteLine($"Due to an exception:\n{exception}");
            }
        }
    }
}