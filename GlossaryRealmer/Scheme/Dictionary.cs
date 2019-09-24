using System;
using System.Collections.Generic;
using Realms;

namespace Realmer.Scheme
{
    internal class Dictionary : RealmObject
    {
        internal int PK => DictionaryId;

        [PrimaryKey]
        public int DictionaryId { get; set; }
        public WordList WordList { get; set; }
        public DictionaryInfo Info { get; set; }
    }

    internal class DictionaryCounter : RealmObject
    {
        public RealmInteger<int> Count { get; set; }
    }
}
