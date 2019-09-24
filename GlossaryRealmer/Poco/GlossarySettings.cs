//using PropertyChanged;
using Realmer.Util;
using System;

namespace Realmer.Poco
{
    //[AddINotifyPropertyChangedInterface]
    public class GlossarySettings : PocoBase<GlossarySettings, int>
    {
        public int ApplicationId { get; }

        public GlossarySettings() : base(o => o.ApplicationId, () => new Scheme.GlossarySettings())
        {
        }
    }
}
