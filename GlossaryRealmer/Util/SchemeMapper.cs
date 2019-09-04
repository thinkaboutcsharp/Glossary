using System;
using System.Collections.Generic;
using System.Linq;

namespace Realmer.Util
{
    static class SchemeMapper
    {
        static internal Type GetSchemeType<TPoco>()
        {
            var name = typeof(TPoco).Name;

            return name switch
            {
                nameof(Poco.WordStore) => typeof(Scheme.WordStore),
                _ => typeof(object)
            };
        }

        static string GetPKName<TPoco>()
        {
            var name = typeof(TPoco).Name;

            return name switch
            {
                nameof(Poco.WordStore) => nameof(Poco.WordStore.WordId),
                _ => ""
            };
        }

        static internal IEnumerable<long> GetPkEnum<TPoco>(IEnumerable<TPoco> obj)
        {
            var pkName = GetPKName<TPoco>();
            var dynamicRecord = (dynamic)obj!;
            long pk = (long)dynamicRecord[pkName];
            yield return pk;
        }

        static internal Func<dynamic, bool> GetPKFunc<TPoco>(TPoco record)
        {
            var pkName = GetPKName<TPoco>();
            var dynamicRecord = (dynamic)record!;
            long pk = (long)dynamicRecord[pkName];
            return GetPKFunc<TPoco>(pk);
        }

        static internal Func<dynamic, bool> GetPKFunc<TPoco>(long pk)
        {
            var name = typeof(TPoco).Name;

            return name switch
            {
                nameof(Poco.WordStore) => new Func<dynamic, bool>((dynamic p) => p.WordId == pk),
                _ => new Func<dynamic, bool>(_ => false)
            };
        }
    }
}
