using System;
using System.Collections.Generic;

namespace Realmer.Poco
{
    public readonly struct PerformanceDictionaryByDictionary
    {
        internal int PK => UserId;

        public int UserId { get; }
        public IReadOnlyList<DictionaryPerformanceListWordByWord> DictionaryPerformances { get; }

        public PerformanceDictionaryByDictionary(int userId, IList<DictionaryPerformanceListWordByWord> dictionaryPerformances)
        {
            UserId = userId;
            DictionaryPerformances = (IReadOnlyList<DictionaryPerformanceListWordByWord>)dictionaryPerformances;
        }

        internal PerformanceDictionaryByDictionary(dynamic carrier)
            : this((int)carrier.UserId, (IList<DictionaryPerformanceListWordByWord>)carrier.DictionaryPerformances)
        { }
    }
}
