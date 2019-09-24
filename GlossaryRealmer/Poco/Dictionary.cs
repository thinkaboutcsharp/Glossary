//using PropertyChanged;
using Realmer.Poco;
using System;

namespace Realmer.Poco
{
    //[AddINotifyPropertyChangedInterface]
    public sealed class Dictionary : Util.PocoBase<Dictionary, int>
    {
        public int DictionaryId { get; set; }
        public WordList WordList { get; set; }
        public DictionaryInfo Info { get; set; }

        public Dictionary() : base(o => o.DictionaryId, () => new Scheme.Dictionary())
        {
        }
    }
}
