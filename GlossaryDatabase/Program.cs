using Realms;
using System;
using System.IO;
using Realmer.Scheme;

namespace GlossaryDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.GetFullPath("../../../MasterDB/master.realm");

            var dirRoot = Path.GetFullPath("../../../MasterDB");
            void ClearOld(string dir)
            {
                foreach (var dirChild in Directory.EnumerateDirectories(dir)) ClearOld(dirChild);

                foreach (var file in Directory.EnumerateFiles(dir))
                {
                    File.Delete(file);
                }
            }
            ClearOld(dirRoot);

            var config = new RealmConfiguration(path);
            config.SchemaVersion = 0;
            config.ObjectClasses = new Type[]
            {
                typeof(WordStore),
                typeof(WordStoreCounter),
                typeof(WordList),
                typeof(Dictionary),
                typeof(DictionaryCounter),
                typeof(DictionaryList),
                typeof(DictionaryInfo),
                typeof(PerformanceWordByWord),
                typeof(DictionaryPerformanceListWordByWord),
                typeof(PerformanceDictionaryByDictionary),
                typeof(User),
                typeof(UserList),
                typeof(UserCounter),
                typeof(GlossarySettings),
                typeof(Glossary)

            };
            var realm = Realm.GetInstance(config);

            realm.Dispose();

            var truePath = Path.GetFullPath("../../../../GlossaryRealmer/MasterDB/master.realm");
            File.Copy(path, truePath, true);
        }
    }
}
