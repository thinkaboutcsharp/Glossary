using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Realmer.Scheme
{
    public class WordList : RealmObject
    {
        internal int PK => DictionaryId;

        [PrimaryKey]
        public int DictionaryId { get; set; }

        [Util.PocoClass(typeof(Poco.WordStore))]
        public IList<WordStore> Words { get; }
    }
}
