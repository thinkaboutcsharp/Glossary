using System;
using System.Collections.Generic;
using Realms;

namespace Realmer.Scheme
{
    public class DictionaryList : RealmObject
    {
        internal int PK => ApplicationId;

        [PrimaryKey]
        public int ApplicationId { get; set; }

        [Util.PocoClass(typeof(Poco.Dictionary))]
        public IList<Dictionary> Dictionaries { get; }
    }
}
