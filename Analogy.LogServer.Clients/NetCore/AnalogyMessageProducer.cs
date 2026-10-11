#pragma warning disable MA0046 // Use EventHandler<T> to declare events
#if NET
using Analogy.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Analogy.LogServer.Clients
{
    public class AnalogyMessageProducer : IDisposable, IAsyncDisposable
    {
        public event EventHandler<string>? OnError;
#pragma warning restore MA0046 // Use EventHandler<T> to declare events
        private static readonly int ProcessId = Environment.ProcessId;
        private static readonly string ProcessName = Process.GetCurrentProcess().ProcessName;
        private GrpcChannel channel = null!;
        private AsyncClientStreamingCall<AnalogyGRPCLogMessage, AnalogyMessageReply> stream = null!;
        private bool connected = true;
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        static AnalogyMessageProducer()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", isEnabled: true);
        }

        /// <summary>
        /// Initializes a new instance.
        /// The address. for example: http://localhost:6000.
        /// </summary>
        public AnalogyMessageProducer(string address)
        {
            try
            {
                channel = GrpcChannel.ForAddress(address);
                var client = new Analogy.AnalogyClient(channel);
                stream = client.SubscribeForPublishingMessages();
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, $"Error creating gRPC Connection: {e}");
            }
        }

        public async Task Log(string text, string source, Interfaces.DataTypes.AnalogyLogLevel level,
            string category = "",
            string machineName = "", string userName = "", string processName = "", int processId = 0, int threadId = 0,
            Dictionary<string, string>? additionalInformation = null, [CallerMemberName] string memberName = "",
            [CallerLineNumber] long lineNumber = 0, [CallerFilePath] string filePath = "")
        {
            if (!connected)
            {
                return;
            }

            var m = new AnalogyGRPCLogMessage()
            {
                Text = text,
                Category = category,
                Class = AnalogyGRPCLogClass.General,
                Date = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow),
                FileName = filePath,
                Id = Guid.NewGuid().ToString(),
                Level = GetLogLevel(level),
                LineNumber = lineNumber,
                MachineName = machineName ?? Environment.MachineName,
                MethodName = memberName,
                Module = processName ?? ProcessName,
                ProcessId = processId is not 0 ? processId : ProcessId,
                ThreadId = threadId is not 0 ? threadId : Environment.CurrentManagedThreadId,
                Source = source,
                User = userName ?? Environment.UserName,
            };
            if (additionalInformation is not null)
            {
                foreach (KeyValuePair<string, string> keyValuePair in additionalInformation)
                {
                    m.AdditionalInformation.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            try
            {
                await semaphoreSlim.WaitAsync();
                await stream.RequestStream.WriteAsync(m);
            }
            catch (Exception e)
            {
                connected = false;
                OnError?.Invoke(this, $"Error sending message to gRPC Server: {e}");
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private static AnalogyGRPCLogLevel GetLogLevel(Interfaces.DataTypes.AnalogyLogLevel level)
        {
            return level switch
            {
                Interfaces.DataTypes.AnalogyLogLevel.Unknown => AnalogyGRPCLogLevel.Unknown,
                Interfaces.DataTypes.AnalogyLogLevel.Trace => AnalogyGRPCLogLevel.Trace,
                Interfaces.DataTypes.AnalogyLogLevel.Verbose => AnalogyGRPCLogLevel.Verbose,
                Interfaces.DataTypes.AnalogyLogLevel.Debug => AnalogyGRPCLogLevel.Debug,
                Interfaces.DataTypes.AnalogyLogLevel.Information => AnalogyGRPCLogLevel.Information,
                Interfaces.DataTypes.AnalogyLogLevel.Warning => AnalogyGRPCLogLevel.Warning,
                Interfaces.DataTypes.AnalogyLogLevel.Error => AnalogyGRPCLogLevel.Error,
                Interfaces.DataTypes.AnalogyLogLevel.Critical => AnalogyGRPCLogLevel.Critical,
                Interfaces.DataTypes.AnalogyLogLevel.Analogy => AnalogyGRPCLogLevel.Analogy,
                Interfaces.DataTypes.AnalogyLogLevel.None => AnalogyGRPCLogLevel.None,
                _ => AnalogyGRPCLogLevel.Unknown,
            };
        }

        public void StopReceiving()
        {
            try
            {
                channel.ShutdownAsync().Wait(5000);
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, $"Error closing  gRPC connection to Server: {e}");
            }
        }
        public async Task StopReceivingAsync()
        {
            try
            {
                await channel.ShutdownAsync();
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, $"Error closing  gRPC connection to Server: {e}");
            }
        }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        {
            try
            {
                semaphoreSlim.Dispose();
                channel?.Dispose();
                stream?.Dispose();
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, $"Error during dispose: {e}");
            }
        }
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public async ValueTask DisposeAsync()
        {
            try
            {
                semaphoreSlim.Dispose();
                if (channel is not null)
                {
                    await channel.ShutdownAsync();
                }

                stream?.Dispose();
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, $"Error during dispose: {e}");
            }
        }
    }
}
#endif