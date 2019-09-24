//using PropertyChanged;
using Realmer.Util;
using System;
using System.Collections.Generic;

namespace Realmer.Poco
{
    //[AddINotifyPropertyChangedInterface]
    public class Glossary : PocoBase<Glossary, int>
    {
        public int ApplicationId { get; set; }
        public GlossarySettings Settings { get; set; }
        public DictionaryList Dictionaries { get; set; }
        public UserList Users { get; set; }

        public Glossary() : base(o => o.ApplicationId, () => new Scheme.Glossary())
        {
        }
    }
}
