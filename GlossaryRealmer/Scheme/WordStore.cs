using Realms;
using System;
using System.Collections.Generic;

namespace Realmer.Scheme
{
    internal class WordStore : RealmObject
    {
        internal long PK => WordId;

        [PrimaryKey]
        public long WordId { get; set; }
        public int DictionaryId { get; set; }
        public string Word { get; set; }
    }

    internal class WordStoreCounter : RealmObject
    {
        public RealmInteger<long> Count { get; set; }
    }
}
