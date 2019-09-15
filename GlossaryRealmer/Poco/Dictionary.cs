using System;

namespace Realmer.Poco
{
    public readonly struct Dictionary
    {
        internal int PK => DictionaryId;

        public int DictionaryId { get; }
        public WordList WordList { get; }
        public DictionaryInfo Info { get; }

        public Dictionary(int dictionaryId, WordList wordList, DictionaryInfo info)
        {
            DictionaryId = dictionaryId;
            WordList = wordList;
            Info = info;
        }

        internal Dictionary(dynamic carrier)
            : this((int)carrier.DictionaryId, (WordList)carrier.WordList, (DictionaryInfo)carrier.Info)
        { }
    }
}
