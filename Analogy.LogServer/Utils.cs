using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Analogy.Interfaces;

namespace Analogy.LogServer
{
    public static class Utils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public static AnalogyGRPCLogLevel GetLogLevelFromString(string level)
        {
            switch (level)
            {
                case "Disabled":
                case "Off":
                case "OFF":
                case "None":
                case "NONE":
                    return AnalogyGRPCLogLevel.None;
                case "TCE":
                case "TRC":
                case "Trace":
                case "TRACE":
                    return AnalogyGRPCLogLevel.Trace;
                case "DBG":
                case "Debug":
                case "DEBUG":
                case "DebugVerbose":
                    return AnalogyGRPCLogLevel.Debug;
                case "INF":
                case "Info":
                case "INFO":
                case "Event":
                case "Information":
                case "information":
                case "INFORMATION":
                    return AnalogyGRPCLogLevel.Information;
                case "WRN":
                case "Warn":
                case "WARN":
                case "Warning":
                case "WARNING":
                    return AnalogyGRPCLogLevel.Warning;
                case "Error":
                case "ERROR":
                case "ERR":
                case "Err":
                    return AnalogyGRPCLogLevel.Error;
                case "FTL":
                case "Critical":
                case "Fatal":
                case "FATAL":
                    return AnalogyGRPCLogLevel.Critical;
                case "Verbose":
                case "VERBOSE":
                case "DebugInfo":
                    return AnalogyGRPCLogLevel.Verbose;
                case "AnalogyInformation":
                case "Analogy":
                case "ANALOGY":
                    return AnalogyGRPCLogLevel.Analogy;
                default:
                    return AnalogyGRPCLogLevel.Unknown;
            }
        }
    }
}
