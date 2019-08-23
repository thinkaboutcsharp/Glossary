using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Realmer
{
    public interface IGlossaryRealmer : IDisposable
    {
        void Open();
        void Close();
        void Backup(string key);
        void Uninstall();

        void Add<TPoco>(TPoco newRecord);
        void AddRange<TPoco>(IEnumerable<TPoco> newRecords);
        Task AddAsync<TPoco>(TPoco newRecord);
        Task AddRangeAsync<TPoco>(IEnumerable<TPoco> newRecords);
        IEnumerable<TPoco> SelectAll<TPoco>();
        IEnumerable<TPoco> Select<TPoco>(Func<TPoco, bool> condition);
        IEnumerable<TPoco> Select<TPoco, TKey>(Func<TPoco, TKey> firstKey, OrderBy firstDirection = OrderBy.Ascending);
        IEnumerable<TPoco> Select<TPoco, TKeyFirst, TKeySecond>(Func<TPoco, TKeyFirst> firstKey, Func<TPoco, TKeySecond> secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending);
        IEnumerable<TPoco> Select<TPoco, TKey>(Func<TPoco, bool> condition, Func<TPoco, TKey> firstKey, OrderBy firstDirection = OrderBy.Ascending);
        IEnumerable<TPoco> Select<TPoco, TKeyFirst, TKeySecond>(Func<TPoco, bool> condition, Func<TPoco, TKeyFirst> firstKey, Func<TPoco, TKeySecond> secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending);
    }

    public enum OrderBy
    {
        Ascending,
        Descending
    }
}
