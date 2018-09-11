using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SHClient.Services
{
    public class JsonService : IJsonService
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, _jsonSerializerSettings);
        }
    }
}
