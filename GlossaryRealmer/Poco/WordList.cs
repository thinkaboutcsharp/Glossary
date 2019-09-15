using System;
using System.Collections.Generic;
using System.Text;

namespace Realmer.Poco
{
    public readonly struct WordList
    {
        internal int PK => DictionaryId;

        public int DictionaryId { get; }
        public IReadOnlyList<WordStore> Words { get; }

        public WordList(int dictionaryId, IList<WordStore> words)
        {
            DictionaryId = dictionaryId;
            Words = (IReadOnlyList<WordStore>)words;
        }

        internal WordList(dynamic carrier)
            : this((int)carrier.DictionaryId, (IList<WordStore>)carrier.Words)
        { }
    }
}
