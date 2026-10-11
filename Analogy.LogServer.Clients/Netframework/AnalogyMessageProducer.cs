#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable MA0046 // Use EventHandler<T> to declare events
#if !NET
using Analogy.Interfaces;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
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
        public event EventHandler<string> OnError;
        private static readonly string ProcessName = Process.GetCurrentProcess().ProcessName;
        private Channel channel;
        private AsyncClientStreamingCall<AnalogyGRPCLogMessage, AnalogyMessageReply> stream;
        private bool connected = true;
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        static AnalogyMessageProducer()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", isEnabled: true);
        }

        public AnalogyMessageProducer(string address)
        {
            try
            {
                channel = new Channel(address, ChannelCredentials.Insecure);
                var client = new Analogy.AnalogyClient(channel);
                stream = client.SubscribeForPublishingMessages();
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, $"Error creating gRPC Connection: {e.Message}");
            }
        }

        public async Task Log(string text, string source, Interfaces.DataTypes.AnalogyLogLevel level, string category = "",
            string machineName = "", string userName = "", string processName = "", int processId = 0, int threadId = 0, Dictionary<string, string>? additionalInformation = null, [CallerMemberName] string memberName = "", [CallerLineNumber] long lineNumber = 0, [CallerFilePath] string filePath = "")
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
                Date = Timestamp.FromDateTime(DateTime.UtcNow),
                FileName = filePath,
                Id = Guid.NewGuid().ToString(),
                Level = GetLogLevel(level),
                LineNumber = lineNumber,
                MachineName = machineName ?? Environment.MachineName,
                MethodName = memberName,
                Module = processName ?? ProcessName,
                ProcessId = processId,
                ThreadId = threadId is not 0 ? threadId : Thread.CurrentThread.ManagedThreadId,
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
                OnError?.Invoke(this, $"Error sending message to gRPC Server: {e.Message}");
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private AnalogyGRPCLogLevel GetLogLevel(Interfaces.DataTypes.AnalogyLogLevel level)
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
                OnError?.Invoke(this, $"Error closing  gRPC connection to Server: {e.Message}");
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
        public void Dispose()
        {
            try
            {
                semaphoreSlim.Dispose();
                _ = channel?.ShutdownAsync();
                stream?.Dispose();
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, $"Error during dispose: {e.Message}");
            }
        }

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
                OnError?.Invoke(this, $"Error during dispose: {e.Message}");
            }
        }
    }
}
#endif