using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network : NetworkBase
    {
        private double learningRate;
        private double momentum;
        private List<LayerBase> layers = new List<LayerBase>();
        private InputLayer inputLayer => (InputLayer)this.layers[0];
        private OutputLayer outputLayer => (OutputLayer)this.layers.Last();
        private INetwork networkType;


        public override NetworkBase BuildNetwork(int inputNeurons, List<int> hiddenNeurons, int outputNeurons, double learningRate, double momentum,
            IActivation activationFunction, INetwork networkType)
        {
            this.networkType = networkType;
            this.activationFunction = activationFunction;
            this.learningRate = learningRate;
            this.momentum = momentum;
            var rand = new Random();
            this.layers.Add(new InputLayer(inputNeurons));
            for(var i = 0; i < hiddenNeurons.Count; i++)
            {
                this.layers.Add(new HiddenLayer(this.layers[i], this.CreateWeightLists(this.layers[i].Count, hiddenNeurons[i]),
                    new SigmoidFunction(), this.networkType, new Bias() { Value = 1, Wage = rand.NextDouble() }));
            }

            this.layers.Add(new OutputLayer(this.layers.Last(), this.CreateWeightLists(this.layers.Last().Count, outputNeurons),
                activationFunction, this.networkType, new Bias() { Value = 1, Wage = rand.NextDouble() }));
            return this;
        }

        public NetworkBase BuildNetwork(List<List<List<double>>> wages, double learningRate, double momentum, List<double> biasWage, IActivation activationFunction)
        {
            this.activationFunction = activationFunction;
            this.learningRate = learningRate;
            this.momentum = momentum;
            this.layers.Add(new InputLayer(wages[0][0].Count));
            for(var i = 0; i< wages.Count - 1; i++)
            {
                this.layers.Add(new HiddenLayer(this.layers[i], wages[i], new SigmoidFunction(), networkType, new Bias() { Value = 1, Wage = biasWage[i] }));
            }

            this.layers.Add(new OutputLayer(this.layers.Last(), wages.Last(), activationFunction, networkType, new Bias() { Value = 1, Wage = biasWage.Last() }));
            return this;
        }

        public List<double> GetWages()
        {
            List<double> wages = new List<double>();
            foreach (var layer in layers)
            {
                foreach (var neuron in layer.Neurons)
                {
                    if (neuron is Neuron n)
                    {
                        foreach (var wage in n.GetPredecessorsWages())
                        {
                            wages.Add(wage);
                        }
                    }
                }
            }
            return wages;
        }

        private List<List<double>> CreateWeightLists(int prev, int current)
        {
            // prepare list
            List<List<double>> weights = new List<List<double>>(current);
            // foreach neuron in the hidden layer -- lubię ten komentarz - Tomasz Chudzik <Like>
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

        public override List<double> Train(List<Tuple<List<double>, List<double>>> trainSet, int iterations)
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
                        this.BackpropagationError(e.Item2);
                        sum += this.outputLayer.GetTotalError(e.Item2);
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

        public override List<double> Predict(List<double> inputs)
        {
            this.inputLayer.SetInputs(inputs);
            this.CalculateNetwork();
            return this.outputLayer.GetOutput();
        }

    }
}
