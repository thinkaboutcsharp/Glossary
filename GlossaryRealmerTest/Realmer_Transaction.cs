﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using Realmer;
using Realmer.Poco;
using Realms;
using System.Reflection;

namespace GlossaryRealmerTest
{
    public class Realmer_Transaction : IDisposable
    {
        ITestOutputHelper output;
        string appPath;
        string filePath;

        #region Spells

        public Realmer_Transaction(ITestOutputHelper output)
        {
            this.output = output;

            var folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            appPath = Path.Combine(folder, "Glossary@TACFilozofio");
            filePath = Path.Combine(appPath, "glossary@tacfilozofio.realm");

            ClearTestDB();
        }

        public void Dispose()
        {
            ClearTestDB();
        }

        void ClearTestDB()
        {
            void ClearDirectory(string dir)
            {
                foreach (var child in Directory.EnumerateDirectories(dir))
                {
                    ClearDirectory(child);
                    Directory.Delete(child);
                }
                foreach (var file in Directory.EnumerateFiles(dir)) File.Delete(file);
            }

            if (Directory.Exists(appPath))
            {
                Realm.DeleteRealm(new RealmConfiguration(filePath));
                ClearDirectory(appPath);
            }
        }

        bool EqualsObject<T>(T expected, T actual)
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

        #endregion

        [Fact]
        public void Add()
        {
            IGlossaryRealmer? realmer = GlossaryRealmer.GetRealmer();

            var data = new WordStore(
                long.MinValue,
                0,
                "TestData"
            );
            realmer.Add(data);

            var destination = new List<WordStore>();
            realmer.SelectAll(destination);
            realmer.Close();

            Assert.Equal(1, (int)destination.Count());

            var selected = destination.First();
            Assert.True(EqualsObject(data, selected));
        }
    }
}