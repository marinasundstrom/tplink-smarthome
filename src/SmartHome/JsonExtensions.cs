using Newtonsoft.Json.Linq;

namespace SmartHome
{
    public static class JsonExtensions
    {
        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null)
                   || (token.Type == JTokenType.Array && !token.HasValues)
                   || (token.Type == JTokenType.Object && !token.HasValues)
                   || (token.Type == JTokenType.String && token.ToString()?.Length == 0)
                   || (token.Type == JTokenType.Null);
        }
    }
}
