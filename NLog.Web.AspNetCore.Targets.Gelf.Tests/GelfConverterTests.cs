using Xunit;

namespace NLog.Web.AspNetCore.Targets.Gelf.Tests
{
    public class GelfConverterTests
    {
        [Fact]
        public void ShouldGetGelfJsonAddMappedDiagnosticsLogicalContextData()
        {
            MappedDiagnosticsLogicalContext.Set("test", "value");

            var logEvent = LogEventInfo.Create(LogLevel.Info, "loggerName", null, "message");

            var converter = new GelfConverter();

            // Act
            var gelfJson = converter.GetGelfJson(logEvent, "facility");

            Assert.Equal("value", gelfJson.Value<string>("_test"));
        }

        [Fact]
        public void ShouldGetGelfJsonDiscardMappedDiagnosticsLogicalContextDataIfPresentInLogEventInfo()
        {
            MappedDiagnosticsLogicalContext.Set("test", "value");
            
            var logEvent = LogEventInfo.Create(LogLevel.Info, "loggerName", null, "message");
            logEvent.Properties.Add("test", "anotherValue");

            var converter = new GelfConverter();

            // Act
            var gelfJson = converter.GetGelfJson(logEvent, "facility");

            Assert.Equal("anotherValue", gelfJson.Value<string>("_test"));
        }
    }
}