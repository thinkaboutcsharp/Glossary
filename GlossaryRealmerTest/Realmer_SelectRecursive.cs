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
            var lists = new List<WordList>();
            for (int l = 0; l < listSize; l++)
            {
                var words = new List<WordStore>();
                for (int w = 0; w < wordNumber; w++)
                {
                    var word = new WordStore()
                    { 
                        WordId = w,
                        DictionaryId = 0,
                        Word = $"Word{l - w}"
                    };
                    words.Add(word);
                }
                var list = new WordList() { DictionaryId = 0 };
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
        }
    }
}
