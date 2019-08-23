using Realmer;
using Realms;
using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace GlossaryRealmerTest
{
    public class Realmer_IO : IDisposable
    {
        ITestOutputHelper output;
        string appPath;
        string filePath;

        #region Spells

        public Realmer_IO(ITestOutputHelper output)
        {
            this.output = output;

            var folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            appPath = Path.Combine(folder, "Glossary@TACFilozofio");
            filePath = Path.Combine(appPath, "glossary@tacfilozofio.realm");

            ClearTestDB();
        }

        public void Dispose()
        {
            ClearTestDB();
        }

        void ClearTestDB()
        {
            void ClearDirectory(string dir)
            {
                foreach (var child in Directory.EnumerateDirectories(dir))
                {
                    ClearDirectory(child);
                    Directory.Delete(child);
                }
                foreach (var file in Directory.EnumerateFiles(dir)) File.Delete(file);
            }

            if (Directory.Exists(appPath))
            {
                Realm.DeleteRealm(new RealmConfiguration(filePath));
                ClearDirectory(appPath);
            }
        }

        #endregion

        [Fact]
        public void CreateFile()
        {
            var realmer = GlossaryRealmer.GetRealmer();

            Assert.True(File.Exists(filePath));

            realmer.Close();
        }

        [Fact]
        public void CreateFileWithDirectory()
        {
            Directory.Delete(appPath);

            var realmer = GlossaryRealmer.GetRealmer();

            Assert.True(File.Exists(filePath));

            realmer.Close();
        }

        [Fact]
        public void BackupFileOpening()
        {
            var realmer = GlossaryRealmer.GetRealmer();

            Action test = () => realmer.Backup("BkTest");
            var ex = Assert.Throws<InvalidOperationException>(test);
            Assert.Equal("Realm is open. Can't do backup.", ex.Message);

            realmer.Close();
        }

        [Fact]
        public void BackupFile()
        {
            var realmer = GlossaryRealmer.GetRealmer();
            realmer.Close();

            realmer.Backup("BkTest");

            Assert.True(File.Exists(filePath + "_@BkTest@_"));
        }

        [Fact]
        public void Uninstall()
        {
            var realmer = GlossaryRealmer.GetRealmer();
            realmer.Close();
            realmer.Backup("BkTest");

            realmer.Uninstall();

            Assert.False(Directory.Exists(appPath));
        }
    }
}
