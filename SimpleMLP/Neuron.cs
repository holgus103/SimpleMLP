using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        private class Neuron
        {
            public double Output => this.activate(this.calculateNetInput());
            private List<Tuple<Neuron, double>> predecessors = new List<Tuple<Neuron, double>>();
            private double calculateNetInput() => this.predecessors.Aggregate(0.0, (s, t) => s + t.Item1.Output * t.Item2);
            // activationFunction
            private double activate(double val) => 1 / (1 + Math.Exp(-val));
        }
    }

}
