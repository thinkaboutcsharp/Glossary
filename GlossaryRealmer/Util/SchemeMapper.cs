﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Realmer.Poco
{
    static class SchemeMapper
    {
        static internal Type GetSchemeType<TPoco>()
        {
            return GetSchemeType(typeof(TPoco));
        }

        static internal Type GetSchemeType(Type pocoType)
        {
            return SwitchSchemeType<Type>(pocoType, SwitchType.Type);
        }

        static internal IEnumerable<long> GetPkEnum<TPoco>(IEnumerable<TPoco> objEnum)
        {
            foreach (var obj in objEnum)
            {
                var dynamicRecord = (dynamic)obj!;
                long pk = dynamicRecord.PK;
                yield return pk;
            }
        }

        static internal Func<dynamic, bool> GetPKFunc<TPoco>(TPoco record)
        {
            var dynamicRecord = (dynamic)record!;
            long pk = dynamicRecord.PK;
            return GetPKFunc<TPoco>(pk);
        }

        static internal Func<dynamic, bool> GetPKFunc<TPoco>(long pk)
        {
            return SwitchSchemeType<Func<dynamic, bool>>(typeof(TPoco), SwitchType.Func, pk);
        }

        static TReturn SwitchSchemeType<TReturn>(Type pocoType, SwitchType type, long pk = 0L)
        {
            if (type == SwitchType.Func) return (TReturn)(object)new Func<dynamic, bool>((dynamic p) => p.PK == pk);

            var pocoName = pocoType.Name;

            if (type == SwitchType.Type)
            {
                object schemeType = new object();
                switch (pocoName)
                {
                    case nameof(Poco.WordStore):
                        schemeType = typeof(Scheme.WordStore);
                        break;
                    case nameof(Poco.WordList):
                        schemeType = typeof(Scheme.WordList);
                        break;
                    case nameof(Poco.Dictionary):
                        schemeType = typeof(Scheme.Dictionary);
                        break;
                    case nameof(Poco.DictionaryInfo):
                        schemeType = typeof(Scheme.DictionaryInfo);
                        break;
                    case nameof(Poco.DictionaryList):
                        schemeType = typeof(Scheme.DictionaryList);
                        break;
                    case nameof(Poco.PerformanceWordByWord):
                        schemeType = typeof(Scheme.PerformanceWordByWord);
                        break;
                    case nameof(Poco.DictionaryPerformanceListWordByWord):
                        schemeType = typeof(Scheme.DictionaryPerformanceListWordByWord);
                        break;
                    case nameof(Poco.PerformanceDictionaryByDictionary):
                        schemeType = typeof(Scheme.PerformanceDictionaryByDictionary);
                        break;
                    case nameof(Poco.GlossarySettings):
                        schemeType = typeof(Scheme.GlossarySettings);
                        break;
                    case nameof(Poco.User):
                        schemeType = typeof(Scheme.User);
                        break;
                    case nameof(Poco.UserList):
                        schemeType = typeof(Scheme.UserList);
                        break;
                    case nameof(Poco.Glossary):
                        schemeType = typeof(Scheme.Glossary);
                        break;
                }

                return (TReturn)schemeType;
            }

            return (TReturn)new object();
        }

        enum SwitchType
        {
            Type,
            Func
        }
    }

    internal class ConvertLambdaPoco2Scheme<TResult> : ConvertLambdaPoco2Scheme<dynamic, TResult>
    {
        internal ConvertLambdaPoco2Scheme(Type schemeType) : base(schemeType) { }
    }

    internal class ConvertLambdaPoco2Scheme<TParameter, TResult> : ExpressionVisitor
    {
        ParameterExpression convertedParameter;
        Type schemeType;

        internal ConvertLambdaPoco2Scheme(Type schemeType) => this.schemeType = schemeType;

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            convertedParameter = Expression.Parameter(typeof(TParameter), node.Parameters[0].Name);
            var convertedLambda = Expression.Lambda<Func<TParameter, TResult>>(
                Expression.Convert(
                    node.Body,
                    typeof(TResult)
                    ),
                convertedParameter
                );
            return base.VisitLambda(convertedLambda);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var convertedMemberExpression = Expression.Property(
                Expression.Convert(convertedParameter, schemeType),
                node.Member.Name
                );
            return convertedMemberExpression;

            //TODO: IList Member Count Property
        }
    }
}
