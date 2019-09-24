using AutoMapper.Configuration.Annotations;
using Realms;
using System.Collections.Generic;

namespace Realmer.Scheme
{
    internal class DictionaryPerformanceListWordByWord : RealmObject
    {
        internal int PK => DictionaryId;

        [PrimaryKey]
        public int DictionaryId { get; set; }
        [Ignore]
        public IList<PerformanceWordByWord> WordPerformances { get; }
    }
}
