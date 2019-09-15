using System;
using System.Collections.Generic;
using System.Reflection;

namespace Realmer.Util
{
    internal class PocoBuilder
    {
        static IDictionary<Type, ConstructorInfo> ctors = new Dictionary<Type, ConstructorInfo>();

        internal dynamic Build(Type schemeType, Type pocoType, dynamic scheme)
        {
            ConstructorInfo ctor;
            if (ctors.TryGetValue(pocoType, out ctor))
            {
                ctor = ctors[pocoType];
            }
            else
            {
                ctor = pocoType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(object) }, null);
            }

            dynamic carrier = new MappingCarrier();

            var schemeProperties = schemeType.GetProperties();
            for (int p = 0; p < schemeProperties.Length; p++)
            {
                var property = schemeProperties[p];
                if (property.PropertyType.IsGenericType)
                {
                    if (property.PropertyType.GetGenericTypeDefinition() == typeof(IList<>))
                    {

                    }
                }
                else
                {
                    carrier[property.Name] = property.GetValue(scheme);
                }
            }

            var pocoObj = ctor.Invoke(new object[] { carrier });
            return pocoObj;
        }
    }
}
