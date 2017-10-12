using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleMLP;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class NetworkTests
    {
        private static Network n = new Network(2, 2, 2);
        [TestInitialize]

        [TestMethod]
        public void BasicTest()
        {
            var val = n.Predict(new List<double>(){1, 0});
        }
    }
}
