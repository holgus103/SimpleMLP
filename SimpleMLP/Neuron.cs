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
                this.initialValue = initialValue;
            }

            public double Output
            {
                get
                {
                    if (predecessors.Count > 0)
                    {
                        if (!this.isSynced)
                        {
                            this.initialValue = this.initialValue = this.activate(this.calculateNetInput());
                            this.isSynced = true;
                        }
                    }
                    return initialValue;
                }
            }

            private bool isSynced = false;
            private double initialValue;
            private Dictionary<INeuron, double> predecessors = new Dictionary<INeuron, double>();
            private double calculateNetInput() => this.predecessors.Aggregate(0.0, (s, t) => s + t.Key.Output * t.Value);
            // activationFunction
            private double activate(double val) => 1 / (1 + Math.Exp(-val));

            public void AddPredecessors(List<Tuple<INeuron, double>> incomingNeurons)
            {
                foreach (var incomingNeuronTuple in incomingNeurons)
                {
                    predecessors.Add(incomingNeuronTuple.Item1, incomingNeuronTuple.Item2);
                }
            }
            
            public void AlterWeight(double delta)
            {
                var o = this.Output;
                var d = delta * o * (1 - o);
                foreach(var val in this.predecessors.Keys)
                {
                    
                    this.predecessors[val] -= this.predecessors[val] * d;
                }
            }
        }
    }

}
