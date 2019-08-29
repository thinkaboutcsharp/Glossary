using Realmer;
using Realmer.Poco;
using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace GlossaryRealmerTest
{
    [Collection("Realm Test Collection")]
    public class Realmer_IO : Spells.SetupAndTeardown
    {
        public Realmer_IO(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void BackupFileOpening()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => realmer.Backup("BkTest"));
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
            Assert.True(File.Exists(filePath), $"actual  :{realmer.FilePath}\nexpected:{filePath}");
        }

        [Fact]
        public void CreateFileWithDirectory()
        {
            realmer.Dispose();

            GlossaryRealmer.Uninstall();

            realmer.Open();

            Assert.True(File.Exists(filePath));

            realmer.Close();
        }
    }
}
