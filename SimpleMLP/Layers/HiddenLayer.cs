using System;
using System.CodeDom;
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
            //readonly List<Tuple<INeuron, double>> incomingLayerTuple;
            public HiddenLayer(LayerBase incomingLayer, List<List<double>> initialWages, Bias bias = null) : base(initialWages.Count, bias)
            {
                for (int i = 0; i < this.neurons.Count; i++)
                {
                    var predecessors = new List<Tuple<INeuron, double>>();
                    for (int j = 0; j < incomingLayer.Neurons.Count; j++)
                    {
                        predecessors.Add(new Tuple<INeuron, double>(incomingLayer.Neurons[j], initialWages[i][j]));
                    }
                    this.GetNeuron(i).AddPredecessors(predecessors);
                }
            }

            public void BackpropagateError(List<double> desireOutputs )
            {
                if(desireOutputs.Count != this.neurons.Count) throw new Exception("Invalid outputs number");
                for(var i = 0; i < this.neurons.Count; i++)
                {
                    var n = this.GetNeuron(i);
                    n.AddToForwardDelta(n.Output - desireOutputs[i]);
                }
            }
        }
    }
}
