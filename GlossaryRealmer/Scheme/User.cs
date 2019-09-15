using System;
using Realms;

namespace Realmer.Scheme
{
    public class User : RealmObject
    {
        internal int PK => UserId;

        [PrimaryKey]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public PerformanceDictionaryByDictionary PerformanceList { get; set; }
    }

    public class UserCounter : RealmObject
    {
        public RealmInteger<int> Count { get; set; }
    }
}
