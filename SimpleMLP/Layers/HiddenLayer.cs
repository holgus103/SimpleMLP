using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        class HiddenLayer : Layer
        {
            List<Tuple<Neuron, double>> incomingLayerTuple;
            public HiddenLayer(Layer incomingLayer, List<double> initialValues, List<List<double>> initialWages, Bias bias = null) : base(initialValues, bias)
            {

                //foreach (var incomingNeuron in incomingLayer.Neurons)
                //{
                //    incomingLayerTuple.Add(new Tuple<Neuron, double>(incomingNeuron, initialWage));
                //}
                for (int i = 0; i < neurons.Count; i++)
                {
                    incomingLayerTuple = new List<Tuple<Neuron, double>>();
                    for (int j = 0; j < incomingLayer.Neurons.Count; j++)
                    {
                        incomingLayerTuple.Add(new Tuple<Neuron, double>(incomingLayer.Neurons[j], initialWages[i][j]));
                    }
                    neurons[i].AddPredecessors(incomingLayerTuple);
                }
            }
        }
    }
}
