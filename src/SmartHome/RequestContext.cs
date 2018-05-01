using System.Net;
using Newtonsoft.Json.Linq;

namespace SmartHome
{
    public class RequestContext
    {
        public RequestContext(JObject data, IPAddress address)
        {
            Data = data;
            Address = address;
        }

        public JObject Data { get; set; }

        public IPAddress Address { get; set; }
    }
}
