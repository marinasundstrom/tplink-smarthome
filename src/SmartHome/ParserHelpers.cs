using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHome
{
    static class ParserHelpers
    {
        internal static JObject ParseGetSysInfo(string str)
        {
            var obj = JObject.Parse(str)?
                                .Value<JObject>("system")?
                                .Value<JObject>("get_sysinfo");

            if (obj?.IsNullOrEmpty() ?? false)
            {
                return null;
            }
            else
            {
                return obj;
            }
        }
    }
}
