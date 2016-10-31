using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace XL.Utilities
{
    public static class JsonHelper
    {
        public  static  T DeserializeJson<T>(string jsonString)
        {
            try
            {

                T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
                return t;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string SerializeToJson<T>(T t)
        {
            try
            {
                string result = string.Empty;

                result = Newtonsoft.Json.JsonConvert.SerializeObject(t);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
