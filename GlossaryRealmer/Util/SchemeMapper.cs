using System;
using System.Collections.Generic;
using System.Linq;

namespace Realmer.Util
{
    static class SchemeMapper
    {
        static internal Type GetSchemeType<TPoco>()
        {
            return SwitchSchemeType<TPoco, Type>(SwitchType.Type);
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
            return SwitchSchemeType<TPoco, Func<dynamic, bool>>(SwitchType.Func, pk);
        }

        static TReturn SwitchSchemeType<TPoco, TReturn>(SwitchType type, long pk = 0L)
        {
            var pocoName = typeof(TPoco).Name;

            if (pocoName == nameof(Poco.WordStore))
            {
                if (type == SwitchType.Type) return (TReturn)(object)typeof(Scheme.WordStore);
                else return (TReturn)(object)new Func<dynamic, bool>((dynamic p) => p.WordId == pk);
            }

            return (TReturn)new object();
        }

        enum SwitchType
        {
            Type,
            Func
        }
    }
}
