using System;

namespace Realmer.Poco
{
    public readonly struct GlossarySettings
    {
        internal int PK => ApplicationId;

        public int ApplicationId { get; }

        public GlossarySettings(int applicationId)
        {
            ApplicationId = applicationId;
        }

        internal GlossarySettings(dynamic carrier)
            : this((int)carrier.ApplicationId)
        { }
    }
}
