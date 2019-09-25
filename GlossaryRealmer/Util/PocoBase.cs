using AutoMapper;
using Realmer.Poco;
using Realms;
using Realms.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Realmer.Util
{
    static class Accessers
    {
        static IDictionary<TypePair, ListFunctionGroup> functionGroupCache = new Dictionary<TypePair, ListFunctionGroup>();

        static internal ListFunctionGroup CreateFuntionGroupCache<TPoco, TPocoElement>() where TPoco : PocoClass, new() where TPocoElement : PocoClass
        {
            var key = GenerateKey<TPoco, TPocoElement>();
            if (!functionGroupCache.TryGetValue(key, out var groupCache))
            {
                var cache = new ListFunctionGroup();
                functionGroupCache.Add(GenerateKey<TPoco, TPocoElement>(), cache);
                return cache;
            }
            return groupCache;
        }

        static internal void AddFunctionGroup<TPoco, TPocoElement>(IListPropertyFunctions functions) where TPoco : PocoClass, new() where TPocoElement : PocoClass
        {
            var groupCache = CreateFuntionGroupCache<TPoco, TPocoElement>();
            groupCache.AddFunctions(functions);
        }

        static internal bool GetFunctionGroup<TPoco, TPocoElement>(out IEnumerable<IListPropertyFunctions>? cache)
        {
            var key = GenerateKey<TPoco, TPocoElement>();
            if (functionGroupCache.TryGetValue(key, out var groupCache))
            {
                cache = groupCache.Functionses;
                return true;
            }
            cache = null;
            return false;
        }

        static internal bool HasCache<TPoco, TPocoElement>() => functionGroupCache.ContainsKey(GenerateKey<TPoco, TaskScheduler>());

        static TypePair GenerateKey<TPoco, TPocoElement>() => new TypePair(typeof(TPoco), typeof(TPocoElement));

        class TypePair
        {
            internal Type PocoType { get; }
            internal Type ELementType { get; }

            internal TypePair(Type pocoType, Type elementType) => (PocoType, ELementType) = (pocoType, elementType);
        }

        internal class ListFunctionGroup
        {
            List<IListPropertyFunctions> functionsList = new List<IListPropertyFunctions>();
            internal IReadOnlyList<IListPropertyFunctions> Functionses => functionsList;
            internal void AddFunctions(IListPropertyFunctions functions) => functionsList.Add(functions);
        }
    }

    public abstract class PocoBase<TPoco, TKey> : PocoRoot<TKey>
        where TPoco : PocoBase<TPoco, TKey>, new()
        where TKey : struct
    {
        protected private IMapper Mapper { get; private set; }

        protected private Func<TPoco, TKey> PrimaryKey { private get; set; }
        override internal TKey PK => PrimaryKey((TPoco)this);

        protected private Func<RealmObject> SchemeCtor { private get; set; }

        IDictionary<Type, IListPropertyFunctions> listHandlers
            = new Dictionary<Type, IListPropertyFunctions>();

        protected private IReadOnlyList<TPocoElement> AddListProperty<TPocoElement, TScheme, TSchemeElement>(Expression<Func<TPoco, IReadOnlyList<TPocoElement>>> getter)
            where TPocoElement : PocoClass, new()
            where TScheme : RealmObject
            where TSchemeElement : RealmObject
        {
            if (!Accessers.GetFunctionGroup<TPoco, TPocoElement>(out var cache))
            {
                CreateNewFunctions<TPocoElement, TScheme, TSchemeElement>(getter);
            }
            else
            {
                if (!Accessers.HasCache<TPoco, TPocoElement>())
                {
                    CreateNewFunctions<TPocoElement, TScheme, TSchemeElement>(getter);
                }
            }

            return new List<TPocoElement>();
        }

        void CreateNewFunctions<TPocoElement, TScheme, TSchemeElement>(Expression<Func<TPoco, IReadOnlyList<TPocoElement>>> getter)
            where TPocoElement : PocoClass, new()
            where TScheme : RealmObject
            where TSchemeElement : RealmObject
        {
            var funcs = new ListPropertyFunctions<TPoco, TPocoElement, TScheme, TSchemeElement>();
            funcs.CreateGetterSetter(getter);
            listHandlers.Add(typeof(TPocoElement), funcs);
            Accessers.AddFunctionGroup<TPoco, TPocoElement>(funcs);
        }

        protected private PocoBase(Func<TPoco, TKey> pkFunc, Func<RealmObject> schemeFunc)
        {
            PrimaryKey = pkFunc;
            SchemeCtor = schemeFunc;
        }

        public void SetList<TElement>(IEnumerable<TElement> source) where TElement : PocoClass
        {
            if (listHandlers.TryGetValue(typeof(TElement), out var listFunctions))
            {
                var getter = listFunctions.GetterObj;
                var destination = (IList<TElement>)getter((TPoco)this);
                foreach (var element in source)
                {
                    destination.Add(element);
                }
            }
        }

        internal override void SetScheme(RealmObject parameter)
        {
            Contract.Requires(PrimaryKey != null);
            Contract.Requires(SchemeCtor != null);

            SchemeObj = parameter;
            Mapper = SchemeObj.GetMapper();
            Mapper.Map(parameter, this, parameter.GetType(), typeof(TPoco));

            CopySchemeList();
        }

        void CopySchemeList()
        {
            foreach (var handler in listHandlers)
            {
                dynamic getter = handler.Value.GetterObjScheme;
                dynamic setter = handler.Value.SetterObj;
                dynamic ctor = handler.Value.PocoElementCtor;
                var schemeList = getter(SchemeObj!);

                foreach (var scheme in schemeList)
                {
                    var element = (PocoClass)ctor();
                    element.SetScheme(scheme);
                    setter((TPoco)this, element);
                }
            }
        }

        internal override RealmObject AdaptClean()
        {
            SchemeObj = SchemeCtor();
            Mapper = SchemeObj.GetMapper();
            AdaptMemberClean();
            return SchemeObj;
        }

        internal override void AdaptUpdate()
        {
            Contract.Requires(SchemeObj != null);

            AdaptMember();
        }

        protected private virtual void AdaptMember()
        {
            var schemeType = SchemeMapper.GetSchemeType(typeof(TPoco));
            Mapper.Map(this, SchemeObj, typeof(TPoco), schemeType);
        }

        protected private virtual void AdaptMemberClean()
        {
            AdaptMember();

            foreach (var handler in listHandlers)
            {
                dynamic getter = handler.Value.GetterObj;
                dynamic setter = handler.Value.SetterObjScheme;
                var pocoList = getter((TPoco)this);

                foreach (var poco in pocoList)
                {
                    var pocoObj = poco;
                    setter(SchemeObj!, pocoObj.AdaptClean());
                }
            }
        }
    }

    #region ListFunc

    interface IListPropertyFunctions
    {
        internal dynamic PocoElementCtor { get; }
        internal dynamic GetterObj { get; }
        internal dynamic SetterObj { get; }
        internal dynamic GetterObjScheme { get; }
        internal dynamic SetterObjScheme { get; }
    }

    class ListPropertyFunctions<TPoco, TPocoElement, TScheme, TSchemeElement> : IListPropertyFunctions
        where TPoco : PocoClass, new()
        where TPocoElement : PocoClass, new()
        where TScheme : RealmObject
        where TSchemeElement : RealmObject
    {
        internal ListPropertySetterGetter<TPoco, TPocoElement, TScheme, TSchemeElement> Functions { get; private set; }
        internal void CreateGetterSetter(Expression<Func<TPoco, IReadOnlyList<TPocoElement>>> getter)
        {
            Functions = new ListPropertySetterGetter<TPoco, TPocoElement, TScheme, TSchemeElement>(getter);
        }

        object IListPropertyFunctions.PocoElementCtor => Functions.PocoElementCtor;
        object IListPropertyFunctions.GetterObj => Functions.Getter;
        object IListPropertyFunctions.SetterObj => Functions.Setter;
        object IListPropertyFunctions.GetterObjScheme => Functions.GetterScheme;
        object IListPropertyFunctions.SetterObjScheme => Functions.SetterScheme;
    }

    class ListPropertySetterGetter<TPoco, TPocoElement, TScheme, TSchemeElement>
        where TPoco : PocoClass, new()
        where TPocoElement : PocoClass, new()
        where TScheme : RealmObject
        where TSchemeElement : RealmObject
    {
        internal Func<TPocoElement> PocoElementCtor { get; }
        internal Func<TPoco, IReadOnlyList<TPocoElement>> Getter { get; }
        internal Action<TPoco, object> Setter { get; }
        internal Func<RealmObject, IList<TSchemeElement>> GetterScheme { get; }
        internal Action<RealmObject, TSchemeElement> SetterScheme { get; }

        internal ListPropertySetterGetter(Expression<Func<TPoco, IReadOnlyList<TPocoElement>>> getter)
        {
            PocoElementCtor = () => new TPocoElement();
            Getter = getter.Compile();
            Setter = (TPoco p, object e) => ((IList<TPocoElement>)Getter(p)).Add((TPocoElement)e);

            var getterScheme = ExchangeSchemeLambda(getter);
            GetterScheme = (s) => getterScheme((TScheme)s);
            SetterScheme = (s, e) => getterScheme((TScheme)s).Add(e);
        }

        Func<RealmObject, IList<TSchemeElement>> ExchangeSchemeLambda(Expression<Func<TPoco, IReadOnlyList<TPocoElement>>> pocoFunc)
        {
            var converter = new ConvertLambdaPoco2Scheme<RealmObject, IList<TSchemeElement>>(typeof(TScheme));
            var schemeExpression = (LambdaExpression)converter.Visit(pocoFunc);
            var schemeLambda = (Func<RealmObject, IList<TSchemeElement>>)schemeExpression.Compile();
            return schemeLambda;
        }
    }

    #endregion
}
