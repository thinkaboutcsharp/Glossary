using Realmer;
using System;
using System.IO;
using Xunit.Abstractions;
using System.Runtime.CompilerServices;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace GlossaryRealmerTest.Spells
{
    public abstract class SetupAndTeardown : IDisposable
    {
        protected ITestOutputHelper output;

        protected string appPath;
        protected string filePath;
        protected IGlossaryRealmer realmer;

        private protected Comparers comparer;

        public SetupAndTeardown(ITestOutputHelper output)
        {
            this.output = output;

            var folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            appPath = Path.Combine(folder, "Glossary@TACFilozofio");
            filePath = Path.Combine(appPath, "glossary@tacfilozofio.realm");

            comparer = new Comparers(output);

            GlossaryRealmer.Uninstall();
            realmer = GlossaryRealmer.GetRealmer();
        }

        public void Dispose()
        {
            realmer.Close();
            GlossaryRealmer.Uninstall();
        }
    }
}
