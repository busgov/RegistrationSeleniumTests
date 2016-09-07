using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RegistrationSeleniumTests
{
    [DataContract]
    internal class Action
    {
        [DataMember]
        [DefaultValue("")]
        public string Title { get; set; } = "";

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public ActionType ActionType { get; set; } = ActionType.None;

        [DataMember]
        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string XPath { get; set; }

        [DataMember]
        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Value { get; set; }

        [DataMember]
        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Disabled { get; set; }

        [DataMember]
        [DefaultValue(0)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int PreDelay { get; set; }

        [DataMember]
        [DefaultValue(800)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int PostDelay { get; set; }
    }
}
