using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCP
{
    /// <summary>Transport using standard input/output for local process connections.</summary>
    public class StdioTransport : IMcpTransport
    {
        private readonly Stream _input;
        private readonly Stream _output;
        private readonly Encoding _encoding;
        private CancellationTokenSource? _cts;
        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1, 1);

        public StdioTransport(Stream? input = null, Stream? output = null, Encoding? encoding = null)
        {
            _input = input ?? Console.OpenStandardInput();
            _output = output ?? Console.OpenStandardOutput();
            _encoding = encoding ?? new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        }

        public Task StartAsync(Func<string, Task> onMessageReceived)
        {
            _cts = new CancellationTokenSource();
            var ct = _cts.Token;
            return Task.Run(async () =>
            {
                byte[] buffer = new byte[4096];
                List<byte> leftover = new List<byte>();
                try
                {
                    while (!ct.IsCancellationRequested)
                    {
                        int bytesRead = await _input.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead <= 0) break;
                        
                        // Process the buffer without using spans across async boundaries
                        int start = 0;
                        for (int i = 0; i < bytesRead; i++)
                        {
                            if (buffer[i] == (byte)'\n')
                            {
                                // Found end of message
                                int lineLength = i - start;
                                // Check if we need to trim CR
                                if (lineLength > 0 && buffer[i - 1] == (byte)'\r')
                                {
                                    lineLength--;
                                }
                                
                                byte[] messageBytes;
                                if (leftover.Count > 0)
                                {
                                    // Copy current line segment
                                    byte[] currentSegment = new byte[lineLength];
                                    Array.Copy(buffer, start, currentSegment, 0, lineLength);
                                    
                                    // Prepend any leftover bytes from previous chunk
                                    leftover.AddRange(currentSegment);
                                    messageBytes = leftover.ToArray();
                                    leftover.Clear();
                                }
                                else
                                {
                                    // No leftover, just copy the current segment
                                    messageBytes = new byte[lineLength];
                                    Array.Copy(buffer, start, messageBytes, 0, lineLength);
                                }
                                
                                string message = _encoding.GetString(messageBytes);
                                if (message.Length > 0)
                                {
                                    await onMessageReceived(message);
                                }
                                start = i + 1;
                            }
                        }
                        
                        if (start < bytesRead)
                        {
                            // Store leftover bytes for next iteration (partial message not yet terminated)
                            int remainingLength = bytesRead - start;
                            byte[] remaining = new byte[remainingLength];
                            Array.Copy(buffer, start, remaining, 0, remainingLength);
                            leftover.AddRange(remaining);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"StdioTransport error: {ex.Message}");
                    // Reading loop ends on any error or cancellation.
                }
            }, ct);
        }

        public async Task SendAsync(string message)
        {
            byte[] data = _encoding.GetBytes(message + "\n");
            await _writeLock.WaitAsync();
            try
            {
                await _output.WriteAsync(data, 0, data.Length);
                await _output.FlushAsync();
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public Task StopAsync()
        {
            _cts?.Cancel();
            return Task.CompletedTask;
        }
    }
}