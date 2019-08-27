using Realmer;
using System;
using System.IO;
using Xunit.Abstractions;
using System.Runtime.CompilerServices;

namespace GlossaryRealmerTest.Spells
{
    abstract public class SetupAndTeardown 
    {
        protected ITestOutputHelper output;
        protected string appPath;
        protected string filePath;
        protected IGlossaryRealmer realmer;
        private protected Comparers comparer;

        protected SetupAndTeardown(ITestOutputHelper output)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

            this.output = output;

            var folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            appPath = Path.Combine(folder, "Glossary@TACFilozofio");
            filePath = Path.Combine(appPath, "glossary@tacfilozofio.realm");

            comparer = new Comparers(output);
        }

        protected void Setup([CallerMemberName] string testName = "")
        {
            output.WriteLine($"TestCase:{testName}");
            GC.WaitForFullGCComplete();
            realmer = GlossaryRealmer.GetRealmer();
        }

        protected void Dispose()
        {
            realmer.Dispose();
            GlossaryRealmer.Uninstall();
        }
    }
}
