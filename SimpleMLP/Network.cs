using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        //private Layer inputLayer;
        //private Layer outputLayer;
        private List<Layer> layers = new List<Layer>();

        public Network(int inputNeurons, int hiddenNeurons, int outputNeurons)
        {
            var rand = new Random();
            this.layers.Add(new InputLayer(inputNeurons));
            this.layers.Add(new HiddenLayer(this.layers[0], createWeightLists(this.layers[0].Count, hiddenNeurons), new Bias() { Value = 1, Wage = rand.NextDouble() }));
            this.layers.Add(new OutputLayer(this.layers[1], createWeightLists(this.layers[1].Count, outputNeurons), new Bias() { Value = 1, Wage = rand.NextDouble() }));
        }

        private List<List<double>> createWeightLists(int prev, int current)
        {
            // prepare list
            List<List<double>> weights = new List<List<double>>(current);
            // foreach neuron in the hidden layer
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

        public void Train()
        {
            // TODO: everything, kek
        }
    }
}
