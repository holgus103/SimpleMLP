using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        protected abstract class Layer : LayerBase
        {
            public Layer(int neuronsNumber, Bias bias = null) : base(neuronsNumber)
            {
                neurons = new List<INeuron>();
                for (int i = 0; i < neuronsNumber; i++)
                {
                    neurons.Add(new Neuron());
                }
                if (bias != null)
                    foreach (var neuron in neurons)
                    {
                        ((Neuron)neuron).AddPredecessors(new List<Tuple<INeuron, double>>() { new Tuple<INeuron, double>(new Neuron(bias.Value), bias.Wage) });
                    }
            }

            public void CalculateLayer()
            {
                double o;
                foreach (var n in this.neurons)
                {
                    o = n.Output;
                }
            }
        }
    }
}
