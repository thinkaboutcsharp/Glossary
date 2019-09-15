using System;
using System.Collections.Generic;
using Realms;

namespace Realmer.Scheme
{
    public class DictionaryPerformanceListWordByWord : RealmObject
    {
        internal int PK => DictionaryId;

        [PrimaryKey]
        public int DictionaryId { get; set; }

        [Util.PocoClass(typeof(Poco.PerformanceWordByWord))]
        public IList<PerformanceWordByWord> WordPerformances { get; }
    }
}
