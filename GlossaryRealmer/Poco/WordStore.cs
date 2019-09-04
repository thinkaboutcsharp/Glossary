using System.Data;
using Scheme = Realmer.Scheme;

namespace Realmer.Poco
{
    public readonly struct WordStore
    {
        public readonly string PKName => nameof(WordId);

        public readonly long WordId { get; }
        public readonly int DictionaryId { get; }
        public readonly string Word { get; }

        public WordStore(long id, int dictionaryId, string word)
        {
            WordId = id;
            DictionaryId = dictionaryId;
            Word = word;
        }

        internal WordStore(Scheme.WordStore scheme)
            : this(scheme.WordId, scheme.DictionaryId, scheme.Word)
        { }
    }
}
