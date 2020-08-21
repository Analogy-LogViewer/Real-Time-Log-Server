using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Analogy.Interfaces;
using Analogy.LogViewer.gRPCLogServer;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using AnalogyLogMessage = Analogy.LogViewer.gRPCLogServer.AnalogyLogMessage;
using Enum = System.Enum;

namespace Analogy.LogViewer.gRPCClient
{
    public class AnalogyMessageConsumer
    {
        private static gRPCLogServer.Analogy.AnalogyClient client { get; set; }
        private GrpcChannel channel;
        private AsyncServerStreamingCall<AnalogyLogMessage> stream;
        private CancellationTokenSource cts;

        static AnalogyMessageConsumer()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        public AnalogyMessageConsumer()
        {
            channel = GrpcChannel.ForAddress("http://localhost:6000");
            client = new gRPCLogServer.Analogy.AnalogyClient(channel);
            AnalogyConsumerMessage m = new AnalogyConsumerMessage {Message = "client"};
            stream = client.SubscribeForConsumeMessages(m);
        }

        public async IAsyncEnumerable<Interfaces.AnalogyLogMessage> GetMessages()
        {
            cts=new CancellationTokenSource();
            await foreach (var m in stream.ResponseStream.ReadAllAsync())
            {

                Interfaces.AnalogyLogMessage msg = new Interfaces.AnalogyLogMessage()
                {
                    Id = Guid.Parse((ReadOnlySpan<char>) m.Id),
                    Category = m.Category,
                    Class = (AnalogyLogClass) Enum.Parse(typeof(AnalogyLogClass), m.Class),
                    Date = m.Date.ToDateTime().ToLocalTime(),
                    FileName = m.FileName,
                    Level = (AnalogyLogLevel) Enum.Parse(typeof(AnalogyLogLevel), m.Level),
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
                yield return msg;
                if (cts.Token.IsCancellationRequested)
                    yield break;
                
            }
        }

        public void Stop()
        {
            cts?.Cancel();
        }
    }
}
