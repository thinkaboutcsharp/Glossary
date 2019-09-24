//using PropertyChanged;

namespace Realmer.Poco
{
    //[AddINotifyPropertyChangedInterface]
    public sealed class WordStore : Util.PocoBase<WordStore, long>
    {
        public long WordId { get; set; }
        public int DictionaryId { get; set; }
        public string Word { get; set; }

        public WordStore() : base(o => o.WordId, () => new Scheme.WordStore())
        {
        }
    }
}
