using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Web.Util
{
    public static class UtilCookies
    {
        public static Dictionary<string, string> FromLegacyCookieString(string legacyCookie)
        {
            if(string.IsNullOrEmpty(legacyCookie))
            {
                return new Dictionary<string, string>();
            }
            return legacyCookie.Split('&').Select(s => s.Split('=')).ToDictionary(kvp => kvp[0], kvp => kvp[1]);
        }

        public static string ToLegacyCookieString(IDictionary<string, string> dict)
        {
            return string.Join("&", dict.Select(kvp => string.Join("=", kvp.Key, kvp.Value)));
        }
    }
}
