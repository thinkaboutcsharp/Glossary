using System;
using System.Collections.Generic;
using Realms;

namespace Realmer.Scheme
{
    public class Glossary : RealmObject
    {
        internal int PK => ApplicationId;

        [PrimaryKey]
        public int ApplicationId { get; set; }
        public GlossarySettings Settings { get; set; }

        [Util.PocoClass(typeof(Poco.User))]
        public IList<User> Users { get; }
    }
}
