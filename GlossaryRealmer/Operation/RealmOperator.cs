using System;
using System.IO;
using Realms;

namespace Realmer.Operation
{
    public class RealmOperator
    {
        const string DatabaseFileName = "glossary@tacfilozofio.realm";
        const ulong SchemaVersion = 0;

        const string BackupKeyFormat = "_@{0}@_";

        Realm? realm;
        RealmConfiguration config;
        string filePath;

        public RealmOperator() => realm = InitRealm();

        public void  Open()
        {
            Close();
            realm = Realm.GetInstance(config);
        }

        public void Close()
        {
            realm?.Dispose();
            realm = null;
        }



        Realm InitRealm()
        {
            config = ConfigRealm();
            CheckDatabaseFile();

            return Realm.GetInstance(config);
        }

        RealmConfiguration ConfigRealm()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            filePath = Path.Combine(folder, DatabaseFileName);

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
                var master = assembly.GetManifestResourceStream("SimpleTodo.Data.master.realm");

                CopyOriginalDatabase(filePath, master);
            }
        }

        void CopyOriginalDatabase(string copyPath, Stream originStream)
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
    }
}
