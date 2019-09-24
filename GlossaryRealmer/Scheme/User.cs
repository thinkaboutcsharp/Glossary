using System;
using Realms;

namespace Realmer.Scheme
{
    internal class User : RealmObject
    {
        internal int PK => UserId;

        [PrimaryKey]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public PerformanceDictionaryByDictionary PerformanceList { get; set; }
    }

    internal class UserCounter : RealmObject
    {
        public RealmInteger<int> Count { get; set; }
    }
}
