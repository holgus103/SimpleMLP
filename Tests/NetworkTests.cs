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

        [TestMethod]
        public void TrainTest()
        {
            n.Train(new List<Tuple<List<double>, List<double>>>()
            {
                new Tuple<List<double>, List<double>>(

                    // inputs
                    new List<double>()
                    {
                        0.4,
                        0.5
                    },
                    // outputs
                    new List<double>()
                    {
                        1,
                        0
                    }
                ),
                new Tuple<List<double>, List<double>>(
                    new List<double>()
                    {
                        0.1,
                        0.2
                    },
                    new List<double>()
                    {
                        0,
                        1
                    }
                )
            }
            , 100);
        }
    }
}
