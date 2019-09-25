using AutoMapper;
using Realms;
using System;
using System.Linq.Expressions;

namespace Realmer
{
    internal static class RealmerConst
    {
        internal const string SchemeParameterKey = "Scheme";
        internal const string MasterDBEmbeddedPath = "Realmer.MasterDB.master.realm";
    }
}

namespace Realmer.Util
{
    public abstract class PocoClass
    {
        abstract internal void SetScheme(RealmObject parameter);
        abstract internal RealmObject AdaptClean();
        abstract internal void AdaptUpdate();
    }

    public abstract class PocoCore : PocoClass
    {
        internal RealmObject? SchemeObj { get; protected private set; }
    }

    public abstract class PocoRoot<TKey> : PocoCore
        where TKey : struct
    {
        virtual internal TKey PK { get => default(TKey); }
    }
}