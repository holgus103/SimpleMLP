using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleMLP;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DataParser;

namespace Tests
{
    [TestClass]
    public class NetworkTests
    {

        [TestMethod]
        public void XorTest()
        {
            Network n = new Network(2, 4, 1, 1, 1);
            n.Train(new List<Tuple<List<double>, List<double>>>()
            {
                new Tuple<List<double>, List<double>>(

                    // inputs
                    new List<double>()
                    {
                        1,
                        1
                    },
                    // outputs
                    new List<double>()
                    {
                        0
                    }
                ),
                new Tuple<List<double>, List<double>>(

                    // inputs
                    new List<double>()
                    {
                        1,
                        0
                    },
                    // outputs
                    new List<double>()
                    {
                        1
                    }
                ),
                new Tuple<List<double>, List<double>>(

                    // inputs
                    new List<double>()
                    {
                        0,
                        1
                    },
                    // outputs
                    new List<double>()
                    {
                        1
                    }
                ),
                new Tuple<List<double>, List<double>>(

                    // inputs
                    new List<double>()
                    {
                        0,
                        0
                    },
                    // outputs
                    new List<double>()
                    {
                        0
                    }
                )
            }
           , 1000);

            var ans = n.Predict(new List<double>()
            {
                1,
                1
            });
            Assert.AreEqual(0, ans[0], 0.10);

            ans = n.Predict(new List<double>()
            {
                1,
                0
            });
            Assert.AreEqual(1, ans[0], 0.1);

            ans = n.Predict(new List<double>()
            {
                0,
                1
            });
            Assert.AreEqual(1, ans[0], 0.1);

            ans = n.Predict(new List<double>()
            {
                0,
                0
            });
            Assert.AreEqual(0, ans[0], 0.1);

        }

        [TestMethod]
        public void ParserTest()
        {
            int classesCount, attributesCount;
            var data = CsvParser.Parse("./../../in.txt", out classesCount, out attributesCount);
            Assert.AreEqual(data.Count, classesCount);
            CollectionAssert.AreEqual(new double[] { 1, 1 }, data[0].Item1);
            CollectionAssert.AreEqual(new double[] { 1, 0, 0 }, data[0].Item2);
            CollectionAssert.AreEqual(new double[] { 2, 1 }, data[1].Item1);
            CollectionAssert.AreEqual(new double[] { 1, 0, 0 }, data[1].Item2);
            CollectionAssert.AreEqual(new double[] { 2, 3 }, data[2].Item1);
            CollectionAssert.AreEqual(new double[] { 1, 0, 0 }, data[2].Item2);
        }

        [TestMethod]
        public void MainTest()
        {
            int classesCount, attributesCount;
            var trainData = CsvParser.Parse("./../../../DataSets/data.train.csv", out classesCount, out attributesCount);
            var testData = CsvParser.Parse("./../../../DataSets/data.train.csv", out classesCount, out attributesCount);

            var n = new Network(attributesCount, 9, classesCount, 1, 1);
            n.Train(trainData, 100);
            var correct = 0;
            testData.ForEach(e =>
            {
                var d = n.Predict(e.Item1);
                if (n.GetClass(d) == n.GetClass(e.Item2))
                {
                    correct++;
                }
            });
        }

    }
}
