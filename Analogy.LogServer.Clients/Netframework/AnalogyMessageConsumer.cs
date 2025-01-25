#if !NETCOREAPP3_1 && !NET
using Analogy.Interfaces;
using Grpc.Core;
using Grpc.Core.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Analogy.LogServer.Clients
{
    public class AnalogyMessageConsumer : IDisposable
    {
        public event EventHandler<Interfaces.AnalogyLogMessage> OnNewMessage;
        public event EventHandler<string> OnError;
        private static Analogy.AnalogyClient Client { get; set; }
        private readonly AsyncServerStreamingCall<AnalogyGRPCLogMessage> _stream;
        private CancellationTokenSource _cts;
        private Channel channel;
        private Task consumer;
        static AnalogyMessageConsumer()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        public AnalogyMessageConsumer(string address)
        {
            channel = new Channel(address, ChannelCredentials.Insecure);
            Client = new Analogy.AnalogyClient(channel);
            AnalogyConsumerMessage m = new AnalogyConsumerMessage { Message = "client" };
            _stream = Client.SubscribeForConsumingMessages(m);
            consumer = Task.Factory.StartNew(GetMessages);
        }

        private async Task GetMessages()
        {
            _cts = new CancellationTokenSource();
            try
            {
                while (await _stream.ResponseStream.MoveNext(_cts.Token))
                {
                    var m = _stream.ResponseStream.Current;
                    var token = _cts.Token;
                    Interfaces.AnalogyLogMessage msg = new Interfaces.AnalogyLogMessage()
                    {
                        Level = (AnalogyLogLevel)m.Level,
                        Class = (AnalogyLogClass)m.Class,
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
                        msg.AddOrReplaceAdditionalProperty("Category", m.Category);
                    }
                    msg.Id = string.IsNullOrEmpty(m.Id)
                        ? Guid.NewGuid()
                        : Guid.TryParse(m.Id, out Guid id) ? id : Guid.NewGuid();
                    OnNewMessage?.Invoke(this, msg);
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, e.Message);
            }
        }
        public async Task Stop()
        {
            _cts?.Cancel();
            await consumer;
            await channel.ShutdownAsync();
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _stream?.Dispose();
            _ = channel?.ShutdownAsync();
        }
    }
}
#endif