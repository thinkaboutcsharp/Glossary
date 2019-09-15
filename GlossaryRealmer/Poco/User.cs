using System;
using System.Collections.Generic;
using System.Text;

namespace Realmer.Poco
{
    public readonly struct User
    {
        internal int PK => UserId;

        public int UserId { get; }
        public string UserName { get; }
        public PerformanceDictionaryByDictionary PerformanceList { get; }

        public User(int userId, string userName, PerformanceDictionaryByDictionary performanceList)
        {
            UserId = userId;
            UserName = userName;
            PerformanceList = performanceList;
        }

        internal User(dynamic carrier)
            : this((int)carrier.UserId, (string)carrier.UserName, (PerformanceDictionaryByDictionary)carrier.PerformanceList)
        { }
    }
}
