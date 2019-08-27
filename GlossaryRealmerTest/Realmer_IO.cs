using Realmer;
using Realmer.Poco;
using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace GlossaryRealmerTest
{
    public class Realmer_IO : Spells.SetupAndTeardown
    {
        public Realmer_IO(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void CreateFile()
        {
            Setup();

            Assert.True(File.Exists(filePath));

            Dispose();
        }

        //[Fact]
        public void CreateFileWithDirectory()
        {
            realmer.Dispose();

            GlossaryRealmer.Uninstall();

            realmer.Open();

            Assert.True(File.Exists(filePath));

            realmer.Close();
        }

        //[Fact]
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
            Setup();

            realmer.Close();

            realmer.Backup("BkTest");

            Assert.True(File.Exists(filePath + "_@BkTest@_"));

            Dispose();
        }

        [Fact]
        public void Uninstall()
        {
            Setup();

            realmer.Close();
            realmer.Backup("BkTest");

            GlossaryRealmer.Uninstall();

            Assert.False(Directory.Exists(appPath));

            Dispose();
        }
    }
}
