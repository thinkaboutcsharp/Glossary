using System;
using System.IO;
using System.Net.Http.Headers;
using AutoMapper;
using Realmer.Scheme;
using Realms;
using Scheme = Realmer.Scheme;
using Poco = Realmer.Poco;
using System.Collections.Generic;
using System.Threading;
using Realmer.Util;
using System.Linq;
using System.Threading.Tasks;

namespace Realmer.Operation
{
    class RealmOperator : IGlossaryRealmer
    {
        const string ApplicationFolder = "Glossary@TACFilozofio";
        const string DatabaseFileName = "glossary@tacfilozofio.realm";
        const ulong SchemaVersion = 0;

        const string BackupKeyFormat = "{0}_@{1}@_";

        Realm? realm;
        RealmConfiguration config;
        string appPath;
        string filePath;

        Mapper mapper;

        Action disposeAction;

        internal RealmOperator(Action disposeAction)
        {
            this.disposeAction = disposeAction;
            realm = InitRealm();
            mapper = CreateMap();
        }

        public void Dispose()
        {
            Close();
            disposeAction();
        }

        public void Open()
        {
            Close();
            realm = Realm.GetInstance(config);
        }

        public void Close()
        {
            realm?.Dispose();
            realm = null;
        }

        public void Backup(string key)
        {
            if (realm != null) throw new InvalidOperationException("Realm is open. Can't do backup.");

            var backupPath = GetBackupPath(key);
            CopyToBackup(backupPath);
        }

        public void Uninstall()
        {
            Close();

            Realm.DeleteRealm(config);
            foreach (var file in Directory.EnumerateFiles(appPath)) File.Delete(file);
            Directory.Delete(appPath);
        }

        public void Add<TPoco>(TPoco newRecord)
        {
            var native = mapper.Map(newRecord, typeof(TPoco), SchemeMapper.GetSchemeType<TPoco>()) as RealmObject;
            realm!.Write(() => AddRecord(realm!, native!));
        }

        public void AddRange<TPoco>(IEnumerable<TPoco> newRecords)
        {
            realm!.Write(() =>
            {
                foreach (var newRecord in newRecords)
                {
                    var native = mapper.Map(newRecord, typeof(TPoco), SchemeMapper.GetSchemeType<TPoco>()) as RealmObject;
                    AddRecord(realm!, native!);
                }
            });
        }

        public async Task AddAsync<TPoco>(TPoco newRecord)
        {
            var native = mapper.Map(newRecord, typeof(TPoco), SchemeMapper.GetSchemeType<TPoco>()) as RealmObject;
            await realm!.WriteAsync(asyncRealm => AddRecord(asyncRealm, native!));
        }

        public async Task AddRangeAsync<TPoco>(IEnumerable<TPoco> newRecords)
        {
            await realm!.WriteAsync(asyncRealm =>
            {
                foreach (var newRecord in newRecords)
                {
                    var native = mapper.Map(newRecord, typeof(TPoco), SchemeMapper.GetSchemeType<TPoco>()) as RealmObject;
                    AddRecord(asyncRealm, native!);
                }
            });
        }

        public IEnumerable<TPoco> SelectAll<TPoco>()
        {
            var records = SelectInternal<TPoco, object, object>(null, null, null);
            var result = MapResult<TPoco>(records);
            return result;
        }

        public IEnumerable<TPoco> Select<TPoco>(Func<TPoco, bool> condition)
        {
            var records = SelectInternal<TPoco, object, object>(condition, null, null);
            var result = MapResult<TPoco>(records);
            return result;
        }

        public IEnumerable<TPoco> Select<TPoco, TKey>(Func<TPoco, TKey> firstKey, OrderBy firstDirection = OrderBy.Ascending)
        {
            var records = SelectInternal<TPoco, TKey, object>(null, firstKey, null, firstDirection: firstDirection);
            var result = MapResult<TPoco>(records);
            return result;
        }

        public IEnumerable<TPoco> Select<TPoco, TKeyFirst, TKeySecond>(Func<TPoco, TKeyFirst> firstKey, Func<TPoco, TKeySecond> secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending)
        {
            var records = SelectInternal<TPoco, TKeyFirst, TKeySecond>(null, firstKey, secondKey, firstDirection, secondDirection);
            var result = MapResult<TPoco>(records);
            return result;
        }

        public IEnumerable<TPoco> Select<TPoco, TKey>(Func<TPoco, bool> condition, Func<TPoco, TKey> firstKey, OrderBy firstDirection = OrderBy.Ascending)
        {
            var records = SelectInternal<TPoco, TKey, object>(condition, firstKey, null, firstDirection: firstDirection);
            var result = MapResult<TPoco>(records);
            return result;
        }

        public IEnumerable<TPoco> Select<TPoco, TKeyFirst, TKeySecond>(Func<TPoco, bool> condition, Func<TPoco, TKeyFirst> firstKey, Func<TPoco, TKeySecond> secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending)
        {
            var records = SelectInternal<TPoco, TKeyFirst, TKeySecond>(condition, firstKey, secondKey, firstDirection, secondDirection);
            var result = MapResult<TPoco>(records);
            return result;
        }


        #region Private

        #region Prepare

        Realm InitRealm()
        {
            PrepareConfiguration();
            config = ConfigRealm();
            CheckDatabaseFile();

            return Realm.GetInstance(config);
        }

        void PrepareConfiguration()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            appPath = Path.Combine(folder, ApplicationFolder);

            if (!Directory.Exists(appPath)) Directory.CreateDirectory(appPath);
        }

        RealmConfiguration ConfigRealm()
        {
            filePath = Path.Combine(appPath, DatabaseFileName);

            return new RealmConfiguration(filePath)
            {
                SchemaVersion = SchemaVersion,
                ShouldCompactOnLaunch = (totalSize, dataSize) => ((double)(totalSize - dataSize) / dataSize > 1.2),
                MigrationCallback = MigrateRealm
            };
        }

        void CheckDatabaseFile()
        {
            if (!File.Exists(filePath))
            {
                var assembly = this.GetType().Assembly;

                using (var master = assembly.GetManifestResourceStream("Realmer.MasterDB.master.realm"))
                {
                    CopyOriginalDatabase(master, filePath);
                }
            }
        }

        void CopyOriginalDatabase(Stream originStream, string copyPath)
        {
            using (var db = File.Create(copyPath))
            {
                originStream.CopyToAsync(db).Wait();
                db.FlushAsync().Wait();
            }
        }

        void MigrateRealm(Migration migration, ulong oldSchemaVersion)
        {
            throw new NotImplementedException();
        }

        Mapper CreateMap()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap(typeof(Scheme.WordStore), typeof(Poco.WordStore));
                cfg.CreateMap(typeof(Poco.WordStore), typeof(Scheme.WordStore));
            });
            var mapper = new Mapper(config);
            return mapper;
        }

        #endregion //Prepare

        #region Backup

        string GetBackupPath(string key)
        {
            var path = string.Format(BackupKeyFormat, filePath, key);
            return path;
        }

        void CopyToBackup(string backupPath)
        {
            using (var current = new FileStream(filePath, FileMode.Open))
            {
                CopyOriginalDatabase(current, backupPath);
            }
        }

        #endregion //Backup

        #region Transaction

        void AddRecord(Realm transactionRealm, RealmObject newRecord) => transactionRealm.Add(newRecord);

        IEnumerable<dynamic> SelectInternal<TPoco, TKeyFirst, TKeySecond>(Func<TPoco, bool>? condition, Func<TPoco, TKeyFirst>? firstKey, Func<TPoco, TKeySecond>? secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending)
        {
            var scheme = SchemeMapper.GetSchemeType<TPoco>();
            var records = realm!.All(scheme.Name).AsEnumerable();
            if (condition != null)
            {
                records = records.Where(condition as Func<dynamic, bool>);
            }
            if (firstKey != null)
            {
                records = OrderResult(records, (firstKey as Func<dynamic, TKeyFirst>)!, firstDirection);
            
                if (secondKey != null)
                {
                    records = OrderResult(records, (secondKey as Func<dynamic, TKeySecond>)!, secondDirection);
                }
            }
            return records;
        }

        IEnumerable<dynamic> OrderResult<TKey>(IEnumerable<dynamic> records, Func<dynamic, TKey> key, OrderBy orderBy)
        {
            switch (orderBy)
            {
                case OrderBy.Ascending:
                    return records.OrderBy(key);
                case OrderBy.Descending:
                    return records.OrderByDescending(key);
                default:
                    return records;
            }
        }

        IList<TPoco> MapResult<TPoco>(IEnumerable<dynamic> source)
        {
            var result = new List<TPoco>();
            foreach (var record in source)
            {
                result.Add(mapper.Map<TPoco>(record));
            }
            return result;
        }

        #endregion //Transaction

        #endregion //Private
    }
}
