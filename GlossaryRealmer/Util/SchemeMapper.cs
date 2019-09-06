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

        static internal IEnumerable<long> GetPkEnum<TPoco>(IEnumerable<TPoco> objEnum)
        {
            foreach (var obj in objEnum)
            {
                var dynamicRecord = (dynamic)obj!;
                long pk = dynamicRecord.PK;
                yield return pk;
            }
        }

        static internal Func<dynamic, bool> GetPKFunc<TPoco>(TPoco record)
        {
            var dynamicRecord = (dynamic)record!;
            long pk = dynamicRecord.PK;
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
