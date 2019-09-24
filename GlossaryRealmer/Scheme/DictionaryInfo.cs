using System;
using Realms;

namespace Realmer.Scheme
{
    internal class DictionaryInfo : RealmObject
    {
        internal int PK => DictionaryId;

        [PrimaryKey]
        public int DictionaryId { get; set; }
        //order color etc...
    }
}
