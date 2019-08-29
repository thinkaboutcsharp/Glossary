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

            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (var i = 0; i < expected.Count(); i++)
            {
                if (!EqualsAllPublicProperties(expected.ElementAt(i), actual.ElementAt(i), properties)) return false;
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
