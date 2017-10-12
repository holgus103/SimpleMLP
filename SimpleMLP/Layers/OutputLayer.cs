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
            //List<Tuple<Neuron, double>> incomingLayerTuple;
            public OutputLayer(Layer incomingLayer, List<List<double>> initialWages, Bias bias = null) : base(incomingLayer, initialWages, bias) { }
        }
    }
}
