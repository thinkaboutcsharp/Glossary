using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using Realmer;
using Realmer.Poco;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;

namespace GlossaryRealmerTest
{
    [Collection("Realm Test Collection")]
    public class Realmer_Tran : Spells.SetupAndTeardown
    {
        public Realmer_Tran(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Add()
        {
            var data = new WordStore(
                long.MinValue,
                0,
                "TestData"
            );
            realmer.Add(data);

            var destination = realmer.SelectAll<WordStore>();
            realmer.Close();

            Assert.Equal(1, (int)destination.Count());

            var selected = destination.First();
            Assert.True(comparer.EqualsObject(data, selected));
        }

        [Fact]
        public void AddRange()
        {
            var data = new[]
            {
                new WordStore(
                    long.MinValue,
                    0,
                    "TestDataA"
                ),
                new WordStore(
                    long.MinValue + 1,
                    0,
                    "TestDataB"
                )
            };
            realmer.AddRange(data);

            var destination = realmer.SelectAll<WordStore>();
            realmer.Close();

            Assert.Equal(2, (int)destination.Count());

            Assert.True(comparer.EqualsObject(data, destination));
        }

        [Fact]
        public void Update()
        {
            var data = new[]
            {
                new WordStore(
                    long.MinValue,
                    0,
                    "TestDataA"
                ),
                new WordStore(
                    long.MinValue + 1,
                    0,
                    "TestDataB"
                )
            };
            realmer.AddRange(data);

            var newData = new WordStore(long.MinValue + 1, 1, "TestDataB");

            realmer.Update(newData);
            data[1] = newData;

            var destination = realmer.SelectAll<WordStore>();
            realmer.Close();

            Assert.Equal(2, (int)destination.Count());

            Assert.True(comparer.EqualsObject(data, destination));
        }
    }
}
