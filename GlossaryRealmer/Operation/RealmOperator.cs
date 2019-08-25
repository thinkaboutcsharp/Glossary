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
using System.Runtime.CompilerServices;

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
            realm!.Write(() =>
            {
                var native = mapper.Map(newRecord, typeof(TPoco), SchemeMapper.GetSchemeType<TPoco>()) as RealmObject;
                AddRecord(realm!, native!);
            });
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
            await realm!.WriteAsync(asyncRealm =>
            {
                var native = mapper.Map(newRecord, typeof(TPoco), SchemeMapper.GetSchemeType<TPoco>()) as RealmObject;
                AddRecord(asyncRealm, native!);
            });
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

        public IEnumerable<TPoco> SelectAll<TPoco>(IList<TPoco> destination)
        {
            var records = SelectInternal<TPoco, object, object>(null, null, null);
            MapResult(destination, records!);
            return destination;
        }

        public IEnumerable<TPoco> Select<TPoco>(IList<TPoco> destination, Func<dynamic, bool> condition)
        {
            var records = SelectInternal<TPoco, object, object>(condition, null, null);
            MapResult(destination, records!);
            return destination;
        }

        public IEnumerable<TPoco> Select<TPoco, TKey>(IList<TPoco> destination, Func<dynamic, TKey> firstKey, OrderBy firstDirection = OrderBy.Ascending)
        {
            var records = SelectInternal<TPoco, TKey, object>(null, firstKey, null, firstDirection);
            MapResult(destination, records!);
            return destination;
        }

        public IEnumerable<TPoco> Select<TPoco, TKeyFirst, TKeySecond>(IList<TPoco> destination, Func<dynamic, TKeyFirst> firstKey, Func<dynamic, TKeySecond> secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending)
        {
            var records = SelectInternal<TPoco, TKeyFirst, TKeySecond>(null, firstKey, secondKey, firstDirection, secondDirection);
            MapResult(destination, records!);
            return destination;
        }

        public IEnumerable<TPoco> Select<TPoco, TKey>(IList<TPoco> destination, Func<dynamic, bool> condition, Func<dynamic, TKey> firstKey, OrderBy firstDirection = OrderBy.Ascending)
        {
            var records = SelectInternal<TPoco, TKey, object>(condition, firstKey, null, firstDirection);
            MapResult(destination, records!);
            return destination;
        }

        public IEnumerable<TPoco> Select<TPoco, TKeyFirst, TKeySecond>(IList<TPoco> destination, Func<dynamic, bool> condition, Func<dynamic, TKeyFirst> firstKey, Func<dynamic, TKeySecond> secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending)
        {
            var records = SelectInternal<TPoco, TKeyFirst, TKeySecond>(condition, firstKey, secondKey, firstDirection, secondDirection);
            MapResult(destination, records!);
            return destination;
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

        IEnumerable<dynamic> SelectInternal<TPoco, TFirstKey, TSecondKey>(
            Func<dynamic, bool>? condition,
            Func<dynamic, TFirstKey>? firstOrderKey,
            Func<dynamic, TSecondKey>? secondOrderKey,
            OrderBy firstDirection = OrderBy.Ascending,
            OrderBy secondDirection = OrderBy.Ascending
            )
        {
            var scheme = SchemeMapper.GetSchemeType<TPoco>();
            var records = realm!.All(scheme.Name).AsEnumerable() as IEnumerable<dynamic>;
            if (condition != null) records = records.Where(condition);
            if (firstOrderKey != null)
            {
                records = firstDirection switch
                {
                    OrderBy.Ascending => records.OrderBy(firstOrderKey!),
                    OrderBy.Descending => records.OrderByDescending(firstOrderKey!),
                    _ => throw new ArgumentException("Not OrderBy member.", nameof(firstDirection))
                };

                if (secondOrderKey != null)
                {
                    var orderedRecords = records as IOrderedEnumerable<dynamic>;
                    records = secondDirection switch
                    {
                        OrderBy.Ascending => orderedRecords.ThenBy(secondOrderKey!),
                        OrderBy.Descending => orderedRecords.ThenByDescending(secondOrderKey),
                        _ => throw new ArgumentException("Not OrderBy member.", nameof(secondDirection))
                    };
                }
            }

            return records;
        }

        void MapResult<TPoco>(IList<TPoco> destination, IEnumerable<dynamic> source)
        {
            destination.Clear();
            if (source.Count() == 0) return;

            var sourceType = source.First().GetType();
            var cons = typeof(TPoco).GetConstructor(new Type[] { sourceType });
            foreach (var record in source)
            {
                var pocoObj = (TPoco)cons.Invoke(new object[] { record });
                destination.Add(pocoObj);
            }
        }

        #endregion //Transaction

        #endregion //Private
    }
}
