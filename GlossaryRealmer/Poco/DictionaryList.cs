using System;
using System.Collections.Generic;

namespace Realmer.Poco
{
    public readonly struct DictionaryList
    {
        internal int PK => ApplicationId;

        public int ApplicationId { get; }
        public IReadOnlyList<Dictionary> Dictionaries { get; }

        public DictionaryList(int applicationId, IList<Dictionary> dictionaries)
        {
            ApplicationId = applicationId;
            Dictionaries = (IReadOnlyList<Dictionary>)dictionaries;
        }

        internal DictionaryList(dynamic carrier)
            : this((int)carrier.ApplicationId, (IList<Dictionary>)carrier.Dictionaries)
        { }
    }
}
