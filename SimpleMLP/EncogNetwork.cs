using Encog.Engine.Network.Activation;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.NeuralData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public class EncogNetwork : NetworkBase
    {
        BasicNetwork network = new BasicNetwork();
        private double momentum;
        private double learningRate;

        public override NetworkBase BuildNetwork(int inputNeurons, List<int> hiddenNeurons, int outputNeurons, double learningRate, double momentum)
        {
            this.momentum = momentum;
            this.learningRate = learningRate;
            this.network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, inputNeurons));
            hiddenNeurons.ForEach(e => network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, e)));
            this.network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, outputNeurons));
            this.network.Structure.FinalizeStructure();
            this.network.Reset();
            return this;
        }

        public override List<double> Predict(List<double> inputs)
        {
            var output = network.Compute(new BasicNeuralData(inputs.ToArray()));
            var o = new List<double>(output.Count);
            for (var i = 0; i < output.Count; i++)
            {
                o.Add(output[i]);
            }
            return o;

        }

        public override List<double> Train(List<Tuple<List<double>, List<double>>> trainSet, int iterations)
        {
            var errors = new List<double>(iterations);
            INeuralDataSet trainingSet = new BasicNeuralDataSet(trainSet.Select(e => e.Item1.ToArray()).ToArray(), trainSet.Select(e => e.Item2.ToArray()).ToArray());
            ITrain train = new Backpropagation(network, trainingSet, this.learningRate, this.momentum);
            var epoch = 0;
            do
            {
                train.Iteration();
                epoch++;
                errors.Add(train.Error);
            } while (epoch < iterations);

            return errors;
        }
    }
}
