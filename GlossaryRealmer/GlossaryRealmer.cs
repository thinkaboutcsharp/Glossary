using System.IO;
using System.Runtime.CompilerServices;
using Realmer.Operation;
using Realms;

[assembly: InternalsVisibleTo("GlossaryDatabase")]

namespace Realmer
{
    public class GlossaryRealmer
    {
        static GlossaryRealmer? instance;
        IGlossaryRealmer? ope;

        static string appPath;
        static string filePath;

        private GlossaryRealmer()
        {
            ope = new RealmOperator();
            appPath = ope.AppPath;
            filePath = ope.FilePath;
        }

        private void DisposeRealmer() => ope = null;

        public static IGlossaryRealmer GetRealmer()
        {
            if (instance ==  null) instance = new GlossaryRealmer();
            return instance.ope!;
        }

        public static void Dispose()
        {
            instance?.ope?.Dispose();
            instance?.DisposeRealmer();
            instance = null;
        }

        public static void Uninstall()
        {
            Dispose();

            if (Directory.Exists(appPath))
            {
                var deleteConfig = new RealmConfiguration(filePath);
                Realm.DeleteRealm(deleteConfig);
                foreach (var file in Directory.EnumerateFiles(appPath)) File.Delete(file);
                Directory.Delete(appPath);
            }
        }
    }
}
