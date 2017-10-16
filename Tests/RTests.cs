using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class RTests
    {
        private static REngine engine;

        [ClassInitialize]
        public static void Init(TestContext ctx)
        {
            REngine.SetEnvironmentVariables(); // <-- May be omitted; the next line would call it.
            engine = REngine.GetInstance();
        }

        [TestMethod]
        public void RTest()
        {

            // A somewhat contrived but customary Hello World:
            CharacterVector charVec = engine.CreateCharacterVector(new[] { "Hello, R world!, .NET speaking" });
            engine.SetSymbol("greetings", charVec);
            engine.Evaluate("str(greetings)"); // print out in the console
            string[] a = engine.Evaluate("'Hi there .NET, from the R engine'").AsCharacter().ToArray();
            Console.WriteLine("R answered: '{0}'", a[0]);
            Console.WriteLine("Press any key to exit the program");
        }

        [TestMethod]
        public void DrawCharRTest()
        {
            var x = engine.CreateIntegerVector(new[] { 1, 2, 3 });
            var y = engine.CreateIntegerVector(new[] { 1, 2, 3 });
            engine.SetSymbol("x", x);
            engine.SetSymbol("y", y);
            engine.Evaluate("plot(x,y)");
        }

        [ClassCleanup]
        public static void Teardown()
        {
            engine.Dispose();
        }
    }
}
