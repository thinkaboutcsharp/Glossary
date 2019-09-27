using GlossaryRealmerTest.Spells;
using Realmer.Poco;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Xunit;
using Xunit.Abstractions;

namespace GlossaryRealmerTest
{
    public class Realmer_SelectRecursive : SetupAndTeardown
    {
        public Realmer_SelectRecursive(ITestOutputHelper helper) : base(helper) { }

        IEnumerable<WordList> MakeWordList(int listSize, int wordNumber)
        {
            var wordCount = 0;
            var lists = new List<WordList>();
            for (int l = 0; l < listSize; l++)
            {
                var words = new List<WordStore>();
                for (int w = 0; w < wordNumber; w++)
                {
                    var word = new WordStore()
                    { 
                        WordId = wordCount,
                        DictionaryId = l,
                        Word = $"Word{l - w}"
                    };
                    words.Add(word);
                    wordCount++;
                }
                var list = new WordList() { DictionaryId = l };
                list.SetList(words);
                lists.Add(list);
            }
            return lists;
        }

        [Fact]
        public void SelectSingle()
        {
            var testObj = MakeWordList(1, 1);
            realmer.AddRange(testObj);

            var result = realmer.SelectAll<WordList>();

            Assert.Equal(1, result.Count());
            Assert.Equal(1, result.First().Words.Count);
            Assert.Equal(0, result.First().Words[0].WordId);
            Assert.Equal(0, result.First().Words[0].DictionaryId);
            Assert.Equal("Word0", result.First().Words[0].Word);
        }

        [Fact]
        public void SelectMultiInSingle()
        {
            var testObj = MakeWordList(1, 3);
            realmer.AddRange(testObj);

            var result = realmer.SelectAll<WordList>();

            Assert.Equal(1, result.Count());
            Assert.Equal(3, result.First().Words.Count);
            for (int w = 0; w < 3; w++)
            {
                Assert.Equal(w, result.First().Words[w].WordId);
                Assert.Equal(0, result.First().Words[w].DictionaryId);
                Assert.Equal($"Word{0 - w}", result.First().Words[w].Word);
            }
        }

        [Fact]
        public void SelectMultiInMulti()
        {
            var testObj = MakeWordList(3, 3);
            realmer.AddRange(testObj);

            var result = realmer.SelectAll<WordList>();

            Assert.Equal(3, result.Count());
            int l = 0;
            int wordCount = 0;
            foreach (var wordList in result)
            {
                Assert.Equal(l, wordList.DictionaryId);
                Assert.Equal(3, wordList.Words.Count);
                for (int w = 0; w < 3; w++)
                {
                    Assert.Equal(wordCount, wordList.Words[w].WordId);
                    Assert.Equal(l, wordList.Words[w].DictionaryId);
                    Assert.Equal($"Word{l - w}", wordList.Words[w].Word);
                    wordCount++;
                }
                l++;
            }
        }
    }
}
