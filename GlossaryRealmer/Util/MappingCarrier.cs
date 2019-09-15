using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Realmer.Util
{
    internal class MappingCarrier : DynamicObject
    {
        IDictionary<string, object> mapping = new Dictionary<string, object>();

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            var index = (string)indexes[0];
            SetValue(index, value);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return GetValue(binder.Name, out result);
        }

        void SetValue(string key, object value)
        {
            if (mapping.ContainsKey(key)) mapping[key] = value;
            else mapping.Add(key, value);
        }

        bool GetValue(string key, out object value)
        {
            if (mapping.ContainsKey(key)) { value = mapping[key]; return true; }
            else { value = new object(); return false; }
        }
    }
}
