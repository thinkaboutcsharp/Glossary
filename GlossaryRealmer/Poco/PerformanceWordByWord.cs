//using PropertyChanged;
using Realmer.Util;
using System;

namespace Realmer.Poco
{
    //[AddINotifyPropertyChangedInterface]
    public class PerformanceWordByWord : PocoBase<PerformanceWordByWord, long>
    {
        public long WordId { get; }
        public int TestCount { get; }
        public int CorrectCount { get; }

        public PerformanceWordByWord() : base(o => o.WordId, () => new Scheme.PerformanceWordByWord())
        {
        }
    }
}
