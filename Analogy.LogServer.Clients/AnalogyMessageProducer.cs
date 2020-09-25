using Analogy.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.Collections;

namespace Analogy.LogServer.Clients
{
    public class AnalogyMessageProducer : IDisposable
    {
        private static readonly int ProcessId = Process.GetCurrentProcess().Id;
        private static readonly string ProcessName = Process.GetCurrentProcess().ProcessName;
        private static Analogy.AnalogyClient client { get; set; }
        private GrpcChannel channel;
        private AsyncClientStreamingCall<AnalogyLogMessage, AnalogyMessageReply> stream;
        private ILogger _logger;
        private bool connected = true;
        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        static AnalogyMessageProducer()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        public AnalogyMessageProducer(string address, ILogger logger)
        {
            _logger = logger;
            try
            {
                // channel = GrpcChannel.ForAddress("http://localhost:6000");
                channel = GrpcChannel.ForAddress(address);
                client = new Analogy.AnalogyClient(channel);
                stream = client.SubscribeForSendMessages();
            }
            catch (Exception e)
            {
                logger?.LogError(e, "Error creating gRPC Connection");
            }

        }

        public async Task Log(string text, string source, AnalogyLogLevel level, string category = "",
            string machineName = null, string userName = null, string processName = null, int processId = -1, int threadId = -1, Dictionary<string, string> additionalInformation = null, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {
            if (!connected) return;
            var m = new AnalogyLogMessage()
            {
                Text = text,
                Category = category,
                Class = AnalogyLogClass.General.ToString(),
                Date = Timestamp.FromDateTime(DateTime.UtcNow),
                FileName = filePath,
                Id = Guid.NewGuid().ToString(),
                Level = level.ToString(),
                LineNumber = lineNumber,
                MachineName = machineName ?? Environment.MachineName,
                MethodName = memberName,
                Module = processName ?? ProcessName,
                ProcessId = processId != -1 ? processId : ProcessId,
                ThreadId = threadId != -1 ? threadId : Thread.CurrentThread.ManagedThreadId,
                Source = source,
                User = userName ?? Environment.UserName,
            };
            if (additionalInformation != null)
                foreach (KeyValuePair<string, string> keyValuePair in additionalInformation)
                {
                    m.AdditionalInformation.Add(keyValuePair.Key, keyValuePair.Value);
                }

            try
            {
                await _semaphoreSlim.WaitAsync();
                await stream.RequestStream.WriteAsync(m);

            }
            catch (Exception e)
            {
                connected = false;
                _logger?.LogError(e, "Error sending message to gRPC Server");
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
        public void StopReceiving()
        {
            try
            {
                channel.Dispose();
                GrpcEnvironment.ShutdownChannelsAsync();
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "Error closing  gRPC connection to Server");

            }

        }

        public void Dispose()
        {
            try
            {
                _semaphoreSlim.Dispose();
                channel?.Dispose();
                stream?.Dispose();
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "Error during dispose");
            }
        }
    }
}
