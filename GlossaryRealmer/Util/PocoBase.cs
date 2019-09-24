using AutoMapper;
using Realmer.Poco;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq.Expressions;

namespace Realmer.Util
{
    public abstract class PocoBase<TPoco, TKey> : PocoRoot<TKey>
        where TPoco : PocoBase<TPoco, TKey>
        where TKey : struct
    {
        protected private IMapper Mapper { get; private set; }

        protected private Func<TPoco, TKey> PrimaryKey { private get; set; }
        override internal TKey PK => PrimaryKey((TPoco)this);

        protected private Func<RealmObject> SchemeCtor { private get; set; }

        IDictionary<Type, Func<TPoco, IReadOnlyList<object>>> listSetter
            = new Dictionary<Type, Func<TPoco, IReadOnlyList<object>>>(); //object == Type

        protected private IReadOnlyList<TElement> AddListProperty<TElement>(Func<TPoco, IReadOnlyList<TElement>> setter)
        {
            listSetter.Add(typeof(TElement), (Func<TPoco, IReadOnlyList<object>>)setter);
            return new List<TElement>();
        }

        protected private PocoBase(Func<TPoco, TKey> pkFunc, Func<RealmObject> schemeFunc)
        {
            PrimaryKey = pkFunc;
            SchemeCtor = schemeFunc;
        }

        public void SetList<TElement>(IEnumerable<TElement> source)
        {
            if (listSetter.TryGetValue(typeof(TElement), out var setter))
            {
                var destination = (IList<TElement>)(setter((TPoco)this));
                foreach (var element in source)
                {
                    destination.Add(element);
                }
            }
        }

        internal override void Initialize(RealmObject parameter, IMapper mapper)
        {
            Contract.Requires(PrimaryKey != null);
            Contract.Requires(SchemeCtor != null);

            SchemeObj = parameter;
            Mapper = mapper;
            Mapper.Map(parameter, this, parameter.GetType(), typeof(TPoco));
        }

        internal override RealmObject AdaptClean()
        {
            SchemeObj = SchemeCtor();
            AdaptMember();
            return SchemeObj;
        }

        internal override void AdaptUpdate()
        {
            Contract.Requires(SchemeObj != null);

            AdaptMember();
        }

        override protected private TScheme AdaptScheme<TScheme>()
        {
            if (SchemeObj == null) SchemeObj = new TScheme();
            AdaptMember();
            return (TScheme)SchemeObj!;
        }

        protected private virtual void AdaptMember()
        {
            var schemeType = SchemeMapper.GetSchemeType(typeof(TPoco));
            Mapper.Map(this, SchemeObj, typeof(TPoco), schemeType);
        }
    }
}
