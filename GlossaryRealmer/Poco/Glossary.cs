using System;
using System.Collections.Generic;

namespace Realmer.Poco
{
    public readonly struct Glossary
    {
        internal int PK => ApplicationId;

        public int ApplicationId { get; }
        public GlossarySettings Settings { get; }
        public IReadOnlyList<User> Users { get; }

        public Glossary(int applicationId, GlossarySettings settings, IList<User> users)
        {
            ApplicationId = applicationId;
            Settings = settings;
            Users = (IReadOnlyList<User>)users;
        }

        internal Glossary(dynamic carrier)
            : this((int)carrier.ApplicationId, (GlossarySettings)carrier.Settings, (IList<User>)carrier.Users)
        { }
    }
}
