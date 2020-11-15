using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Analogy.LogServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MessagesController : Controller
    {
        private readonly MessagesContainer messageContainer;
        private ILogger<MessagesController> Logger { get; }

        public MessagesController(MessagesContainer messageContainer, ILogger<MessagesController> logger)
        {
            this.messageContainer = messageContainer;
            Logger = logger;
        }
        public class Message
        {
            public string Text { get; set; }

            public string Level { get; set; }

            public string Source { get; set; }
            public string Module { get; set; }
            public string MachineName { get; set; }

            public Message()
            {
                Text = Level = Source = Module = MachineName = string.Empty;
            }
        }
        [HttpPost()]
        public ActionResult LogMessageObject(Message msg)
        {
            AnalogyGRPCLogMessage m = new AnalogyGRPCLogMessage
            {
                Text = msg.Text,
                Date = Timestamp.FromDateTime(DateTime.UtcNow),
                Category = $"Http Post ({nameof(LogMessageObject)})",
                Source = msg.Source,
                Module = msg.Module,
                Id = Guid.NewGuid().ToString(),
                Level = Utils.GetLogLevelFromString(msg.Level),
                Class = AnalogyGRPCLogClass.General,
                MachineName = msg.MachineName,
                FileName = string.Empty,
                MethodName = string.Empty,
                User = string.Empty
            };
            messageContainer.AddMessage(m);
            return Ok();
        }

        [HttpPost()]
        public ActionResult LogMessage(string msg, string level)
        {
            AnalogyGRPCLogMessage m = new AnalogyGRPCLogMessage
            {
                Text = msg,
                Date = Timestamp.FromDateTime(DateTime.UtcNow),
                Category = $"Http Post ({nameof(LogMessage)})",
                Source = string.Empty,
                Module = string.Empty,
                Id = Guid.NewGuid().ToString(),
                Level = Utils.GetLogLevelFromString(level),
                MachineName = string.Empty,
                Class = AnalogyGRPCLogClass.General,
                FileName = string.Empty,
                MethodName = string.Empty,
                User = string.Empty

            };
            messageContainer.AddMessage(m);
            return Ok();
        }

    }
}
