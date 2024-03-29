﻿using Realmer;
using Realmer.Poco;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace GlossaryRealmerTest
{
    [Collection("Realm Test Collection")]
    public class Realmer_Select : Spells.SetupAndTeardown
    {
        public Realmer_Select(ITestOutputHelper output) : base(output)
        {
        }

        public static IEnumerable<WordStore> MakeInitialDatas(int number)
        {
            var datas = Enumerable.Range(0, number).Select(
                i => new WordStore()
                {
                    WordId = i,
                    DictionaryId = number - i,
                    Word = $"TestData{i}"
                }
                );
            return datas;
        }

        [Fact]
        public void SelectAll()
        {
            var datas = MakeInitialDatas(10);
            realmer.AddRange(datas);

            var results = realmer.SelectAll<WordStore>();

            Assert.Equal(10, results.Count());

            comparer.EqualsObject(datas, results);
        }

        [Fact]
        public void SelectAfterQuery()
        {
            var datas = MakeInitialDatas(10);
            realmer.AddRange(datas);

            var results = realmer.SelectAll<WordStore>().Where(s => s.WordId < 5);

            Assert.Equal(5, results.Count());

            comparer.EqualsObject(datas.Where(s => s.WordId < 5), results);
        }

        [Fact]
        public void SelectWhereClause()
        {
            var datas = MakeInitialDatas(10);
            realmer.AddRange(datas);

            var results = realmer.Select<WordStore>(s => s.WordId < 5);

            Assert.Equal(5, results.Count());

            comparer.EqualsObject(datas.Where(s => s.WordId < 5), results);
        }

        [Fact]
        public void SelectOrderBy()
        {
            var datas = MakeInitialDatas(10);
            realmer.AddRange(datas);

            var results = realmer.Select<WordStore, int>(s => s.DictionaryId);

            Assert.Equal(10, results.Count());

            comparer.EqualsObject(datas.Reverse(), results);
        }

        [Fact]
        public void SelectOrderByDesc()
        {
            var datas = MakeInitialDatas(10);
            realmer.AddRange(datas);

            var results = realmer.Select<WordStore, long>(s => s.WordId, OrderBy.Descending);

            Assert.Equal(10, results.Count());

            comparer.EqualsObject(datas.Reverse(), results);
        }

        [Fact]
        public void SelectTwoOrderBy()
        {
            var datas = MakeInitialDatas(10);
            realmer.AddRange(datas);

            var results = realmer.Select<WordStore, long, int>(s => s.WordId % 2L, s => s.DictionaryId);

            Assert.Equal(10, results.Count());

            comparer.EqualsObject(datas.OrderBy(s => s.WordId % 2L).ThenBy(s => s.DictionaryId), results);
        }

        [Fact]
        public void SelectWhereOrderBy()
        {
            var datas = MakeInitialDatas(10);
            realmer.AddRange(datas);

            var results = realmer.Select<WordStore, int>(s => s.WordId < 5, s => s.DictionaryId);

            Assert.Equal(5, results.Count());

            comparer.EqualsObject(datas.Where(s => s.WordId < 5).OrderBy(s => s.DictionaryId), results);
        }

        [Fact]
        public void SelectProjection()
        {
            var datas = MakeInitialDatas(3);
            realmer.AddRange(datas);

            var results = realmer.SelectAll<WordStore>(s => new { s.WordId, s.DictionaryId });

            Assert.Equal(3, results.Count());

            var dataList = datas.ToList();
            var resultList = results.ToList();
            for (int i = 0; i < 3; i++)
            {
                Assert.Equal(dataList[i].WordId, resultList[i].WordId);
                Assert.Equal(dataList[i].DictionaryId, resultList[i].DictionaryId);
            }
        }
    }
}
