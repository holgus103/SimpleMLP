using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        protected class Neuron
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
                        return this.activate(this.calculateNetInput());
                    else return initialValue;
                }
            }
            private double initialValue;
            private List<Tuple<Neuron, double>> predecessors = new List<Tuple<Neuron, double>>();
            private double calculateNetInput() => this.predecessors.Aggregate(0.0, (s, t) => s + t.Item1.Output * t.Item2);
            // activationFunction
            private double activate(double val) => 1 / (1 + Math.Exp(-val));

            public void AddPredecessors(List<Tuple<Neuron, double>> incomingNeurons)
            {
                foreach (var incomingNeuronTuple in incomingNeurons)
                {
                    predecessors.Add(incomingNeuronTuple);
                }
            }
        }
    }

}
