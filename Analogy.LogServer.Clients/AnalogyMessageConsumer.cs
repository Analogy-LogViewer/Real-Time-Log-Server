using Analogy.Interfaces;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enum = System.Enum;

namespace Analogy.LogServer.Clients
{
    public class AnalogyMessageConsumer : IDisposable
    {
        private static Analogy.AnalogyClient client { get; set; }
        private readonly AsyncServerStreamingCall<AnalogyLogMessage> _stream;
        private CancellationTokenSource _cts;
        private GrpcChannel channel;
        static AnalogyMessageConsumer()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        public AnalogyMessageConsumer(string address)
        {
            //using var channel = GrpcChannel.ForAddress("http://localhost:6000");
            channel = GrpcChannel.ForAddress(address);
            client = new Analogy.AnalogyClient(channel);
            AnalogyConsumerMessage m = new AnalogyConsumerMessage { Message = "client" };
            _stream = client.SubscribeForConsumeMessages(m);
        }

        public async IAsyncEnumerable<Interfaces.AnalogyLogMessage> GetMessages()
        {
            _cts = new CancellationTokenSource();
            await foreach (var m in _stream.ResponseStream.ReadAllAsync())
            {
                var token = _cts.Token;
                Interfaces.AnalogyLogMessage msg = new Interfaces.AnalogyLogMessage()
                {
                    Id = Guid.Parse((ReadOnlySpan<char>)m.Id),
                    Category = m.Category,
                    Date = m.Date.ToDateTime().ToLocalTime(),
                    FileName = m.FileName,
                    LineNumber = m.LineNumber,
                    MachineName = m.MachineName,
                    MethodName = m.MethodName,
                    Module = m.Module,
                    ProcessId = m.ProcessId,
                    Source = m.Source,
                    Text = m.Text,
                    ThreadId = m.ThreadId,
                    User = m.User
                };
                if (Enum.TryParse(m.Level, out AnalogyLogLevel all))
                {
                    msg.Level = all;
                }
                else
                {
                    msg.Level = AnalogyLogLevel.Unknown;
                    m.Text += $" [Unknown log level: {m.Level}]";
                }
                if (Enum.TryParse(m.Class, out AnalogyLogClass alc))
                {
                    msg.Class = alc;
                }
                else
                {
                    msg.Class = AnalogyLogClass.General;
                    m.Text += $" [Unknown log class: {m.Class}]";
                }
                yield return msg;
                if (token.IsCancellationRequested)
                    yield break;

            }
        }
        public Task Stop()
        {
            _cts?.Cancel();
            return channel.ShutdownAsync();

        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _stream?.Dispose();
            channel?.Dispose();
        }
    }
}
