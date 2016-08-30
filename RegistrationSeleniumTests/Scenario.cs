using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RegistrationSeleniumTests
{
    [DataContract]
    internal class Scenario
    {
        public string Name { get; set; }
        public List<Step> Steps { get; set; }
    }
}
