using Newtonsoft.Json;

namespace RegistrationSeleniumTests
{
    internal class SerializationHelper
    {
        public static string Serialize<T>(T t)
        {
            return JsonConvert.SerializeObject(t, Formatting.Indented);
        }

        public static T Deserialize<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
