using AutoMapper;
using Realmer.Poco;
using Realmer.Util;
using Realms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

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

        IMapper Mapper { get; set; }

        static string appPath;
        static string filePath;

        public string AppPath { get => appPath; }
        public string FilePath { get => filePath; }

        internal RealmOperator()
        {
            realm = InitRealm();
            Mapper = InitMapper();
            GlossaryMapper.Mapper = Mapper;
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

        #region Update

        public void Add<TPoco>(TPoco newRecord) where TPoco : PocoClass
        {
            realm!.Write(() =>
            {
                var native = newRecord.AdaptClean();
                realm!.Add(native!);
            });
        }

        public void AddRange<TPoco>(IEnumerable<TPoco> newRecords) where TPoco : PocoClass
        {
            realm!.Write(() =>
            {
                foreach (var newRecord in newRecords)
                {
                    var native = newRecord.AdaptClean();
                    realm!.Add(native!);
                }
            });
        }

        public void Update<TPoco>(TPoco record) where TPoco : PocoClass
        {
            var singleList = new TPoco[] { record };
            UpdateRange(singleList);
        }

        public void UpdateRange<TPoco>(IEnumerable<TPoco> records) where TPoco : PocoClass
        {
            var schemeType = SchemeMapper.GetSchemeType<TPoco>();
            var allScheme = realm!.All(schemeType.Name);
            realm!.Write(() =>
            {
                foreach (var record in records)
                {
                    record.AdaptUpdate();
                }
            });
        }

        public void Delete<TPoco>(TPoco record) where TPoco : PocoClass
        {
            var singleList = new TPoco[] { record };
            DeleteRecord(singleList);
        }

        public void Delete<TPoco>(long id) where TPoco : PocoClass
        {
            var singleList = new long[] { id };
            DeleteRecord<TPoco>(singleList);
        }

        public void Delete<TPoco>(int id) where TPoco : PocoClass
        {
            var singleList = new long[] { id };
            DeleteRecord<TPoco>(singleList);
        }

        public void DeleteRange<TPoco>(IEnumerable<TPoco> records) where TPoco : PocoClass
        {
            DeleteRecord(records);
        }

        public void DeleteRange<TPoco>(IEnumerable<long> ids) where TPoco : PocoClass
        {
            DeleteRecord<TPoco>(ids);
        }

        public void DeleteRange<TPoco>(IEnumerable<int> ids) where TPoco : PocoClass
        {
            DeleteRecord<TPoco>(DeleteRecord<TPoco>(ids));
        }

        #endregion

        #region Select

        public IEnumerable<TPoco> SelectAll<TPoco>() where TPoco : PocoClass, new()
        {
            var records = SelectInternal<TPoco, object, object>(null, null, null);
            var result = MapResult<TPoco>(records!);
            return result;
        }

        public IEnumerable<TPoco> Select<TPoco>(Expression<Func<TPoco, bool>> condition) where TPoco : PocoClass, new()
        {
            var records = SelectInternal<TPoco, object, object>(condition, null, null);
            var result = MapResult<TPoco>(records!);
            return result;
        }

        public IEnumerable<TPoco> Select<TPoco, TKey>(Expression<Func<TPoco, TKey>> firstKey, OrderBy firstDirection = OrderBy.Ascending) where TPoco : PocoClass, new()
        {
            var records = SelectInternal<TPoco, TKey, object>(null, firstKey, null, firstDirection);
            var result = MapResult<TPoco>(records!);
            return result;
        }

        public IEnumerable<TPoco> Select<TPoco, TKeyFirst, TKeySecond>(Expression<Func<TPoco, TKeyFirst>> firstKey, Expression<Func<TPoco, TKeySecond>> secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending) where TPoco : PocoClass, new()
        {
            var records = SelectInternal(null, firstKey, secondKey, firstDirection, secondDirection);
            var result = MapResult<TPoco>(records!);
            return result;
        }

        public IEnumerable<TPoco> Select<TPoco, TKey>(Expression<Func<TPoco, bool>> condition, Expression<Func<TPoco, TKey>> firstKey, OrderBy firstDirection = OrderBy.Ascending) where TPoco : PocoClass, new()
        {
            var records = SelectInternal<TPoco, TKey, object>(condition, firstKey, null, firstDirection);
            var result = MapResult<TPoco>(records!);
            return result;
        }

        public IEnumerable<TPoco> Select<TPoco, TKeyFirst, TKeySecond>(Expression<Func<TPoco, bool>> condition, Expression<Func<TPoco, TKeyFirst>> firstKey, Expression<Func<TPoco, TKeySecond>> secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending) where TPoco : PocoClass, new()
        {
            var records = SelectInternal(condition, firstKey, secondKey, firstDirection, secondDirection);
            var result = MapResult<TPoco>(records!);
            return result;
        }

        #endregion //Select

        #region SelectDynamic

        public IEnumerable<dynamic> SelectAll<TPoco>(Expression<Func<TPoco, dynamic>> projection) where TPoco : PocoClass, new()
        {
            var records = SelectInternal<TPoco, object, object>(null, null, null);
            var result = MapResult<TPoco>(records!);
            return result;
        }

        public IEnumerable<dynamic> Select<TPoco>(Expression<Func<TPoco, dynamic>> projection, Expression<Func<TPoco, bool>> condition) where TPoco : PocoClass, new()
        {
            var records = SelectInternal<TPoco, object, object>(condition, null, null, projection: projection);
            var result = MapResult<TPoco>(records!);
            return result;
        }

        public IEnumerable<dynamic> Select<TPoco, TKey>(Expression<Func<TPoco, dynamic>> projection, Expression<Func<TPoco, TKey>> firstKey, OrderBy firstDirection = OrderBy.Ascending) where TPoco : PocoClass, new()
        {
            var records = SelectInternal<TPoco, TKey, object>(null, firstKey, null, firstDirection, projection: projection);
            var result = MapResult<TPoco>(records!);
            return result;
        }

        public IEnumerable<dynamic> Select<TPoco, TKeyFirst, TKeySecond>(Expression<Func<TPoco, dynamic>> projection, Expression<Func<TPoco, TKeyFirst>> firstKey, Expression<Func<TPoco, TKeySecond>> secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending) where TPoco : PocoClass, new()
        {
            var records = SelectInternal(null, firstKey, secondKey, firstDirection, secondDirection, projection: projection);
            var result = MapResult<TPoco>(records!);
            return result;
        }

        public IEnumerable<dynamic> Select<TPoco, TKey>(Expression<Func<TPoco, dynamic>> projection, Expression<Func<TPoco, bool>> condition, Expression<Func<TPoco, TKey>> firstKey, OrderBy firstDirection = OrderBy.Ascending) where TPoco : PocoClass, new()
        {
            var records = SelectInternal<TPoco, TKey, object>(condition, firstKey, null, firstDirection, projection: projection);
            var result = MapResult<TPoco>(records!);
            return result;
        }

        public IEnumerable<dynamic> Select<TPoco, TKeyFirst, TKeySecond>(Expression<Func<TPoco, dynamic>> projection, Expression<Func<TPoco, bool>> condition, Expression<Func<TPoco, TKeyFirst>> firstKey, Expression<Func<TPoco, TKeySecond>> secondKey, OrderBy firstDirection = OrderBy.Ascending, OrderBy secondDirection = OrderBy.Ascending) where TPoco : PocoClass, new()
        {
            var records = SelectInternal(condition, firstKey, secondKey, firstDirection, secondDirection, projection: projection);
            var result = MapResult<TPoco>(records!);
            return result;
        }

        #endregion //SelectDynamic

        #region Private

        #region Prepare

        Realm InitRealm()
        {
            PrepareConfiguration();
            config = ConfigRealm();
            CheckDatabaseFile();

            return Realm.GetInstance(config);
        }

        IMapper InitMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Poco.WordStore, Scheme.WordStore>();
                cfg.CreateMap<Poco.WordList, Scheme.WordList>();
                cfg.CreateMap<Poco.Dictionary, Scheme.Dictionary>();
                cfg.CreateMap<Poco.DictionaryInfo, Scheme.DictionaryInfo>();
                cfg.CreateMap<Poco.DictionaryList, Scheme.DictionaryList>();
                cfg.CreateMap<Poco.Glossary, Scheme.Glossary>();
                cfg.CreateMap<Poco.GlossarySettings, Scheme.GlossarySettings>();
                cfg.CreateMap<Poco.UserList, Scheme.UserList>();
                cfg.CreateMap<Poco.User, Scheme.User>();
                cfg.CreateMap<Poco.PerformanceDictionaryByDictionary, Scheme.PerformanceDictionaryByDictionary>();
                cfg.CreateMap<Poco.DictionaryPerformanceListWordByWord, Scheme.DictionaryPerformanceListWordByWord>();
                cfg.CreateMap<Poco.PerformanceWordByWord, Scheme.PerformanceWordByWord>();

                cfg.CreateMap<Scheme.WordStore, Poco.WordStore>();
                cfg.CreateMap<Scheme.WordList, Poco.WordList>();
                cfg.CreateMap<Scheme.Dictionary, Poco.Dictionary>();
                cfg.CreateMap<Scheme.DictionaryInfo, Poco.DictionaryInfo>();
                cfg.CreateMap<Scheme.DictionaryList, Poco.DictionaryList>();
                cfg.CreateMap<Scheme.Glossary, Poco.Glossary>();
                cfg.CreateMap<Scheme.GlossarySettings, Poco.GlossarySettings>();
                cfg.CreateMap<Scheme.UserList, Poco.UserList>();
                cfg.CreateMap<Scheme.User, Poco.User>();
                cfg.CreateMap<Scheme.PerformanceDictionaryByDictionary, Poco.PerformanceDictionaryByDictionary>();
                cfg.CreateMap<Scheme.DictionaryPerformanceListWordByWord, Poco.DictionaryPerformanceListWordByWord>();
                cfg.CreateMap<Scheme.PerformanceWordByWord, Poco.PerformanceWordByWord>();
            });
            var mapper = new Mapper(config);
            return mapper;
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

                using (var master = assembly.GetManifestResourceStream(RealmerConst.MasterDBEmbeddedPath))
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
            //TODO: Migrate
        }

        #endregion //Prepare

        #region Backup

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:IFormatProvider を指定します", Justification = "<保留中>")]
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

        #region Delete

        void DeleteRecord<TPoco>(IEnumerable<long> ids)
        {
            var schemeType = SchemeMapper.GetSchemeType<TPoco>();
            var targetRecordAll = realm!.All(schemeType.Name);
            realm!.Write(() =>
            {
                foreach (var id in ids)
                {
                    var targetRecord = targetRecordAll.Where(SchemeMapper.GetPKFunc<TPoco>(id)).Single();
                    realm!.Remove(targetRecord);
                }
            });
        }

        IEnumerable<long> DeleteRecord<TPoco>(IEnumerable<int> ids)
        {
            foreach (var id in ids) yield return id;
        }

        void DeleteRecord<TPoco>(IEnumerable<TPoco> records)
        {
            DeleteRecord<TPoco>(SchemeMapper.GetPkEnum(records));
        }

        #endregion

        #region Select

        IEnumerable<dynamic> SelectInternal<TPoco, TFirstKey, TSecondKey>(
            Expression<Func<TPoco, bool>>? condition,
            Expression<Func<TPoco, TFirstKey>>? firstOrderKey,
            Expression<Func<TPoco, TSecondKey>>? secondOrderKey,
            OrderBy firstDirection = OrderBy.Ascending,
            OrderBy secondDirection = OrderBy.Ascending,
            Expression<Func<TPoco, dynamic>>? projection = null
            )
        {
            var scheme = SchemeMapper.GetSchemeType<TPoco>();
            var records = realm!.All(scheme.Name).AsEnumerable();
            if (condition != null) records = records.Where(ExchangeConditionLambda(condition));
            if (firstOrderKey != null)
            {
                records = (IEnumerable<dynamic>)(firstDirection switch
                {
                    OrderBy.Ascending => records.OrderBy(ExchangeOrderLambda(firstOrderKey!)),
                    OrderBy.Descending => records.OrderByDescending(ExchangeOrderLambda(firstOrderKey!)),
                    _ => throw new ArgumentException("Not OrderBy member.", nameof(firstDirection))
                });

                if (secondOrderKey != null)
                {
                    var orderedRecords = records as IOrderedEnumerable<dynamic>;
                    records = (IEnumerable<dynamic>)(secondDirection switch
                    {
                        OrderBy.Ascending => orderedRecords.ThenBy(ExchangeOrderLambda(secondOrderKey!)),
                        OrderBy.Descending => orderedRecords.ThenByDescending(ExchangeOrderLambda(secondOrderKey!)),
                        _ => throw new ArgumentException("Not OrderBy member.", nameof(secondDirection))
                    });
                }
            }
            if (projection != null) records = records.Select(ExchangeProjectionLambda(projection));

            return records;
        }

        IList<TPoco> MapResult<TPoco>(IEnumerable<dynamic> source) where TPoco : PocoClass, new()
        {
            if (!source.Any()) return new List<TPoco>();

            var result = new List<TPoco>();
            foreach (var record in source)
            {
                var pocoObj = new TPoco();
                pocoObj.SetScheme(record);
                result.Add(pocoObj);
            }
            return result;
        }

        #region Expression

        Func<dynamic, bool> ExchangeConditionLambda<TPoco>(Expression<Func<TPoco, bool>> condition)
        {
            return ExchangeLambda(condition);
        }

        Func<dynamic, TOrderKey> ExchangeOrderLambda<TPoco, TOrderKey>(Expression<Func<TPoco, TOrderKey>> order)
        {
            return ExchangeLambda(order);
        }

        Func<dynamic, dynamic> ExchangeProjectionLambda<TPoco>(Expression<Func<TPoco, dynamic>> projection)
        {
            return ExchangeLambda(projection);
        }

        Func<dynamic, TResult> ExchangeLambda<TPoco, TResult>(Expression<Func<TPoco, TResult>> originalLambda)
        {
            var converter = new ConvertLambdaPoco2Scheme<TResult>(SchemeMapper.GetSchemeType<TPoco>());
            var schemeExpression = (LambdaExpression)converter.Visit(originalLambda);
            var schemeLambda = (Func<dynamic, TResult>)schemeExpression.Compile();
            return schemeLambda;
        }

        #endregion //Expression

        #endregion //Select

        #endregion //Transaction

        #endregion //Private
    }
}
