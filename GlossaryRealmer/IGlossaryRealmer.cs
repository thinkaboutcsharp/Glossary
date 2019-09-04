using Realms;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Realmer
{
    public interface IGlossaryRealmer : IDisposable
    {
        string AppPath { get; }
        string FilePath { get; }

        void Open();
        void Close();
        void Backup(string key);

        void Add<TPoco>(TPoco newRecord);
        void AddRange<TPoco>(IList<TPoco> newRecords);
        void Update<TPoco>(TPoco record);
        void UpdateRange<TPoco>(IList<TPoco> records);
        void Delete<TPoco>(TPoco record);
        void Delete<TPoco>(long id);
        void DeleteRange<TPoco>(IList<TPoco> records);
        void DeleteRange<TPoco>(IList<long> ids);

        //Async write has risk, throws 'Realm is accessed by incorrect thread', so sealed.
        //Task AddAsync<TPoco>(TPoco newRecord);
        //Task AddRangeAsync<TPoco>(IList<TPoco> newRecords);

        IEnumerable<TPoco> SelectAll<TPoco>();
        IEnumerable<TPoco> Select<TPoco>(Func<dynamic, bool> condition);
        IEnumerable<TPoco> Select<TPoco, TKey>(Func<dynamic, TKey> firstKey, OrderBy firstDirection = OrderBy.Ascending);
        IEnumerable<TPoco> Select<TPoco, TKeyFirst, TKeySecond>(Func<dynamic, TKeyFirst> firstKey, Func<dynamic, TKeySecond> secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending);
        IEnumerable<TPoco> Select<TPoco, TKey>(Func<dynamic, bool> condition, Func<dynamic, TKey> firstKey, OrderBy firstDirection = OrderBy.Ascending);
        IEnumerable<TPoco> Select<TPoco, TKeyFirst, TKeySecond>(Func<dynamic, bool> condition, Func<dynamic, TKeyFirst> firstKey, Func<dynamic, TKeySecond> secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending);
    }

    public enum OrderBy
    {
        Ascending,
        Descending
    }
}
