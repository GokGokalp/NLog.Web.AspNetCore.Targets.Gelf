using Newtonsoft.Json.Linq;

namespace NLog.Web.AspNetCore.Targets.Gelf
{
    public interface IConverter
    {
        JObject GetGelfJson(LogEventInfo logEventInfo, string facility, string gelfVersion = "1.0");
    }
}