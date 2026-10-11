#if NETCOREAPP3_1 || NET
using Analogy.Interfaces;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Analogy.LogServer.Clients
{
    public class AnalogyMessageConsumer : IDisposable
    {
        private static Analogy.AnalogyClient Client { get; set; } = null!;
        private readonly AsyncServerStreamingCall<AnalogyGRPCLogMessage> stream;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private GrpcChannel channel;
        static AnalogyMessageConsumer()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", isEnabled: true);
        }

        public AnalogyMessageConsumer(string address)
        {
            channel = GrpcChannel.ForAddress(address);
            Client = new Analogy.AnalogyClient(channel);
            AnalogyConsumerMessage m = new AnalogyConsumerMessage { Message = "client" };
            stream = Client.SubscribeForConsumingMessages(m);
        }

        public async IAsyncEnumerable<Interfaces.DataTypes.AnalogyLogMessage> GetMessages()
        {
            cts = new CancellationTokenSource();
            await foreach (var m in stream.ResponseStream.ReadAllAsync())
            {
                var token = cts.Token;
                Interfaces.DataTypes.AnalogyLogMessage msg = new Interfaces.DataTypes.AnalogyLogMessage()
                {
                    Level = (Interfaces.DataTypes.AnalogyLogLevel)m.Level,
                    Class = (Interfaces.DataTypes.AnalogyLogClass)m.Class,
                    Date = m.Date.ToDateTimeOffset(),
                    FileName = m.FileName,
                    LineNumber = m.LineNumber,
                    MachineName = m.MachineName,
                    MethodName = m.MethodName,
                    Module = m.Module,
                    ProcessId = m.ProcessId,
                    Source = m.Source,
                    Text = m.Text,
                    ThreadId = m.ThreadId,
                    User = m.User,
                };
                if (!string.IsNullOrEmpty(m.Category))
                {
                    msg.AddOrReplaceAdditionalProperty("Category", m.Category, StringComparer.Ordinal);
                }
                msg.Id = string.IsNullOrEmpty(m.Id)
                    ? Guid.NewGuid()
                    : Guid.TryParse(m.Id, out Guid id) ? id : Guid.NewGuid();
                yield return msg;
                if (token.IsCancellationRequested)
                {
                    yield break;
                }
            }
        }
        public async Task Stop()
        {
            if (cts is not null)
            {
                await cts.CancelAsync();
            }
            await channel.ShutdownAsync();
        }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        {
            cts?.Cancel();
            cts?.Dispose();
            stream?.Dispose();
            channel?.Dispose();
        }
    }
}
#endif