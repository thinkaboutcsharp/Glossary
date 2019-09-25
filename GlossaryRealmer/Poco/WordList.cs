using AutoMapper.Configuration.Annotations;
//using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Realmer.Poco
{
    //[AddINotifyPropertyChangedInterface]
    public sealed class WordList : Util.PocoBase<WordList, int>
    {
        public int DictionaryId { get; set; }
        [Ignore]
        public IReadOnlyList<WordStore> Words { get; }

        public WordList() : base(o => o.DictionaryId, () => new Scheme.WordList())
        {
            Words = AddListProperty<WordStore, Scheme.WordList, Scheme.WordStore>(o => o.Words);
        }
    }
}
