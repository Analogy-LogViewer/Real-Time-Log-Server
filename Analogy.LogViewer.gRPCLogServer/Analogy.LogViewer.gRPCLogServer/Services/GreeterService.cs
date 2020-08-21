using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Analogy.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Enum = System.Enum;

namespace Analogy.LogViewer.gRPCLogServer
{
    public class GreeterService : Analogy.AnalogyBase
    {
        private readonly ILogger<GreeterService> _logger;
        private AnalogyViewerSender Sender { get; }

        public GreeterService(AnalogyViewerSender sender, ILogger<GreeterService> logger)
        {
            _logger = logger;
            Sender = sender;
        }

        public override async Task<AnalogyMessageReply> SubscribeForSendMessages(
            IAsyncStreamReader<AnalogyLogMessage> requestStream, ServerCallContext context)
        {
            var tasks = Task.WhenAll(AwaitCancellation(context.CancellationToken),
                HandleClientActions(requestStream, context.CancellationToken));

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }

            _logger.LogInformation("Subscription finished.");
            // gRPCReporter.Instance.MessageReady(m);
            return new AnalogyMessageReply { Message = "Reply at " + DateTime.Now };
        }

        public override async Task SubscribeForConsumeMessages(AnalogyConsumerMessage request, IServerStreamWriter<AnalogyLogMessage> responseStream, ServerCallContext context)
        {
            Sender.AddConsumer(request.Message, responseStream);
            try
            {
                await AwaitCancellation(context.CancellationToken);
            }
            catch (Exception e)
            {
              _logger.LogError(e,"Error");
            }
        }

        private async Task HandleClientActions(IAsyncStreamReader<AnalogyLogMessage> requestStream,
            CancellationToken token)
        {
            ulong i = 0;
            try
            {
                await foreach (var message in requestStream.ReadAllAsync(token))
                {
                    try
                    {
                        Console.WriteLine("Received " +i++);
                        Sender.AddMessage(message);
                        //Interfaces.AnalogyLogMessage m = new Interfaces.AnalogyLogMessage
                        //{
                        //    Text = message.Text,
                        //    ThreadId = message.ThreadId,
                        //    MachineName = message.MachineName,
                        //    Id = Guid.Parse(message.Id),
                        //    Category = message.Category,
                        //    Source = message.Source,
                        //    MethodName = message.MethodName,
                        //    FileName = message.FileName,
                        //    LineNumber = message.LineNumber,
                        //    Class = (AnalogyLogClass) Enum.Parse(typeof(AnalogyLogClass), message.Class),
                        //    Level = (AnalogyLogLevel) Enum.Parse(typeof(AnalogyLogLevel), message.Level),
                        //    Module = message.Module,
                        //    ProcessId = message.ProcessId,
                        //    Date = message.Date.ToDateTime(),
                        //    User = message.User,
                        //    AdditionalInformation = null
                        //};
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Error  receiving messages: {e}");
                    }
                }
            }
            catch (Exception e)
            {

                _logger.LogError($"Error: {e.Message}");
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

