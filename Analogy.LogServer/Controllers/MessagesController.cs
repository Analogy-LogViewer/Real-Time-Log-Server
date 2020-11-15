using System;
using System.Threading.Tasks;
using Analogy.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Analogy.LogServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MessagesController : Controller
    {
        private readonly MessagesContainer messageContainer;
        private ILogger<MessagesController> Logger { get; }

        public MessagesController(MessagesContainer messageContainer,ILogger<MessagesController> logger)
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

            public Message()
            {
                Text = Level = Source = Module = string.Empty;
            }
        }


        [HttpPost()]
        public async Task<ActionResult> LogMessage(Message msg)
        {
            AnalogyGRPCLogMessage m = new AnalogyGRPCLogMessage();
            m.Text = msg.Text;
            m.Date = Timestamp.FromDateTime(DateTime.UtcNow);
            m.Category = "Http Post";
            m.Source = msg.Source;
            m.Module = msg.Module;
            m.Id = Guid.NewGuid().ToString();
            m.Level = Utils.GetLogLevelFromString(msg.Level);
            messageContainer.AddMessage(m);
            return Ok();
        }
    }
}
