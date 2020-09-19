# Analogy gRPC Log Server

<p align="center">
    
[![Gitter](https://badges.gitter.im/Analogy-LogViewer/community.svg)](https://gitter.im/Analogy-LogViewer/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge) 
[![Build Status](https://dev.azure.com/Analogy-LogViewer/Analogy%20Log%20Viewer/_apis/build/status/Analogy-LogViewer.Analogy.LogViewer.gRPCLogServer?branchName=master)](https://dev.azure.com/Analogy-LogViewer/Analogy%20Log%20Viewer/_build/latest?definitionId=35&branchName=master) <a href="https://github.com/Analogy-LogViewer/Analogy.LogViewer.gRPCLogServer/issues">
    <img src="https://img.shields.io/github/issues/Analogy-LogViewer/Analogy.LogViewer.gRPCLogServer"  alt="Issues" />
</a>
![GitHub closed issues](https://img.shields.io/github/issues-closed-raw/Analogy-LogViewer/Analogy.LogViewer.gRPCLogServer)
<a href="https://github.com/Analogy-LogViewer/Analogy.LogViewer.gRPCLogServer/blob/master/LICENSE.md">
    <img src="https://img.shields.io/github/license/Analogy-LogViewer/Analogy.LogViewer.gRPCLogServer"  alt="License" />
</a>
</p>

gRPC Log Server (Windows Services) to receive and then forward messages to Analogy Log Viewer

Combined with  https://github.com/Analogy-LogViewer/Analogy.LogViewer.gRPC

There  are, currently, 4 types of gRPC clients:
| Data Provider   |      Nuget Version |
|----------|:---------------|
| [NLog Target](https://github.com/Analogy-LogViewer/Analogy.LogViewer.NLog.Targets) | NLog target to stream logs to Analogy|
| [Serilog Sink](https://github.com/Analogy-LogViewer/Analogy.LogViewer.Serilog) | Serilog Sink for sending logs to Analogy|
| [Analogy.AspNetCore.LogProvider](https://github.com/Analogy-LogViewer/Analogy.AspNetCore.LogProvider/) | Custom logging provider in Asp.Net Core that write logs to Analogy |
| [Python Logging](https://github.com/Analogy-LogViewer/Analogy-Python-Logging/) | Python gRPC client logger that sends logs to Analogy |

## Usage

Once you have setup Analogy Log Server you can start sending messages to it:


- NLog Target: 
Add nuget [Analogy.LogViewer.NLog.Targets](https://www.nuget.org/packages/Analogy.LogViewer.NLog.Targets/)
 In your nlog.config add:
 ```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true"
      internalLogLevel="warn">

  <extensions>
    <add assembly="NLog.Web.AspNetCore"/>
    <add assembly="Analogy.LogViewer.NLog.Targets"/>
  </extensions>
 
  <targets>
    <!-- write logs to file  -->
    <target xsi:type="File" name="allfile"
        fileName="c:\KALPA\logs\${processname:fullName=false}-KX.nlog"
        layout="${longdate}|${uppercase:${level}}|${logger}|${message}${exception:format=tostring}|${processname:fullName=false}|${processid}"
        keepFileOpen="false"
        archiveFileName="c:\KALPA\logs\${processname:fullName=false}-KX-${shortdate}.{##}.nlog"
        archiveNumbering="Sequence"
        archiveEvery="Day"
        maxArchiveFiles="100"
        archiveAboveSize="30000000">
    </target>
    <!-- write logs to Analogy Log Server  -->
    <target xsi:type="NlogAnalogyGRPCTarget" name="NLogToAnalogyGRPCTarget"
            layout="${longdate}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}|${processname:fullName=false}|${processid}">
      <contextproperty name="MachineName" layout="${machinename}" />
      <contextproperty name="ThreadId" layout="${threadid}"/>
      <contextproperty name="ProcessId" layout="${processid}" />
      <contextproperty name="callsite" layout="   ${callsite:className=true:fileName=true:includeSourcePath=true:methodName=true}" />
      <contextproperty name="ProcessName" layout="${processname:fullName=false}" />
    </target>
  </targets>

  <rules>
    <logger name="*" minlevel="Trace" writeTo="allfile" />
    <logger name="*" minlevel="Trace" writeTo="NLogToAnalogyGRPCTarget" />
  </rules>
</nlog>
```
 
-  Serilog Sink: 
Add nuget [Analogy.LogViewer.Serilog.Sinks](https://www.nuget.org/packages/Analogy.LogViewer.Serilog.Sinks//)


- Analogy.AspNetCore.LogProvider:
Add Nuget package [Analogy.AspNetCore.LogProvider](https://www.nuget.org/packages/Analogy.AspNetCore.LogProvider/) and then add to the Configure method the following in te Startup.cs

```csharp

 public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
 {
     loggerFactory.AddAnalogyLogger(new AnalogyLoggerConfiguration
     {
         LogLevel = LogLevel.Trace,
         EventId = 0,
         AnalogyServerUrl = "http://localhost:6000"
      });
     }

```

- Python logging: go to [Python Logging](https://github.com/Analogy-LogViewer/Analogy-Python-Logging/) for more information.
