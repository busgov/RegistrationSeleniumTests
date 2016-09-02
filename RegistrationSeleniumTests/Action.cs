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
        public string Title { get; set; }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public ActionType ActionType { get; set; } = ActionType.None;

        [DataMember]
        [DefaultValue(null)]
        public string XPath { get; set; }

        [DataMember]
        [DefaultValue(null)]
        public string Value { get; set; }

        [DataMember]
        [DefaultValue(false)]
        public bool Disabled { get; set; }

        [DataMember]
        [DefaultValue(0)]
        public int PreDelay { get; set; }

        [DataMember]
        [DefaultValue(500)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int PostDelay { get; set; }
    }
}
