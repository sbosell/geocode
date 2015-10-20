using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lucuma.Helper
{
    public static class JObjectExtensions
    {
        public static String GetStringValue<T>(this T JToken) where T : JToken
        {
            try
            {
                if (JToken != null)
                {
                    return JToken.Value<String>();
                }
            }
            catch
            {

            }
            return String.Empty;
        }
        public static double GetDoubleValue<T>(this T JToken) where T : JToken
        {
            try
            {
                if (JToken != null)
                {
                    return JToken.Value<double>();
                }
            }
            catch
            {

            }
            return 0;
        }
    }
}
