//using PropertyChanged;
using Realmer.Util;
using System;
using System.Collections.Generic;

namespace Realmer.Poco
{
    //[AddINotifyPropertyChangedInterface]
    public class PerformanceDictionaryByDictionary : PocoBase<PerformanceDictionaryByDictionary, int>
    {
        public int UserId { get; set; }
        public IReadOnlyList<DictionaryPerformanceListWordByWord> DictionaryPerformances { get; }

        public PerformanceDictionaryByDictionary() : base(o => o.UserId, () => new Scheme.PerformanceDictionaryByDictionary())
        {
            DictionaryPerformances = AddListProperty(o => o.DictionaryPerformances);
        }
    }
}
