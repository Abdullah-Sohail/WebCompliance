using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.Common.Extensions
{
    public static class ExtendGuid
    {
        public static string ToBase64Url(this Guid theGuid)
        {
            return Convert.ToBase64String(theGuid.ToByteArray())
                .Replace("/", "-")
                .Replace("+", "_")
                .Replace("=", "");
        }

        public static Guid FromBase64Url(this Guid theGuid, string encoded)
        {
            Guid guid = Guid.Empty;
            encoded = encoded.Replace("-", "/").Replace("_", "+") + "==";

            try
            {
                guid = new Guid(Convert.FromBase64String(encoded));
            }
            catch (Exception ex)
            {
                throw new Exception("Bad Base64 conversion to GUID", ex);
            }

            return guid;
        }
    }
}
