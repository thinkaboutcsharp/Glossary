using System;

namespace Realmer.Poco
{
    public readonly struct PerformanceWordByWord
    {
        internal long PK => WordId;

        public long WordId { get; }
        public int TestCount { get; }
        public int CorrectCount { get; }

        public PerformanceWordByWord(long wordId, int testCount, int correctCount)
        {
            WordId = wordId;
            TestCount = testCount;
            CorrectCount = correctCount;
        }

        internal PerformanceWordByWord(dynamic carrier)
            : this((long)carrier.WordId, (int)carrier.TestCount, (int)carrier.CorrectCount)
        { }
    }
}
