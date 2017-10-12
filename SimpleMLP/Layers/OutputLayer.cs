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
            
        }
    }
}
