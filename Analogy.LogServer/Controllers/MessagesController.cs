using System;
using System.Threading.Tasks;
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
        [HttpPost()]
        public async Task<ActionResult> LogMessage(string msg,string level)
        {
            AnalogyGRPCLogMessage m = new AnalogyGRPCLogMessage();
            m.Text = msg;
            m.Date = Timestamp.FromDateTime(DateTime.UtcNow);
            m.Category = "Http Post";
            m.Source = string.Empty;
            m.Module = string.Empty;
            m.Id = Guid.NewGuid().ToString();
            switch (level)
            {
                case "Error":
                    Logger.LogError(msg);
                    m.Level = AnalogyGRPCLogLevel.Error;
                    break;
                case "Info":
                    Logger.LogInformation(msg); 
                    m.Level = AnalogyGRPCLogLevel.Information;
                    break;
                case "warn":
                    Logger.LogWarning(msg);
                    m.Level = AnalogyGRPCLogLevel.Warning;
                    break;
            }
            messageContainer.AddMessage(m);
            return Ok();
        }
    }
}
