using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Analogy.LogServer.Interfaces
{
    public interface ILogConsumer
    {
        Task ConsumeLog(AnalogyGRPCLogMessage msg);
    }
}
