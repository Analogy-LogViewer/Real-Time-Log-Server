using Analogy.Interfaces;
using Analogy.Interfaces.DataTypes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Analogy.LogServer.Services
{
    public class WindowsEventLogsMonitor : BackgroundService
    {
        public MessagesContainer MessageContainer { get; }
        public ServiceConfiguration Configuration { get; }
        public ILogger<WindowsEventLogsMonitor> Logger { get; }
        private List<EventLog> Logs { get; }

        public WindowsEventLogsMonitor(MessagesContainer messageContainer, ServiceConfiguration configuration, ILogger<WindowsEventLogsMonitor> logger)
        {
            MessageContainer = messageContainer;
            Configuration = configuration;
            Logger = logger;
            Logs = new List<EventLog>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (Configuration.WindowsEventLogsConfiguration.EnableMonitoring)
            {
                try
                {
                    SetupLogs();
                }
                catch (Exception e)
                {
                    string m = "Error Opening log. Please make sure you are running as Administrator." + Environment.NewLine + "Error:" + e.Message;
                    AnalogyGRPCLogMessage err = new AnalogyGRPCLogMessage { Level = AnalogyGRPCLogLevel.Error, Text = m, Date = Timestamp.FromDateTime(DateTime.UtcNow), Id = Guid.NewGuid().ToString() };
                    MessageContainer.AddMessage(err);
                }
            }
            return Task.CompletedTask;
        }

        private void SetupLogs()
        {
            foreach (EventLog eventLog in Logs)
            {
                eventLog.EnableRaisingEvents = false;
                eventLog.Dispose();
            }
            Logs.Clear();
            foreach (string logName in Configuration.WindowsEventLogsConfiguration.LogsToMonitor)
            {
                try
                {
                    if (EventLog.Exists(logName))
                    {
                        var eventLog = new EventLog(logName);
                        Logs.Add(eventLog);
                        
                        // set event handler
                        eventLog.EntryWritten += (apps, arg) =>
                        {
                            if (LogLevel(arg.Entry.EntryType) >= Configuration.WindowsEventLogsConfiguration.MinimumLogLevel)
                            {
                                AnalogyGRPCLogMessage m = CreateGRPCMessageFromEvent(arg.Entry);
                                m.Module = logName;
                                MessageContainer.AddMessage(m);
                            }
                        };

                        eventLog.EnableRaisingEvents = true;
                    }
                }
                catch (Exception e)
                {
                    string m = "Error Opening log. Please make sure you are running as Administrator." + Environment.NewLine + "Error:" + e.Message;
                    AnalogyGRPCLogMessage err = new AnalogyGRPCLogMessage
                    {
                        Level = AnalogyGRPCLogLevel.Error,
                        Text = m,
                        Date = Timestamp.FromDateTime(DateTime.UtcNow),
                        Id = Guid.NewGuid().ToString(),
                    };
                    MessageContainer.AddMessage(err);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static AnalogyLogLevel LogLevel(EventLogEntryType msgLevel)
        {
            AnalogyLogLevel level;
            switch (msgLevel)
            {
                case EventLogEntryType.Error:
                    level = AnalogyLogLevel.Error;
                    break;
                case EventLogEntryType.Warning:
                    level = AnalogyLogLevel.Warning;
                    break;
                case EventLogEntryType.Information:
                    level = AnalogyLogLevel.Information;
                    break;
                case EventLogEntryType.SuccessAudit:
                    level = AnalogyLogLevel.Information;
                    break;
                case EventLogEntryType.FailureAudit:
                    level = AnalogyLogLevel.Error;
                    break;
                default:
                    level = AnalogyLogLevel.Information;
                    break;
            }

            return level;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static AnalogyGRPCLogLevel GRPCLogLevel(EventLogEntryType msgLevel)
        {
            AnalogyGRPCLogLevel level;
            switch (msgLevel)
            {
                case EventLogEntryType.Error:
                    level = AnalogyGRPCLogLevel.Error;
                    break;
                case EventLogEntryType.Warning:
                    level = AnalogyGRPCLogLevel.Warning;
                    break;
                case EventLogEntryType.Information:
                    level = AnalogyGRPCLogLevel.Information;
                    break;
                case EventLogEntryType.SuccessAudit:
                    level = AnalogyGRPCLogLevel.Information;
                    break;
                case EventLogEntryType.FailureAudit:
                    level = AnalogyGRPCLogLevel.Error;
                    break;
                default:
                    level = AnalogyGRPCLogLevel.Information;
                    break;
            }

            return level;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static AnalogyGRPCLogMessage CreateGRPCMessageFromEvent(EventLogEntry eEntry)
        {
            AnalogyGRPCLogMessage m = new AnalogyGRPCLogMessage();
            m.Level = GRPCLogLevel(eEntry.EntryType);
            m.Category = eEntry.Category ?? "";
            m.Date = Timestamp.FromDateTime(eEntry.TimeGenerated.ToUniversalTime());
            m.Id = Guid.NewGuid().ToString();
            m.Source = eEntry.Source;
            m.Category = "Windows Event Logs";
            m.Text = eEntry.Message ?? "";
            m.User = eEntry.UserName ?? "";
            m.Module = eEntry.Source ?? "";
            return m;
        }
    }
}