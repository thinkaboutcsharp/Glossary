using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Realmer.Operation;

[assembly: InternalsVisibleTo("GlossaryDatabase")]

namespace Realmer
{
    public class GlossaryRealmer
    {
        static GlossaryRealmer? instance;
        IGlossaryRealmer? ope;

        private GlossaryRealmer()
        {
            ope = new RealmOperator();
        }

        public static IGlossaryRealmer GetRealmer()
        {
            if (instance ==  null) instance = new GlossaryRealmer();
            return instance.ope!;
        }

        public static void Dispose()
        {
            instance?.ope?.Dispose();
            instance = null;
        }

        public static void Uninstall()
        {
            Dispose();
            RealmOperator.Uninstall();
        }
    }
}
