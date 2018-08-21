using System.IO;
using Newtonsoft.Json;

namespace Pericles.Configuration
{
    public static class ConfigDeserializer
    {
        public static T Deserialize<T>(string configFilepath)
        {
            var jsonConfig = File.ReadAllText(configFilepath);
            return JsonConvert.DeserializeObject<T>(jsonConfig);
        }
    }
}