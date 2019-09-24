//using PropertyChanged;
using Realmer.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Realmer.Poco
{
    //[AddINotifyPropertyChangedInterface]
    public class User : PocoBase<User, int>
    {
        public int UserId { get; }
        public string UserName { get; }
        public PerformanceDictionaryByDictionary PerformanceList { get; }

        public User() : base(o => o.UserId, () => new Scheme.User())
        {
        }
    }
}
