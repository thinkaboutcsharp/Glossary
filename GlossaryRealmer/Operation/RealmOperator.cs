using AutoMapper;
using Realmer.Scheme;
using Realmer.Util;
using Realms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        static RealmConfiguration config;

        static string appPath;
        static string filePath;

        public string AppPath { get => appPath; }
        public string FilePath { get => filePath; }

        Mapper mapper;

        internal RealmOperator()
        {
            realm = InitRealm();
            mapper = CreateMap();
        }

        public void Dispose()
        {
            Close();
        }

        public void Open()
        {
            Close();
            PrepareConfiguration();
            CheckDatabaseFile();
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

        public void Add<TPoco>(TPoco newRecord)
        {
            realm!.Write(() =>
            {
                var native = mapper.Map(newRecord, typeof(TPoco), SchemeMapper.GetSchemeType<TPoco>()) as RealmObject;
                realm!.Add(native!);
            });
        }

        public void AddRange<TPoco>(IEnumerable<TPoco> newRecords)
        {
            realm!.Write(() =>
            {
                foreach (var newRecord in newRecords)
                {
                    var native = mapper.Map(newRecord, typeof(TPoco), SchemeMapper.GetSchemeType<TPoco>()) as RealmObject;
                    realm!.Add(native!);
                }
            });
        }

        public void Update<TPoco>(TPoco record)
        {
            var singleList = new TPoco[] { record };
            UpdateRange(singleList);
        }

        public void UpdateRange<TPoco>(IEnumerable<TPoco> records)
        {
            var schemeType = SchemeMapper.GetSchemeType<TPoco>();
            var allScheme = realm!.All(schemeType.Name);
            realm!.Write(() =>
            {
                foreach (var record in records)
                {
                    var targetRecord = allScheme.Where(SchemeMapper.GetPKFunc(record)).Single();
                    mapper.Map(record, targetRecord, typeof(TPoco), schemeType);
                }
            });
        }

        public void Delete<TPoco>(TPoco record)
        {
            var singleList = new TPoco[] { record };
            DeleteRecord(singleList);
        }

        public void Delete<TPoco>(long id)
        {
            var singleList = new long[] { id };
            DeleteRecord<TPoco>(singleList);
        }

        public void DeleteRange<TPoco>(IEnumerable<TPoco> records)
        {
            DeleteRecord(records);
        }

        public void DeleteRange<TPoco>(IEnumerable<long> ids)
        {
            DeleteRecord<TPoco>(ids);
        }

        #region Select

        public IEnumerable<TPoco> SelectAll<TPoco>()
        {
            var records = SelectInternal<TPoco, object, object>(null, null, null);
            var result = MapResult<TPoco>(records!);
            return result;
        }

        public IEnumerable<TPoco> Select<TPoco>(Func<dynamic, bool> condition)
        {
            var records = SelectInternal<TPoco, object, object>(condition, null, null);
            var result = MapResult<TPoco>(records!);
            return result;
        }

        public IEnumerable<TPoco> Select<TPoco, TKey>(Func<dynamic, TKey> firstKey, OrderBy firstDirection = OrderBy.Ascending)
        {
            var records = SelectInternal<TPoco, TKey, object>(null, firstKey, null, firstDirection);
            var result = MapResult<TPoco>(records!);
            return result;
        }

        public IEnumerable<TPoco> Select<TPoco, TKeyFirst, TKeySecond>(Func<dynamic, TKeyFirst> firstKey, Func<dynamic, TKeySecond> secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending)
        {
            var records = SelectInternal<TPoco, TKeyFirst, TKeySecond>(null, firstKey, secondKey, firstDirection, secondDirection);
            var result = MapResult<TPoco>(records!);
            return result;
        }

        public IEnumerable<TPoco> Select<TPoco, TKey>(Func<dynamic, bool> condition, Func<dynamic, TKey> firstKey, OrderBy firstDirection = OrderBy.Ascending)
        {
            var records = SelectInternal<TPoco, TKey, object>(condition, firstKey, null, firstDirection);
            var result = MapResult<TPoco>(records!);
            return result;
        }

        public IEnumerable<TPoco> Select<TPoco, TKeyFirst, TKeySecond>(Func<dynamic, bool> condition, Func<dynamic, TKeyFirst> firstKey, Func<dynamic, TKeySecond> secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending)
        {
            var records = SelectInternal<TPoco, TKeyFirst, TKeySecond>(condition, firstKey, secondKey, firstDirection, secondDirection);
            var result = MapResult<TPoco>(records!);
            return result;
        }

        #endregion

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
            File.Copy(filePath, backupPath);
        }

        #endregion //Backup

        #region Transaction

        void DeleteRecord<TPoco>(IEnumerable<long> records)
        {
            var schemeType = SchemeMapper.GetSchemeType<TPoco>();
            var targetRecordAll = realm!.All(schemeType.Name);
            realm!.Write(() =>
            {
                foreach (var key in records)
                {
                    var targetRecord = targetRecordAll.Where(SchemeMapper.GetPKFunc<TPoco>(key)).Single();
                    realm!.Remove(targetRecord);
                }
            });
        }

        void DeleteRecord<TPoco>(IEnumerable<TPoco> records)
        {
            DeleteRecord<TPoco>(SchemeMapper.GetPkEnum(records));
        }

        IEnumerable<dynamic> SelectInternal<TPoco, TFirstKey, TSecondKey>(
            Func<dynamic, bool>? condition,
            Func<dynamic, TFirstKey>? firstOrderKey,
            Func<dynamic, TSecondKey>? secondOrderKey,
            OrderBy firstDirection = OrderBy.Ascending,
            OrderBy secondDirection = OrderBy.Ascending
            )
        {
            var scheme = SchemeMapper.GetSchemeType<TPoco>();
            var records = realm!.All(scheme.Name).AsEnumerable();
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

            return records.AsEnumerable();
        }

        IList<TPoco> MapResult<TPoco>(IEnumerable<dynamic> source)
        {
            if (source.Count() == 0) return new List<TPoco>();

            var sourceType = source.First().GetType();
            var ctor = typeof(TPoco).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { sourceType }, null);

            var result = new List<TPoco>();
            foreach (var record in source)
            {
                var pocoObj = (TPoco)ctor.Invoke(new object[] { record });
                result.Add(pocoObj);
            }
            return result;
        }

        #endregion //Transaction

        #endregion //Private
    }
}
