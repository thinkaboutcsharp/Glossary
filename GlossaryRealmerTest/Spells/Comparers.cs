using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
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
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
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
