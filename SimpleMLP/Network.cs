using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        private double learningRate;
        private double momentum;
        private List<LayerBase> layers = new List<LayerBase>();
        private InputLayer inputLayer => (InputLayer)this.layers[0];
        private OutputLayer outputLayer => (OutputLayer)this.layers.Last();

        public Network(int inputNeurons, int hiddenNeurons, int outputNeurons, double learningRate, double momentum)
        {
            this.learningRate = learningRate;
            this.momentum = momentum;
            var rand = new Random();
            this.layers.Add(new InputLayer(inputNeurons));
            this.layers.Add(new HiddenLayer(this.layers[0], this.CreateWeightLists(this.layers[0].Count, hiddenNeurons), new Bias() { Value = 1, Wage = rand.NextDouble() }));
            this.layers.Add(new OutputLayer(this.layers[1], this.CreateWeightLists(this.layers[1].Count, outputNeurons), new Bias() { Value = 1, Wage = rand.NextDouble() }));
        }

        private List<List<double>> CreateWeightLists(int prev, int current)
        {
            // prepare list
            List<List<double>> weights = new List<List<double>>(current);
            // foreach neuron in the hidden layer -- lubię ten komentarz - Tomasz Chudzik
            for (var i = 0; i < current; i++)
            {
                Random r = new Random();
                // create random weights 
                var rawWeights = Enumerable.Repeat<int>(0, prev)
                    .Select(e => r.Next(1, 1000));
                var sum = rawWeights.Sum();
                // normalize weights
                var currentWeights = rawWeights.Select(e => (double)e / sum).ToList();
                weights.Add(currentWeights);
            }

            return weights;
        }

        private void CalculateNetwork()
        {
            for (var i = 1; i < this.layers.Count; i++)
            {
                ((Layer)this.layers[i]).CalculateLayer();
            }
        }

        public List<double> Train(List<Tuple<List<double>, List<double>>> trainSet, int iterations)
        {
            var errors = new List<double>(iterations);
            var last = trainSet.Last();
            for (var i = 0; i < iterations; i++)
            {
                double sum = 0;
                trainSet.ForEach(
                    e =>
                    {
                        this.inputLayer.SetInputs(e.Item1);
                        this.CalculateNetwork();
                        sum += this.outputLayer.GetTotalError(e.Item2);
                        this.BackpropagationError(e.Item2);
                    }
                );

                errors.Add(sum / trainSet.Count);
            }
            return errors;
        }

        private void BackpropagationError(List<double> desiredOutputs)
        {

            this.outputLayer.BackpropagateError(desiredOutputs);
            for (var i = this.layers.Count - 1; i > 0; i--)
            {
                this.layers[i].AlterWeights(this.learningRate, this.momentum);
            }
        }

        public List<double> Predict(List<double> inputs)
        {
            this.inputLayer.SetInputs(inputs);
            this.CalculateNetwork();
            return this.outputLayer.GetOutput();
        }

        public int GetClass(List<double> res)
        {
            double max = res[0];
            int index = 0;
            for (var i = 0; i < res.Count; i++)
            {
                if (max < res[i])
                {
                    max = res[i];
                    index = i;
                }
            }
            return index;
        }

    }
}
