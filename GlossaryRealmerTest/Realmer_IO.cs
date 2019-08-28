using Realmer;
using Realmer.Poco;
using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace GlossaryRealmerTest
{
    public class Realmer_IO : IClassFixture<Spells.SetupAndTeardown>
    {
        ITestOutputHelper output;
        Spells.SetupAndTeardown helper;

        IGlossaryRealmer realmer;
        string appPath;
        string filePath;

        public Realmer_IO(Spells.SetupAndTeardown helper, ITestOutputHelper output)
        {
            this.helper = helper;
            this.output = output;

            realmer = helper.realmer;
            appPath = helper.appPath;
            filePath = helper.filePath;
        }

        [Fact]
        public void BackupFileOpening()
        {
            Action test = () => realmer.Backup("BkTest");
            var ex = Assert.Throws<InvalidOperationException>(test);
            Assert.Equal("Realm is open. Can't do backup.", ex.Message);

            realmer.Close();
        }

        [Fact]
        public void BackupFile()
        {
            realmer.Close();

            realmer.Backup("BkTest");

            Assert.True(File.Exists(filePath + "_@BkTest@_"));
        }

        [Fact]
        public void Uninstall()
        {
            realmer.Close();
            realmer.Backup("BkTest");

            GlossaryRealmer.Uninstall();

            Assert.False(Directory.Exists(appPath));
        }

        [Fact]
        public void CreateFile()
        {
            Assert.True(File.Exists(filePath));
        }
    }
}
