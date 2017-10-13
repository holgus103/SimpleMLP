﻿using System;
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
                this.initialValue = initialValue;
            }

            public double Output => this.initialValue;

            public void AddToForwardDelta(double delta)
            {
                this.delta += delta;
            }

            private double delta = 0;
            private double initialValue;
            private Dictionary<INeuron, double> predecessors = new Dictionary<INeuron, double>();
            private double calculateNetInput() => this.predecessors.Aggregate(0.0, (s, t) => s + t.Key.Output * t.Value);
            // activationFunction
            private double activate(double val) => 1 / (1 + Math.Exp(-val));

            public void AddPredecessors(List<Tuple<INeuron, double>> incomingNeurons)
            {
                foreach (var incomingNeuronTuple in incomingNeurons)
                {
                    this.predecessors.Add(incomingNeuronTuple.Item1, incomingNeuronTuple.Item2);
                }
            }

            public void AlterWeights()
            {
                var o = this.Output;
                var d = this.delta * o * (1 - o);
                var keys = this.predecessors.Keys.ToList();
                keys.ForEach(val =>
                    {
                        val.AddToForwardDelta(this.delta * this.predecessors[val]);
                        this.predecessors[val] -= val.Output * d;
                    }
                );
                this.delta = 0;
                // normalize weights
                // I HAVE NO IDEA IF THIS IS NECESSARY
                //var sum = this.predecessors.Values.Sum();
                //keys.ForEach(val => this.predecessors[val] = this.predecessors[val] / sum);
            }

            public void CalculateNeuron()
            {
                this.initialValue = this.activate(this.calculateNetInput());
            }
        }
    }

}
