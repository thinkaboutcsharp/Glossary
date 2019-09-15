using System;
using System.Collections.Generic;
using Realms;

namespace Realmer.Scheme
{
    public class Dictionary : RealmObject
    {
        internal int PK => DictionaryId;

        [PrimaryKey]
        public int DictionaryId { get; set; }
        public WordList WordList { get; set; }
        public DictionaryInfo Info { get; set; }
    }

    public class DictionaryCounter : RealmObject
    {
        public RealmInteger<int> Count { get; set; }
    }
}
