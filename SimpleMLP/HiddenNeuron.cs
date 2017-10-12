using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        private class HiddenNeuron : INeuron
        {
            private bool isSynced; 
            private double output; 
            public double Output
            {
                get
                {
                    if (this.isSynced) return this.output;
                    this.output = this.activate(this.calculateNetInput());
                    this.isSynced = true; ;
                    return this.output;
                }
            };
            private List<Tuple<INeuron, double>> predecessors = new List<Tuple<INeuron, double>>();
            private double calculateNetInput() => this.predecessors.Aggregate(0.0, (s, t) => s + t.Item1.Output * t.Item2);
            // activationFunction
            private double activate(double val) => 1 / (1 + Math.Exp(-val));
            public void AlterWeights()
            {

            }
        }
    }

}
