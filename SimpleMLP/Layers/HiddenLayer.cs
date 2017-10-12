using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        private class HiddenLayer : Layer
        {
            List<Tuple<INeuron, double>> incomingLayerTuple;
            public HiddenLayer(LayerBase incomingLayer, List<List<double>> initialWages, Bias bias = null) : base(initialWages.Count, bias)
            {
                for (int i = 0; i < neurons.Count; i++)
                {
                    incomingLayerTuple = new List<Tuple<INeuron, double>>();
                    for (int j = 0; j < incomingLayer.Neurons.Count; j++)
                    {
                        incomingLayerTuple.Add(new Tuple<INeuron, double>(incomingLayer.Neurons[j], initialWages[i][j]));
                    }
                    ((Neuron)neurons[i]).AddPredecessors(incomingLayerTuple);
                }
            }
        }
    }
}
