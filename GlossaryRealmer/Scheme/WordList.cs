﻿using AutoMapper.Configuration.Annotations;
using Realms;
using System.Collections.Generic;

namespace Realmer.Scheme
{
    internal class WordList : RealmObject
    {
        internal int PK => DictionaryId;

        [PrimaryKey]
        public int DictionaryId { get; set; }
        [Ignore]
        public IList<WordStore> Words { get; }
    }
}
