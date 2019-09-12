using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Xunit.Abstractions;

namespace GlossaryRealmerTest.Spells
{
    class Comparers
    {
        ITestOutputHelper output;

        internal Comparers(ITestOutputHelper output) => this.output = output;

        internal bool EqualsObject<T>(T expected, T actual)
        {
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var result = EqualsAllPublicProperties(expected, actual, properties);
            return result;
        }

        internal bool EqualsObject<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            if (expected.Count() != actual.Count())
            {
                output.WriteLine($"Different Length;\n  Expected: {expected.Count()}\n  Actual  : {actual.Count()}");
                return false;
            }

            return EqualsPartOfObject<T>(expected.ToArray().AsSpan(), actual.ToArray().AsSpan());
        }

        internal bool EqualsObject<T>(IEnumerable<T> expected, IEnumerable<T> actual, Range targetRange)
        {
            if (expected.Count() < targetRange.End.Value || actual.Count() < targetRange.End.Value)
            {
                output.WriteLine($"Not Enough Length;\n  Expected Range: {targetRange}\n  Actual Range : Exp. {expected.Count()} Act. {actual.Count()}");
                return false;
            }

            return EqualsPartOfObject<T>(expected.ToArray().AsSpan().Slice(targetRange.Start.Value, targetRange.End.Value)
                                       , actual.ToArray().AsSpan().Slice(targetRange.Start.Value, targetRange.End.Value));
        }

        bool EqualsPartOfObject<T>(ReadOnlySpan<T> expected, ReadOnlySpan<T> actual)
        {
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (var i = 0; i < expected.Length; i++)
            {
                if (!EqualsAllPublicProperties(expected[i], actual[i], properties)) return false;
            }

            return true;
        }

        bool EqualsAllPublicProperties<T>(T expected, T actual, IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                var expectedValue = property.GetValue(expected);
                var actualValue = property.GetValue(actual);
                if (!(expectedValue == null && actualValue == null) && !(expectedValue != null && expectedValue.Equals(actualValue)))
                {
                    output.WriteLine($"Property not equals '{property.Name}' :\n expected '{expectedValue}'\n actual   '{actualValue}'");
                    return false;
                }
            }
            return true;
        }
    }
}
