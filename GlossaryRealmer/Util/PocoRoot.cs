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
        abstract internal void Initialize(RealmObject parameter, IMapper mapper);
        abstract internal RealmObject AdaptClean();
        abstract internal void AdaptUpdate();
    }

    public abstract class PocoCore : PocoClass
    {
        abstract internal TScheme Adapt<TScheme>() where TScheme : RealmObject, new();
        internal RealmObject? SchemeObj { get; protected private set; }
    }

    public abstract class PocoRoot<TKey> : PocoCore
        where TKey : struct
    {
        virtual internal TKey PK { get => default(TKey); }

        override internal TScheme Adapt<TScheme>() => AdaptScheme<TScheme>();
        abstract protected private TScheme AdaptScheme<TScheme>() where TScheme : RealmObject, new();
    }
}