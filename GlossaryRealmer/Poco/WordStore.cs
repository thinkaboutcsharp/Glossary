using System.Data;
using Scheme = Realmer.Scheme;

namespace Realmer.Poco
{
    public readonly struct WordStore
    {
        internal long PK => WordId;

        public long WordId { get; }
        public int DictionaryId { get; }
        public string Word { get; }

        public WordStore(long wordId, int dictionaryId, string word)
        {
            WordId = wordId;
            DictionaryId = dictionaryId;
            Word = word;
        }

        internal WordStore(dynamic carrier)
            : this((long)carrier.WordId, (int)carrier.DictionaryId, (string)carrier.Word)
        { }
    }
}
