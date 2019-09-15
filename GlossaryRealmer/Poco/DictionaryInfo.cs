using System;

namespace Realmer.Poco
{
    public readonly struct DictionaryInfo
    {
        internal int PK => DictionaryId;

        public int DictionaryId { get; }

        public DictionaryInfo(int dictionaryId)
        {
            DictionaryId = dictionaryId;
        }

        internal DictionaryInfo(dynamic carrier)
            : this((int)carrier.DictionaryId)
        { }
    }
}
