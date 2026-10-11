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
        private readonly GRPCLogConsumer grpcLogConsumer;
        private ILogger<GreeterService> Logger { get; }
        private MessagesContainer MessageContainer { get; }
        private MessageHistoryContainer MessageHistoryContainer { get; }
        public GreeterService(MessagesContainer messageContainer, MessageHistoryContainer historyContainer, GRPCLogConsumer grpcLogConsumer, ILogger<GreeterService> logger)
        {
            MessageHistoryContainer = historyContainer;
            this.grpcLogConsumer = grpcLogConsumer;
            Logger = logger;
            MessageContainer = messageContainer;
        }
        public override async Task<AnalogyMessageReply> SubscribeForPublishingMessages(IAsyncStreamReader<AnalogyGRPCLogMessage> requestStream, ServerCallContext context)
        {
            string connectionInfo = context.RequestHeaders.Any()
                ? string.Join(",", context.RequestHeaders.Select(h => h.Value).ToArray())
                : string.Empty;
            Logger.LogInformation("Client subscribe for sending messages. request info: {Info}", connectionInfo);
            var tasks = Task.WhenAll(AwaitCancellation(context.CancellationToken), HandleClientPublisingMessages(requestStream, context, context.CancellationToken));
            Logger.LogWarning("Client subscribe ended for: {Info}", connectionInfo);
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Logger.LogInformation("Error: {Error}", ex.Message);
            }
            Logger.LogInformation("Subscription finished.");
            return new AnalogyMessageReply { Message = "Reply at " + DateTime.Now };
        }

        public override async Task SubscribeForConsumingMessages(AnalogyConsumerMessage request, IServerStreamWriter<AnalogyGRPCLogMessage> responseStream, ServerCallContext context)
        {
            grpcLogConsumer.AddGrpcConsumer(request.Message, responseStream);
            await responseStream.WriteAsync(new AnalogyGRPCLogMessage
            {
                Category = "Server Message",
                Text = "Connection Established. Streaming old messages (if Any)",
                Level = AnalogyGRPCLogLevel.Analogy,
                Date = Timestamp.FromDateTime(DateTime.UtcNow),
                FileName = "",
                Id = Guid.NewGuid().ToString(),
                LineNumber = 0,
                MachineName = Environment.MachineName,
                Module = Process.GetCurrentProcess().ProcessName,
                ProcessId = Environment.ProcessId,
                Source = "Server Operations",
                ThreadId = Environment.CurrentManagedThreadId,
                User = Environment.UserName,
            });
            var oldMessages = await MessageHistoryContainer.GetOldMessages().ConfigureAwait(false);
            if (oldMessages.Any())
            {
                await responseStream.WriteAllAsync(oldMessages);
            }

            try
            {
                await AwaitCancellation(context.CancellationToken);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error");
            }
        }

        private async Task HandleClientPublisingMessages(IAsyncStreamReader<AnalogyGRPCLogMessage> requestStream,
            ServerCallContext serverCallContext, CancellationToken token)
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

                        if (string.IsNullOrEmpty(message.Id))
                        {
                            message.Id = Guid.NewGuid().ToString();
                        }

                        MessageContainer.AddMessage(message);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError("Error  receiving messages: {Error}", e.Message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Logger.LogError("Consuming ended for Peer: {PEer}", serverCallContext.Peer);
            }
            catch (Exception e)
            {
                Logger.LogError("Error: {Error}", e.Message);
            }
        }

#pragma warning disable CA1822 // Mark members as static
        private Task AwaitCancellation(CancellationToken token)
#pragma warning restore CA1822 // Mark members as static
        {
            var completion = new TaskCompletionSource<object>();
            token.Register(() => completion.SetResult(null!));
            return completion.Task;
        }
    }
}