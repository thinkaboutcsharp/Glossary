using System;
using System.Collections.Generic;

namespace Realmer.Poco
{
    public readonly struct DictionaryPerformanceListWordByWord
    {
        internal int PK => DictionaryId;

        public int DictionaryId { get; }
        public IReadOnlyList<PerformanceWordByWord> WordPerformances { get; }

        public DictionaryPerformanceListWordByWord(int dictionaryId, IList<PerformanceWordByWord> performances)
        {
            DictionaryId = dictionaryId;
            WordPerformances = (IReadOnlyList<PerformanceWordByWord>)performances;
        }

        internal DictionaryPerformanceListWordByWord(dynamic carrier)
            : this((int)carrier.DictionaryId, (IList<PerformanceWordByWord>)carrier.WordPerformances)
        { }
    }
}
