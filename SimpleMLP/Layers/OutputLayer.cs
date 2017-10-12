using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        private class OutputLayer : HiddenLayer
        {
            public OutputLayer(LayerBase incomingLayer, List<List<double>> initialWages, Bias bias = null) : base(incomingLayer, initialWages, bias) { }

            public List<double> GetOutput() => this.neurons.Select(n => n.Output).ToList();
            public void AlterWeights(List<double> desiredOutput)
            {
                for (var i = 0; i < this.neurons.Count; i++)
                {
                    var n = this.neurons[i];
                    var t = desiredOutput[i];
                    var d = (t - n.Output) * t*(1 - t);
                    ((Neuron)n).AlterWeight(d);
                }
            }
        }
    }
}
