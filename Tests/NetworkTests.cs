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

        [TestMethod]
        public void InternetExampleTest()
        {
            var wages = new List<List<List<double>>>()
            {
                new List<List<double>>()
                {
                    new List<double>(){ 0.15, 0.2},
                    new List<double>(){ 0.25, 0.30}
                },

                new List<List<double>>()
                {
                    new List<double>(){ 0.4, 0.45},
                    new List<double>(){ 0.5, 0.55}
                }
            };
            Network testNetwork = new Network(wages,
                learningRate:0.5, momentum: 1, biasWage: new List<double> { 0.35, 0.6 });
            var trainSet = new List<Tuple<List<double>, List<double>>>()
            {
                new Tuple<List<double>, List<double>>
                (
                  new List<double>(){0.05, 0.1},
                  new List<double>(){0.01, 0.99}
                    )
            };
            testNetwork.Train(trainSet, 1);
            //Assert działa tak: Puść i sprawdź wagi końcowe
        }

    }
}
