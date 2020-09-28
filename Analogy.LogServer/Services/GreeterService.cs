using Analogy.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Analogy.LogServer.Services
{
    public class GreeterService : Analogy.AnalogyBase
    {

        private readonly GRPCLogConsumer _grpcLogConsumer;
        private ILogger<GreeterService> Logger { get; }
        private MessagesContainer MessageContainer { get; }
        private MessageHistoryContainer MessageHistoryContainer { get; }
        public GreeterService(MessagesContainer messageContainer, MessageHistoryContainer historyContainer, GRPCLogConsumer grpcLogConsumer, ILogger<GreeterService> logger)
        {
            MessageHistoryContainer = historyContainer;
            _grpcLogConsumer = grpcLogConsumer;
            Logger = logger;
            MessageContainer = messageContainer;
        }

        public override async Task<AnalogyMessageReply> SubscribeForSendMessages(
            IAsyncStreamReader<AnalogyLogMessage> requestStream, ServerCallContext context)
        {
            Logger.LogInformation("Client subscribe for sending messages");
            var tasks = Task.WhenAll(AwaitCancellation(context.CancellationToken),
                HandleClientActions(requestStream, context.CancellationToken));

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.Message);
            }
            Logger.LogInformation("Subscription finished.");
            return new AnalogyMessageReply { Message = "Reply at " + DateTime.Now };
        }

        public override async Task SubscribeForConsumeMessages(AnalogyConsumerMessage request, IServerStreamWriter<AnalogyLogMessage> responseStream, ServerCallContext context)
        {
            _grpcLogConsumer.AddGrpcConsumer(request.Message, responseStream);
            await responseStream.WriteAsync(new AnalogyLogMessage
            {
                Category = "Server Message",
                Text = "Connection Established. Streaming old messages (if Any)",
                Class = AnalogyLogClass.General.ToString(),
                Level = AnalogyLogLevel.Analogy.ToString(),
                Date = Timestamp.FromDateTime(DateTime.UtcNow),
                FileName = "",
                Id = Guid.NewGuid().ToString(),
                LineNumber = 0,
                MachineName = Environment.MachineName,
                MethodName = nameof(SubscribeForConsumeMessages),
                Module = Process.GetCurrentProcess().ProcessName,
                ProcessId = Process.GetCurrentProcess().Id,
                Source = "Server Operations",
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                User = Environment.UserName

            });
            var oldMessages = MessageHistoryContainer.GetOldMessages();
            if (oldMessages.Any())
                await responseStream.WriteAllAsync(oldMessages);

            try
            {
                await AwaitCancellation(context.CancellationToken);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error");
            }
        }

        private async Task HandleClientActions(IAsyncStreamReader<AnalogyLogMessage> requestStream, CancellationToken token)
        {
            try
            {
                await foreach (var message in requestStream.ReadAllAsync(token))
                {
                    try
                    {
                        if (message.Date == null)
                        {
                            message.Date = Timestamp.FromDateTime(DateTime.UtcNow);
                        }

                        if (string.IsNullOrEmpty(message.Level))
                            message.Level = AnalogyLogLevel.Unknown.ToString();
                        if (string.IsNullOrEmpty(message.Id))
                            message.Id = Guid.NewGuid().ToString();
                        MessageContainer.AddMessage(message);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Error  receiving messages: {e}");
                    }
                }
            }
            catch (Exception e)
            {

                Logger.LogError($"Error: {e.Message}");
            }
        }

        private Task AwaitCancellation(CancellationToken token)
        {
            var completion = new TaskCompletionSource<object>();
            token.Register(() => { completion.SetResult(null); });
            return completion.Task;

        }
    }
}

