using AutoMapper;
using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Realmer.Util
{
    internal static class GlossaryMapper
    {
        internal static IMapper Mapper { private get; set; }

        internal static IMapper GetMapper(this RealmObject scheme) => Mapper;
    }
}
