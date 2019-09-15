using System;
using System.Collections.Generic;
using Realms;

namespace Realmer.Scheme
{
    public class PerformanceDictionaryByDictionary : RealmObject
    {
        internal int PK => UserId;

        [PrimaryKey]
        public int UserId { get; set; }

        [Util.PocoClass(typeof(Poco.DictionaryPerformanceListWordByWord))]
        public IList<DictionaryPerformanceListWordByWord> DictionaryPerformances { get; }
    }
}
