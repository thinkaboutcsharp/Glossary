using System;
using Realmer.Operation;

namespace Realmer
{
    public class GlossaryRealmer
    {
        RealmOperator ope;

        private GlossaryRealmer()
        {
            ope = new RealmOperator();
        }

        public GlossaryRealmer GetRealmer() => new GlossaryRealmer();
    }
}
