using Realmer;
using System;
using System.IO;
using Xunit.Abstractions;
using System.Runtime.CompilerServices;

namespace GlossaryRealmerTest.Spells
{
    public class SetupAndTeardown : IDisposable
    {
        internal string appPath;
        internal string filePath;
        internal IGlossaryRealmer realmer;

        public SetupAndTeardown()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            appPath = Path.Combine(folder, "Glossary@TACFilozofio");
            filePath = Path.Combine(appPath, "glossary@tacfilozofio.realm");

            //GlossaryRealmer.Uninstall();
            realmer = GlossaryRealmer.GetRealmer();
        }

        public void Dispose()
        {
            GlossaryRealmer.Uninstall();
        }
    }
}
