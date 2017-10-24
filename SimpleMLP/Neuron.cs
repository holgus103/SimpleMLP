using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        protected class Neuron : INeuron
        {
            public Neuron() { }
            public Neuron(double initialValue)
            {
                this.neuronOutput = initialValue;
            }

            public Neuron(IActivation activationFunction, INetwork networkType)
            {
                this.activationFunction = activationFunction;
                if (networkType is null)
                    this.networkType = new ClassificationNetwork();
                else
                    this.networkType = networkType;
            }

            public double Output => this.neuronOutput;

            public void AddToForwardDelta(double delta)
            {
                this.delta += delta;
            }

            protected IActivation activationFunction;
            protected INetwork networkType;

            private double delta = 0;
            private double neuronOutput;
            private Dictionary<INeuron, double> predecessors = new Dictionary<INeuron, double>();
            private Dictionary<INeuron, double> lastUpdate = new Dictionary<INeuron, double>();
            private double CalculateNetInput() => this.predecessors.Aggregate(0.0, (s, t) => s + t.Key.Output * t.Value);
            // activationFunction
            private double Activate(double val) => 1 / (1 + Math.Exp(-val));

            public void AddPredecessors(List<Tuple<INeuron, double>> incomingNeurons)
            {
                foreach (var incomingNeuronTuple in incomingNeurons)
                {
                    this.predecessors.Add(incomingNeuronTuple.Item1, incomingNeuronTuple.Item2);
                    this.lastUpdate.Add(incomingNeuronTuple.Item1, 0);
                }
            }

            public void AlterWeights(double learningRate, double momentum, Layer layerType)
            {
                
                var o = this.Output;
                var d = this.delta;
                if (networkType is ClassificationNetwork || !(layerType is OutputLayer))
                    d *= o * (1 - o);
                var keys = this.predecessors.Keys.ToList();
                keys.ForEach(val =>
                    {
                        val.AddToForwardDelta(d * this.predecessors[val]);
                        this.predecessors[val] -= momentum * this.lastUpdate[val] + d * val.Output * learningRate;
                        this.lastUpdate[val] = d * val.Output * learningRate;
                    }
                );
                this.delta = 0;
                // normalize weights
                // I HAVE NO IDEA IF THIS IS NECESSARY
                //var sum = this.predecessors.Values.Sum();
                //keys.ForEach(val => this.predecessors[val] = this.predecessors[val] / sum);
            }

            public void CalculateNeuron() => this.neuronOutput = this.activationFunction.Activate(this.CalculateNetInput());

            public List<double> GetPredecessorsWages()
            {
                List<double> predWages = new List<double>();
                foreach (var pred in predecessors)
                {
                    predWages.Add(pred.Value);
                }
                return predWages;
            }
        }
    }

}
