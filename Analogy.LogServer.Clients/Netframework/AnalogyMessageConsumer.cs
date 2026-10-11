#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#if !NET
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
#pragma warning disable MA0046 // Use EventHandler<T> to declare events
        public event EventHandler<Interfaces.DataTypes.AnalogyLogMessage> OnNewMessage;
#pragma warning restore MA0046 // Use EventHandler<T> to declare events
#pragma warning disable MA0046 // Use EventHandler<T> to declare events
        public event EventHandler<string> OnError;
#pragma warning restore MA0046 // Use EventHandler<T> to declare events
        private static Analogy.AnalogyClient Client { get; set; }
        private readonly AsyncServerStreamingCall<AnalogyGRPCLogMessage> stream;
        private CancellationTokenSource cts;
        private Channel channel;
        private Task consumer;

        static AnalogyMessageConsumer()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", isEnabled: true);
        }

        public AnalogyMessageConsumer(string address)
        {
            channel = new Channel(address, ChannelCredentials.Insecure);
            Client = new Analogy.AnalogyClient(channel);
            AnalogyConsumerMessage m = new AnalogyConsumerMessage { Message = "client" };
            stream = Client.SubscribeForConsumingMessages(m);
            consumer = Task.Factory.StartNew(GetMessages);
        }

        private async Task GetMessages()
        {
            cts = new CancellationTokenSource();
            try
            {
                while (await stream.ResponseStream.MoveNext(cts.Token))
                {
                    var m = stream.ResponseStream.Current;
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
            cts?.Cancel();
            await consumer;
            await channel.ShutdownAsync();
        }

        public void Dispose()
        {
            cts?.Cancel();
            cts?.Dispose();
            stream?.Dispose();
            _ = channel?.ShutdownAsync();
        }
    }
}
#endif