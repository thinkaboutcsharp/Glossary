using System;

namespace Realmer.Util
{
    static class SchemeMapper
    {
        static internal Type GetSchemeType<TPoco>()
        {
            var name = typeof(TPoco).Name;

            switch (name)
            {
                case nameof(Poco.WordStore):
                    return typeof(Scheme.WordStore);
                default:
                    return typeof(object);
            }
        }
    }
}
