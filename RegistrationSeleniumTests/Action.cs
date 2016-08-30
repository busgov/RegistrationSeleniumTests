using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RegistrationSeleniumTests
{
    [DataContract]
    internal class Action
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public ActionType ActionType { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string XPath { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public bool Disabled { get; set; }

        [DataMember]
        public int PreDelay { get; set; }

        [DataMember]
        public int PostDelay { get; set; }
    }
}
