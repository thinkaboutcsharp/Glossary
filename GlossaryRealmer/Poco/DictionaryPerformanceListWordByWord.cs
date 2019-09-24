//using PropertyChanged;
using Realmer.Util;
using System;
using System.Collections.Generic;

namespace Realmer.Poco
{
    //[AddINotifyPropertyChangedInterface]
    public class DictionaryPerformanceListWordByWord : PocoBase<DictionaryPerformanceListWordByWord, int>
    {
        public int DictionaryId { get; }
        public IReadOnlyList<PerformanceWordByWord> WordPerformances { get; }

        public DictionaryPerformanceListWordByWord() : base(o => o.DictionaryId, () => new Scheme.DictionaryPerformanceListWordByWord())
        {
            WordPerformances = AddListProperty(o => o.WordPerformances);
        }
    }
}
