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

        IEnumerable<RealmObject> SelectInternal<TPoco, TFirstKey, TSecondKey>(
            Expression<Func<TPoco, bool>>? condition,
            Expression<Func<TPoco, TFirstKey>>? firstOrderKey,
            Expression<Func<TPoco, TSecondKey>>? secondOrderKey,
            OrderBy firstDirection = OrderBy.Ascending,
            OrderBy secondDirection = OrderBy.Ascending
            )
        {
            var scheme = SchemeMapper.GetSchemeType<TPoco>();
            var records = realm!.All(scheme.Name);
            if (condition != null) records = records.Where(ExchangeConditionLambda(condition)).AsQueryable();
            if (firstOrderKey != null)
            {
                records = firstDirection switch
                {
                    OrderBy.Ascending => records.OrderBy(ExchangeOrderLambda(firstOrderKey!)).AsQueryable(),
                    OrderBy.Descending => records.OrderByDescending(ExchangeOrderLambda(firstOrderKey!)).AsQueryable(),
                    _ => throw new ArgumentException("Not OrderBy member.", nameof(firstDirection))
                };

                if (secondOrderKey != null)
                {
                    var orderedRecords = records as IOrderedEnumerable<dynamic>;
                    records = (IQueryable<dynamic>)(secondDirection switch
                    {
                        OrderBy.Ascending => orderedRecords.ThenBy(ExchangeOrderLambda(secondOrderKey!)),
                        OrderBy.Descending => orderedRecords.ThenByDescending(ExchangeOrderLambda(secondOrderKey!)),
                        _ => throw new ArgumentException("Not OrderBy member.", nameof(secondDirection))
                    });
                }
            }

            return (IEnumerable<RealmObject>)records.AsEnumerable();
        }

        IList<TPoco> MapResult<TPoco>(IEnumerable<RealmObject> source) where TPoco : PocoClass, new()
        {
            if (source.Any()) return new List<TPoco>();

            var result = new List<TPoco>();
            foreach (var record in source)
            {
                var pocoObj = new TPoco();
                pocoObj.Initialize(record, Mapper);
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

        Func<dynamic, TResult> ExchangeLambda<TPoco, TResult>(Expression<Func<TPoco, TResult>> originalLambda)
        {
            var converter = new ConvertPoco2Scheme<TResult>(SchemeMapper.GetSchemeType<TPoco>());
            var schemeExpression = (LambdaExpression)converter.Visit(originalLambda);
            var schemeLambda = (Func<dynamic, TResult>)schemeExpression.Compile();
            return schemeLambda;
        }

        class ConvertPoco2Scheme<TResult> : ExpressionVisitor
        {
            ParameterExpression convertedParameter;
            Type schemeType;

            internal ConvertPoco2Scheme(Type schemeType) => this.schemeType = schemeType;

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                convertedParameter = Expression.Parameter(typeof(RealmObject), node.Parameters[0].Name);
                var convertedLambda = Expression.Lambda<Func<RealmObject, TResult>>(node.Body, convertedParameter);
                return base.VisitLambda(convertedLambda);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                var convertedMemberExpression = Expression.Property(
                    Expression.Convert(convertedParameter, schemeType),
                    node.Member.Name
                    );
                return convertedMemberExpression;

                //TODO: IList Member Count Property
            }
        }

        #endregion

        #endregion //Transaction

        #endregion //Private
    }
}
