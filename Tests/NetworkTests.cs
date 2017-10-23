using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleMLP;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DataParser;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Engine.Network.Activation;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Data.Basic;
using Encog.ML.Data;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Training.Propagation.Back;

namespace Tests
{
    [TestClass]
    public class NetworkTests
    {
        [TestMethod]
        public void XorTest()
        {
            var n = new Network().BuildNetwork(2, new List<int>() { 4 }, 1, 1, 1, new SigmoidFunction());
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
            var data = CsvParser.Parse("./../../in.txt", out classesCount, out attributesCount).NormalizedData;
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
            var trainData = CsvParser.Parse("./../../../DataSets/data.train.csv", out classesCount, out attributesCount).NormalizedData;
            var testData = CsvParser.Parse("./../../../DataSets/data.train.csv", out classesCount, out attributesCount).NormalizedData;

            var n = new Network().BuildNetwork(attributesCount, new List<int>() { 4, 3 }, classesCount, 1, 1, new SigmoidFunction());
            n.Train(trainData, 1000);
            var correct = 0;
            testData.ForEach(e =>
            {
                var d = n.Predict(e.Item1);
                if (Network.GetClass(d) == Network.GetClass(e.Item2))
                {
                    correct++;
                }
            });
        }


        [TestMethod]
        public void MainTestEncog()
        {
            int classesCount;
            int attributesCount;
            var epoch = 0;
            var trainData = CsvParser.Parse("./../../../DataSets/data.train.csv", out classesCount, out attributesCount).NormalizedData;
            var testData = CsvParser.Parse("./../../../DataSets/data.train.csv", out classesCount, out attributesCount).NormalizedData;
            var correct = 0;

            var network = new BasicNetwork();

            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 2));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 4));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 5));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 3));
            network.Structure.FinalizeStructure();
            network.Reset();

            INeuralDataSet trainingSet = new BasicNeuralDataSet(trainData.Select(e => e.Item1.ToArray()).ToArray(), trainData.Select(e => e.Item2.ToArray()).ToArray());
            ITrain train = new Backpropagation(network, trainingSet, 0.3, 0.6);
            //ITrain train = new ResilientPropagation(network, trainingSet);

            do
            {
                train.Iteration();
                epoch++;
            } while ((epoch < 15000) && (train.Error > 0.001));

            foreach (IMLDataPair pair in trainingSet)
            {
                var output = network.Compute(pair.Input);
                //pair.Ideal
                if(Network.GetClass(new List<double>(){ output[0], output[1], output[2]}) == Network.GetClass(new List<double>() { pair.Ideal[0], pair.Ideal[1], pair.Ideal[2]}))
                {
                    correct++;
                }
            }
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
            var testNetwork = new Network().BuildNetwork(wages,
                learningRate:0.5, momentum: 1, biasWage: new List<double> { 0.35, 0.6 }, activationFunction: new SigmoidFunction());
            var trainSet = new List<Tuple<List<double>, List<double>>>()
            {
                new Tuple<List<double>, List<double>>
                (
                  new List<double>(){0.05, 0.1},
                  new List<double>(){0.01, 0.99}
                    )
            };
            testNetwork.Train(trainSet, 1);

            List<double> expectedWages = new List<double>()
            {
                0.1498, 0.1996, 0.2498, 0.2995,
                0.3589, 0.4087, 0.5113, 0.5614
            };
            if (testNetwork is Network simpleNetwork)
            {
                var networkWages = simpleNetwork.GetWages();
                for (int i = 0, k = 0; i < networkWages.Count; i++)
                {
                    if (i % 3 == 0)
                        continue;
                    Assert.AreEqual(expectedWages[k], networkWages[i], 0.0001);
                    k++;
                }
            }
            //Assert działa tak: Puść i sprawdź wagi końcowe
        }

    }
}
