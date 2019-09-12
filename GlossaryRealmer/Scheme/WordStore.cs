using Realms;
using System;
using System.Collections.Generic;

namespace Realmer.Scheme
{
    public class WordStore : RealmObject
    {
        [PrimaryKey]
        public long WordId { get; set; }
        public int DictionaryId { get; set; }
        public string Word { get; set; }
    }

    public class WordStoreCounter : RealmObject
    {
        public RealmInteger<long> Count { get; set; }
    }
}
