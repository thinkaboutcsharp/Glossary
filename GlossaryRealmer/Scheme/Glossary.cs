using System;
using System.Collections.Generic;
using Realms;

namespace Realmer.Scheme
{
    internal class Glossary : RealmObject
    {
        internal int PK => ApplicationId;

        [PrimaryKey]
        public int ApplicationId { get; set; }
        public GlossarySettings Settings { get; set; }
        public DictionaryList Dictionaries { get; set; }
        public UserList Users { get; set; }
    }
}
