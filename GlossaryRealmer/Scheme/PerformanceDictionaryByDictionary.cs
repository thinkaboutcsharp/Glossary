using System;
using System.Collections.Generic;
using Realms;

namespace Realmer.Scheme
{
    internal class PerformanceDictionaryByDictionary : RealmObject
    {
        internal int PK => UserId;

        [PrimaryKey]
        public int UserId { get; set; }

        public IList<DictionaryPerformanceListWordByWord> DictionaryPerformances { get; }
    }
}
