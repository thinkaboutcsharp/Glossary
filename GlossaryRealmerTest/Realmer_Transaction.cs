using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using Realmer;
using Realmer.Poco;
using System.Reflection;

namespace GlossaryRealmerTest
{
    public class Realmer_Transaction : Spells.SetupAndTeardown
    {
        public Realmer_Transaction(ITestOutputHelper output) : base(output) { }

        //[Fact]
        public void Add()
        {
            var data = new WordStore(
                long.MinValue,
                0,
                "TestData"
            );
            realmer.Add(data);

            /*
            var destination = new List<WordStore>();
            realmer.SelectAll(destination);
            realmer.Close();

            Assert.Equal(1, (int)destination.Count());

            var selected = destination.First();
            Assert.True(EqualsObject(data, selected));
            */
        }
        [Fact]
        public void CreateFileWithDirectory()
        {
            Setup();

            GlossaryRealmer.Uninstall();

            realmer.Open();

            Assert.True(File.Exists(filePath));

            Dispose();
        }

        [Fact]
        public void BackupFileOpening()
        {
            Setup();

            Action test = () => realmer.Backup("BkTest");
            var ex = Assert.Throws<InvalidOperationException>(test);
            Assert.Equal("Realm is open. Can't do backup.", ex.Message);

            Dispose();
        }

    }
}
