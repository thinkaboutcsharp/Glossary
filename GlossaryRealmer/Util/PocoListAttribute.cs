using System;
using System.Collections.Generic;
using System.Text;

namespace Realmer.Util
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class PocoClassAttribute : Attribute
    {
        public Type PocoType { get; }
        public PocoClassAttribute(Type pocoType)
        {
            PocoType = pocoType;
        }
    }
}
