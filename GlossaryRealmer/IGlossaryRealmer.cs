using Realms;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Realmer
{
    public interface IGlossaryRealmer : IDisposable
    {
        void Open();
        void Close();
        void Backup(string key);

        void Add<TPoco>(TPoco newRecord);
        void AddRange<TPoco>(IEnumerable<TPoco> newRecords);
        Task AddAsync<TPoco>(TPoco newRecord);
        Task AddRangeAsync<TPoco>(IEnumerable<TPoco> newRecords);
        IEnumerable<TPoco> SelectAll<TPoco>(IList<TPoco> destination);
        IEnumerable<TPoco> Select<TPoco>(IList<TPoco> destination, Func<dynamic, bool> condition);
        IEnumerable<TPoco> Select<TPoco, TKey>(IList<TPoco> destination, Func<dynamic, TKey> firstKey, OrderBy firstDirection = OrderBy.Ascending);
        IEnumerable<TPoco> Select<TPoco, TKeyFirst, TKeySecond>(IList<TPoco> destination, Func<dynamic, TKeyFirst> firstKey, Func<dynamic, TKeySecond> secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending);
        IEnumerable<TPoco> Select<TPoco, TKey>(IList<TPoco> destination, Func<dynamic, bool> condition, Func<dynamic, TKey> firstKey, OrderBy firstDirection = OrderBy.Ascending);
        IEnumerable<TPoco> Select<TPoco, TKeyFirst, TKeySecond>(IList<TPoco> destination, Func<dynamic, bool> condition, Func<dynamic, TKeyFirst> firstKey, Func<dynamic, TKeySecond> secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending);
    }

    public enum OrderBy
    {
        Ascending,
        Descending
    }
}
