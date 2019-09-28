using System.Collections.Generic;
using System.Dynamic;

namespace Glossary.Util
{
    class Session : DynamicObject
    {
        IDictionary<string, object> sessionDatas = new Dictionary<string, object>();

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var key = (string)indexes[0];
            if (sessionDatas.TryGetValue(key, out result))
            {
                return true;
            }
            return false;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            var key = (string)indexes[0];
            if (sessionDatas.ContainsKey(key))
            {
                sessionDatas[key] = value;
            }
            else
            {
                sessionDatas.Add(key, value);
            }

            return true;
        }
    }

    internal static class AppSession
    {
        static dynamic session = new Session();
        static void Value<T>(string key, T value) { session[key] = value; }
        static T Value<T>(string key) { return (T)session[key]; }
    }
}
