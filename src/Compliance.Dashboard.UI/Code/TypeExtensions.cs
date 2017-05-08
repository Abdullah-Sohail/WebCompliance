using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.Dashboard.UI.Code
{
    public static class TypeExtensions
    {
        public static int GetInt(this string key, int defaultReturn = -1)
        {
            var value = ConfigurationManager.AppSettings[key];
            int temp = defaultReturn;
            int.TryParse(value, out temp);

            return temp;
        }

        public static bool GetBool(this string key, bool defaultReturn = false)
        {
            var value = ConfigurationManager.AppSettings[key];
            bool temp = defaultReturn;
            bool.TryParse(value, out temp);

            return temp;
        }
    }
}
