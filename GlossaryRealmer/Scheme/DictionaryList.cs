using AutoMapper.Configuration.Annotations;
using Realms;
using System.Collections.Generic;

namespace Realmer.Scheme
{
    internal class DictionaryList : RealmObject
    {
        internal int PK => ApplicationId;

        [PrimaryKey]
        public int ApplicationId { get; set; }
        [Ignore]
        public IList<Dictionary> Dictionaries { get; }
    }
}
