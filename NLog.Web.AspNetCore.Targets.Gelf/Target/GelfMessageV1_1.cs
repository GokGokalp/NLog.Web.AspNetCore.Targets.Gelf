using Newtonsoft.Json;

namespace NLog.Web.AspNetCore.Targets.Gelf
{
    public class GelfMessageV1_1
    {
        [JsonProperty("full_message")]
        public string FullMessage { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("short_message")]
        public string ShortMessage { get; set; }

        [JsonProperty("timestamp")]
        public double  Timestamp { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}
