using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RegistrationSeleniumTests
{
    [DataContract]
    internal class Scenario
    {
        [DataMember]
        public string Name { get; set; } = "[Not defined]";

        [DataMember]
        public List<Action> Actions { get; set; } = new List<Action>();
    }
}
