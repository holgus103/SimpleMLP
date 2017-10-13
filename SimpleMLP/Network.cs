﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        private List<LayerBase> layers = new List<LayerBase>();
        private InputLayer inputLayer => (InputLayer) this.layers[0];
        private OutputLayer outputLayer => (OutputLayer) this.layers.Last();

        public Network(int inputNeurons, int hiddenNeurons, int outputNeurons)
        {
            var rand = new Random();
            this.layers.Add(new InputLayer(inputNeurons));
            this.layers.Add(new HiddenLayer(this.layers[0], this.createWeightLists(this.layers[0].Count, hiddenNeurons), new Bias() { Value = 1, Wage = rand.NextDouble() }));
            this.layers.Add(new OutputLayer(this.layers[1], this.createWeightLists(this.layers[1].Count, outputNeurons), new Bias() { Value = 1, Wage = rand.NextDouble() }));
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

        private void calculateNetwork()
        {
            for(var i = 1; i < this.layers.Count; i++)
            {
                ((Layer)this.layers[i]).CalculateLayer();
            }
        }

        public void Train(List<Tuple<List<double>, List<double>>> trainSet, int iterations)
        {
            for (var i = 0; i < iterations; i++)
            {
                trainSet.ForEach(
                    e =>
                    {
                        this.inputLayer.SetInputs(e.Item1);
                        this.calculateNetwork();
                        this.backpropagageError(e.Item2);
                    }
                );
            }
        }

        private void backpropagageError(List<double> desiredOutputs)
        {
            this.outputLayer.BackpropagateError(desiredOutputs);
            for (var i = this.layers.Count - 1; i > 0; i--)
            {
                this.layers[i].AlterWeights();
            }
        }

        public List<double> Predict(List<double> inputs)
        {
            this.inputLayer.SetInputs(inputs);
            this.calculateNetwork();
            return this.outputLayer.GetOutput();
        }
    }
}
