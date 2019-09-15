using System;
using System.Collections.Generic;
using System.Text;
using Realms;

namespace Realmer.Scheme
{
    public class GlossarySettings : RealmObject
    {
        internal int PK => ApplicationId;

        [PrimaryKey]
        public int ApplicationId { get; set; }
        // color, icon, etc...
    }
}
