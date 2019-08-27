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

        [Fact]
        public void Add()
        {
            Setup();

            var data = new WordStore(
                long.MinValue,
                0,
                "TestData"
            );
            realmer.Add(data);

            var destination = new List<WordStore>();
            realmer.SelectAll(destination);
            realmer.Close();

            Assert.Equal(1, (int)destination.Count());

            var selected = destination.First();
            Assert.True(comparer.EqualsObject(data, selected));

            Dispose();
        }
    }
}
