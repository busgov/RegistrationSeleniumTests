using Newtonsoft.Json;

namespace RegistrationSeleniumTests
{
    internal class SerializationHelper
    {
        public static string Serialize<T>(T t)
        {
            return JsonConvert.SerializeObject(
                t,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
        }

        public static T Deserialize<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
