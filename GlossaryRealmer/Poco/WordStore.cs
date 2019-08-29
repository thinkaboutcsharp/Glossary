using System.Data;
using Scheme = Realmer.Scheme;

namespace Realmer.Poco
{
    public readonly struct WordStore
    {
        public readonly long WordId { get; }
        public readonly int DictionaryId { get; }
        public readonly string Word { get; }

        public WordStore(long id, int dictionaryId, string word)
        {
            WordId = id;
            DictionaryId = dictionaryId;
            Word = word;
        }
    }
}
