using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleUpdater.Core.Test
{
    /// <summary>
    ///Dies ist eine Testklasse für "ProgramTest" und soll
    ///alle ProgramTest Komponententests enthalten.
    ///</summary>
    [TestClass]
    public class ProgramTest
    {
        /// <summary>
        ///Ruft den Testkontext auf, der Informationen
        ///über und Funktionalität für den aktuellen Testlauf bietet, oder legt diesen fest.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        ///Ein Test für "ParseArguments"
        ///</summary>
        [TestMethod]
        [DeploymentItem("SimpleUpdater.exe")]
        public void ParseArgumentsTest()
        {
            string[] arguments = new [] { "--value2=15", "--value1=\"hallo welt\"" };
            Dictionary<string, string> expected = new Dictionary<string, string>
            {
                { "value2", "15" },
                { "value1", "hallo welt" },
            }; 

            Dictionary<string, string> actual;
            actual = Program_Accessor.ParseArguments(arguments);

            foreach (var item in expected)
            {
                Assert.IsTrue(actual.ContainsKey(item.Key));
                Assert.AreEqual(item.Value, actual[item.Key]);
            }

            Assert.IsTrue(actual.Count == expected.Count);
        }
    }
}
