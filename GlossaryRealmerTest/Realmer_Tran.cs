﻿using Realmer.Poco;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GlossaryRealmerTest
{
    [Collection("Realm Test Collection")]
    public class Realmer_Tran : Spells.SetupAndTeardown
    {
        public Realmer_Tran(ITestOutputHelper output) : base(output)
        {
        }

        public static IEnumerable<object[]> MakeInitialDatas(int number)
        {
            var datas = Enumerable.Range(0, number).Select(i => new object[] { i, 0, $"TestData{i}" });
            return datas;
        }

        [Fact]
        public void Add()
        {
            var data = new WordStore()
            {
                WordId = long.MinValue,
                DictionaryId = 0,
                Word = "TestData"
            };
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
                new WordStore()
                {
                    WordId = long.MinValue,
                    DictionaryId = 0,
                    Word = "TestDataA"
                },
                new WordStore()
                {
                    WordId = long.MinValue + 1,
                    DictionaryId = 0,
                    Word = "TestDataB"
                }
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
                new WordStore()
                {
                    WordId = long.MinValue,
                    DictionaryId = 0,
                    Word = "TestDataA"
                },
                new WordStore()
                {
                    WordId = long.MinValue + 1,
                    DictionaryId = 0,
                    Word = "TestDataB"
                }
            };
            realmer.AddRange(data);

            var current = realmer.Select<WordStore>(o => o.WordId == long.MinValue + 1).FirstOrDefault();
            current.DictionaryId = 1;
            realmer.Update(current);

            data[1].DictionaryId = 1;

            var destination = realmer.SelectAll<WordStore>();
            realmer.Close();

            Assert.Equal(2, (int)destination.Count());

            Assert.True(comparer.EqualsObject(data, destination));
        }

        [Fact]
        public void UpdateRange()
        {
            var data = new[]
            {
                new WordStore()
                {
                    WordId = long.MinValue,
                    DictionaryId = 0,
                    Word = "TestDataA"
                },
                new WordStore()
                {
                    WordId = long.MinValue + 1,
                    DictionaryId = 0,
                    Word = "TestDataB"
                },
                new WordStore()
                {
                    WordId = long.MinValue + 2,
                    DictionaryId = 0,
                    Word = "TestDataC"
                }
            };
            realmer.AddRange(data);

            var current = realmer.Select<WordStore>(o => o.WordId != long.MinValue);
            foreach (var word in current) word.DictionaryId = 1;
            realmer.UpdateRange(current);

            data[1].DictionaryId = 1;
            data[2].DictionaryId = 1;

            var destination = realmer.SelectAll<WordStore>();
            realmer.Close();

            Assert.Equal(3, (int)destination.Count());

            Assert.True(comparer.EqualsObject(data, destination));
        }

        [Fact]
        public void DeleteObj()
        {
            var data = new[]
            {
                new WordStore()
                {
                    WordId = long.MinValue,
                    DictionaryId = 0,
                    Word = "TestDataA"
                },
                new WordStore()
                {
                    WordId = long.MinValue + 1,
                    DictionaryId = 0,
                    Word = "TestDataB"
                }
            };
            realmer.AddRange(data);

            realmer.Delete(data[1]);

            var destination = realmer.SelectAll<WordStore>();
            realmer.Close();

            Assert.Equal(1, (int)destination.Count());

            Assert.True(comparer.EqualsObject(data[0], destination.First()));
        }

        [Fact]
        public void DeleteRangeObj()
        {
            var data = new[]
            {
                new WordStore()
                {
                    WordId = long.MinValue,
                    DictionaryId = 0,
                    Word = "TestDataA"
                },
                new WordStore()
                {
                    WordId = long.MinValue + 1,
                    DictionaryId = 0,
                    Word = "TestDataB"
                },
                new WordStore()
                {
                    WordId = long.MinValue + 2,
                    DictionaryId = 0,
                    Word = "TestDataC"
                }
            };
            realmer.AddRange(data);

            realmer.DeleteRange(new WordStore[] { data[1], data[2] });

            var destination = realmer.SelectAll<WordStore>();
            realmer.Close();

            Assert.Equal(1, (int)destination.Count());

            Assert.True(comparer.EqualsObject(data[0], destination.First()));
        }

        [Fact]
        public void DeleteId()
        {
            var data = new[]
            {
                new WordStore()
                {
                    WordId = long.MinValue,
                    DictionaryId = 0,
                    Word = "TestDataA"
                },
                new WordStore()
                {
                    WordId = long.MinValue + 1,
                    DictionaryId = 0,
                    Word = "TestDataB"
                }
            };
            realmer.AddRange(data);

            realmer.Delete<WordStore>(data[1].WordId);

            var destination = realmer.SelectAll<WordStore>();
            realmer.Close();

            Assert.Equal(1, (int)destination.Count());

            Assert.True(comparer.EqualsObject(data[0], destination.First()));
        }

        [Fact]
        public void DeleteRangeId()
        {
            var data = new[]
            {
                new WordStore()
                {
                    WordId = long.MinValue,
                    DictionaryId = 0,
                    Word = "TestDataA"
                },
                new WordStore()
                {
                    WordId = long.MinValue + 1,
                    DictionaryId = 0,
                    Word = "TestDataB"
                },
                new WordStore()
                {
                    WordId = long.MinValue + 2,
                    DictionaryId = 0,
                    Word = "TestDataC"
                }
            };
            realmer.AddRange(data);

            realmer.DeleteRange<WordStore>(new long[] { data[1].WordId, data[2].WordId });

            var destination = realmer.SelectAll<WordStore>();
            realmer.Close();

            Assert.Equal(1, (int)destination.Count());

            Assert.True(comparer.EqualsObject(data[0], destination.First()));
        }
    }
}
