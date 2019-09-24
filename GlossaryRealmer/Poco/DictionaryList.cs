//using PropertyChanged;
using Realmer.Util;
using System;
using System.Collections.Generic;

namespace Realmer.Poco
{
    //[AddINotifyPropertyChangedInterface]
    public class DictionaryList : PocoBase<DictionaryList, int>
    {
        public int ApplicationId { get; set; }
        public IReadOnlyList<Dictionary> Dictionaries { get; }

        public DictionaryList() : base(o => o.ApplicationId, () => new Scheme.DictionaryList())
        {
            Dictionaries = AddListProperty(o => o.Dictionaries);
        }
    }
}
