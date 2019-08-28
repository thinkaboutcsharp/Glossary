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
    public class Realmer_Tran : IClassFixture<Spells.SetupAndTeardown>
    {
        ITestOutputHelper output;
        Spells.SetupAndTeardown helper;

        IGlossaryRealmer realmer;
        string appPath;
        string filePath;

        public Realmer_Tran(Spells.SetupAndTeardown helper, ITestOutputHelper output)
        {
            this.helper = helper;
            this.output = output;

            realmer = helper.realmer;
            appPath = helper.appPath;
            filePath = helper.filePath;
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

        [Fact]
        public void Add()
        {
            //var data = new WordStore(
            //    long.MinValue,
            //    0,
            //    "TestData"
            //);
            //realmer.Add(data);

            /*
            var destination = new List<WordStore>();
            realmer.SelectAll(destination);
            realmer.Close();

            Assert.Equal(1, (int)destination.Count());

            var selected = destination.First();
            Assert.True(comparer.EqualsObject(data, selected));
            */
        }
    }
}
