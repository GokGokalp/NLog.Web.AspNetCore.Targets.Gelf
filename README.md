# NLog.Web.AspNetCore.Targets.Gelf
Gelf4NLog is an [NLog] target implementation to push log messages to [GrayLog2]. It implements the [Gelf] specification and communicates with GrayLog server via UDP.

[![NuGet version](https://badge.fury.io/nu/NLog.Web.AspNetCore.Targets.Gelf.svg)](https://badge.fury.io/nu/NLog.Web.AspNetCore.Targets.Gelf)

## History
Code forked from https://github.com/2020Legal/NLog.Targets.Gelf which is a fork from https://github.com/akurdyukov/Gelf4NLog who forked the origonal code from https://github.com/seymen/Gelf4NLog

I transformed the project to .NET Core.

## Usage
Use Nuget:
<!--- 
```
$ dotnet add package NLog.Web.AspNetCore.Targets.Gelf
```
-->
```
$ dotnet add package NLog.Web.AspNetCore.Targets.Gelf
```
### Configuration
Here is a sample nlog.config configuration file for graylog:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true"
      throwExceptions="false"
      internalLogLevel="Off"
      internalLogFile="c:\temp\internal-nlog.txt">
  <extensions>
    <add assembly="NLog.Web.AspNetCore"/>
    <add assembly="NLog.Web.AspNetCore.Targets.Gelf"/>
  </extensions>
  <targets>
    <target xsi:type="File" name="debugFile" filename="C:\@Logs\${shortdate}-${level}-${applicationName}.txt" layout="${longdate}|${level:upperCase=true}|${logger}|${aspnet-Request-Method}|url: ${aspnet-Request-Url}${aspnet-Request-QueryString}|${message}" concurrentWrites="false" />
    <target xsi:type="Gelf" name="graylog" endpoint="udp://192.168.99.100:12201" facility="console-runner"  maxUdpPackageSize="1491" SendLastFormatParameter="true">
	
	<!-- Optional parameters -->
	<parameter name="param1" layout="${longdate}"/>
	<parameter name="param2" layout="${callsite}"/>
    </target>
  </targets>
  <rules>
    <logger name="*" minlevel="Debug" writeTo="debugFile, graylog" />
  </rules>
</nlog>
```

Options are the following:
* __name:__ arbitrary name given to the target
* __xsi:type:__ set this to "gelf"
* __endpoint:__ the uri pointing to the graylog2 input in the format udp://{IP or host name}:{port} *__note:__ support is currently only for udp transport protocol*
* __facility:__ The graylog2 facility to send log messages
* __sendLastFormatParameter:__ default false. If true last parameter of message format will be sent to graylog as separate field per property
* __ maxUdpPackageSize:__ Size of udp package sent to graylog. Useful when running .Net applications in docker because of (https://github.com/docker/for-win/issues/1144 "this bug")
###Code
```c#
//excerpt from ConsoleRunner
var eventInfo = new LogEventInfo
				{
					Message = comic.Title,
					Level = LogLevel.Info,
				};
eventInfo.Properties.Add("Publisher", comic.Publisher);
eventInfo.Properties.Add("ReleaseDate", comic.ReleaseDate);
Logger.Log(eventInfo);
```
or alternatively for simple log messages
```c#
Logger.Info("Simple message {0}", value);
```
or alternatively for use of sendLastFormatParameter
```c#
Logger.Info(comic.Title, new { Publisher = comic.Publisher, ReleaseDate = comic.ReleaseDate });
```
will log Publisher and ReleaseDate as separate fields in Graylog

[NLog]: http://nlog-project.org/
[GrayLog2]: http://graylog2.org/
[Gelf]: http://graylog2.org/about/gelf
