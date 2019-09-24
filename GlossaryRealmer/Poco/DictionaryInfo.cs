//using PropertyChanged;

namespace Realmer.Poco
{
    //[AddINotifyPropertyChangedInterface]
    public sealed class DictionaryInfo : Util.PocoBase<DictionaryInfo, int>
    {
        public int DictionaryId { get; set; }

        public DictionaryInfo() : base(o => o.DictionaryId, () => new Scheme.DictionaryInfo())
        {
        }
    }
}
