using Realms;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Realmer.Util;
using System.Linq.Expressions;

namespace Realmer
{
    public interface IGlossaryRealmer : IDisposable
    {
        string AppPath { get; }
        string FilePath { get; }

        void Open();
        void Close();
        void Backup(string key);

        void Add<TPoco>(TPoco newRecord) where TPoco : PocoClass;
        void AddRange<TPoco>(IEnumerable<TPoco> newRecords) where TPoco : PocoClass;
        void Update<TPoco>(TPoco record) where TPoco : PocoClass;
        void UpdateRange<TPoco>(IEnumerable<TPoco> records) where TPoco : PocoClass;
        void Delete<TPoco>(TPoco record) where TPoco : PocoClass;
        void Delete<TPoco>(long id) where TPoco : PocoClass;
        void Delete<TPoco>(int id) where TPoco : PocoClass;
        void DeleteRange<TPoco>(IEnumerable<TPoco> records) where TPoco : PocoClass;
        void DeleteRange<TPoco>(IEnumerable<long> ids) where TPoco : PocoClass;
        void DeleteRange<TPoco>(IEnumerable<int> ids) where TPoco : PocoClass;

        //Async write has risk, throws 'Realm is accessed by incorrect thread', so sealed.
        //Task AddAsync<TPoco>(TPoco newRecord);
        //Task AddRangeAsync<TPoco>(IList<TPoco> newRecords);

        IEnumerable<TPoco> SelectAll<TPoco>() where TPoco : PocoClass, new();
        IEnumerable<TPoco> Select<TPoco>(Expression<Func<TPoco, bool>> condition) where TPoco : PocoClass, new();
        IEnumerable<TPoco> Select<TPoco, TKey>(Expression<Func<TPoco, TKey>> firstKey, OrderBy firstDirection = OrderBy.Ascending) where TPoco : PocoClass, new();
        IEnumerable<TPoco> Select<TPoco, TKeyFirst, TKeySecond>(Expression<Func<TPoco, TKeyFirst>> firstKey, Expression<Func<TPoco, TKeySecond>> secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending) where TPoco : PocoClass, new();
        IEnumerable<TPoco> Select<TPoco, TKey>(Expression<Func<TPoco, bool>> condition, Expression<Func<TPoco, TKey>> firstKey, OrderBy firstDirection = OrderBy.Ascending) where TPoco : PocoClass, new();
        IEnumerable<TPoco> Select<TPoco, TKeyFirst, TKeySecond>(Expression<Func<TPoco, bool>> condition, Expression<Func<TPoco, TKeyFirst>> firstKey, Expression<Func<TPoco, TKeySecond>> secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending) where TPoco : PocoClass, new();
    }

    public enum OrderBy
    {
        Ascending,
        Descending
    }
}
