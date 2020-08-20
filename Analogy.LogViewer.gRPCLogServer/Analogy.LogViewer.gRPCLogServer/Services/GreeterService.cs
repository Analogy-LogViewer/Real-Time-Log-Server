using System;
using System.Collections.Generic;
using System.Linq;
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

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<AnalogyMessageReply> SendMessage(AnalogyLogMessage message, ServerCallContext context)
        {
            Interfaces.AnalogyLogMessage m = new Interfaces.AnalogyLogMessage
            {
                Text = message.Text,
                ThreadId = message.ThreadId,
                MachineName = message.MachineName,
                Id = Guid.Parse(message.Id),
                Category = message.Category,
                Source = message.Source,
                MethodName = message.MethodName,
                FileName = message.FileName,
                LineNumber = message.LineNumber,
                Class = (AnalogyLogClass)Enum.Parse(typeof(AnalogyLogClass), message.Class),
                Level = (AnalogyLogLevel)Enum.Parse(typeof(AnalogyLogLevel), message.Level),
                Module = message.Module,
                ProcessId = message.ProcessId,
                Date = message.Date.ToDateTime(),
                User = message.User,
                AdditionalInformation = null

            };
           // gRPCReporter.Instance.MessageReady(m);
            return Task.FromResult(new AnalogyMessageReply
            {
                Message = "Received at " + DateTime.Now

            });
        }
    }
}

