using System.Runtime.Serialization;

namespace RegistrationSeleniumTests
{
    [DataContract]
    internal class Step
    {
        public StepAction Action { get; set; }
        public string Url { get; set; }
        public string XPath { get; set; }
    }
}
