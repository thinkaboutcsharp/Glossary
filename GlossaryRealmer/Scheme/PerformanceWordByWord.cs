﻿using System;
using Realms;

namespace Realmer.Scheme
{
    public class PerformanceWordByWord : RealmObject
    {
        internal long PK => WordId;

        [PrimaryKey]
        public long WordId { get; set; }
        public int TestCount { get; set; }
        public int CorrectCount { get; set; }
        // etc..
    }
}
