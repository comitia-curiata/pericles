using System;
using System.Collections.Generic;
using ElectionModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Pericles.Configuration
{
    public class NodeConfig
    {
        [JsonProperty(PropertyName = "electionType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ElectionType ElectionType { get; set; }

        [JsonProperty(PropertyName = "candidates")]
        public List<string> Candidates { get; set; }

        [JsonProperty(PropertyName = "electionEndTime")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime ElectionEndTime { get; set; }

        [JsonProperty(PropertyName = "voterDbFilepath")]
        public string VoterDbFilepath { get; set; }

        [JsonProperty(PropertyName = "isMiningNode")]
        public bool IsMiningNode { get; set; }

        [JsonProperty(PropertyName = "port")]
        public int Port { get; set; }
    }
}