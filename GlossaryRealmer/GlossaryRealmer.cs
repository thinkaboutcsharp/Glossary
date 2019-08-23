using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Realmer.Operation;

[assembly: InternalsVisibleTo("GlossaryDatabase")]

namespace Realmer
{
    public class GlossaryRealmer : IDisposable
    {
        IGlossaryRealmer? ope;

        private GlossaryRealmer()
        {
            ope = new RealmOperator(() => Dispose());
        }

        public void Dispose()
        {
            //It is necessary that release references related to Realm even if realm is disposed.
            //It seems some static objects related to Realm are left.
            ope = null;
        }

        public static IGlossaryRealmer GetRealmer()
        {
            var instance = new GlossaryRealmer();
            return instance.ope!;
        }
    }
}
