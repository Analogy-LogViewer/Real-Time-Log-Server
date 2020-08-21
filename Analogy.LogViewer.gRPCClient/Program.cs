using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Analogy.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;


namespace Analogy.LogViewer.gRPCClient
{
    class Program
    {
        //private static AsyncDuplexStreamingCall<ClientActionMessage, ServerAction> _duplexStream;
        private static Task _responseTask;
        private static CancellationTokenSource _cancellationTokenSource;
        private static Analogy.LogViewer.gRPCLogServer.Analogy.AnalogyClient client { get; set; }
        private GrpcChannel channel;
        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            //// The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("http://localhost:6000");
            client = new Analogy.LogViewer.gRPCLogServer.Analogy.AnalogyClient(channel);
            _cancellationTokenSource = new CancellationTokenSource();
            var stream = client.SubscribeForSendMessages();
            var m = new gRPCLogServer.AnalogyLogMessage()
            {
                Text = "Test Message (Init)",
                Category = "",
                Class = AnalogyLogClass.General.ToString(),
                Date = Timestamp.FromDateTime(DateTime.UtcNow),
                FileName = "",
                Id = Guid.NewGuid().ToString(),
                Level = AnalogyLogLevel.Event.ToString(),
                LineNumber = 0,
                MachineName = Environment.MachineName,
                MethodName = "",
                Module = "",
                ProcessId = 0,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Source = "None",
                User = Environment.UserName
            };
            await stream.RequestStream.WriteAsync(m);
            m.Text = "test 2";
            await stream.RequestStream.WriteAsync(m);
            Console.ReadKey();
        }

    }
}
