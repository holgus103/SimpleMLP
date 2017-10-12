using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        protected abstract class Layer
        {
            public List<Neuron> Neurons => neurons;
            protected List<Neuron> neurons;
            public int Count => this.neurons.Count;

            public Layer(int neuronsNumber, Bias bias = null)
            {
                neurons = new List<Neuron>();
                for (int i = 0; i < neuronsNumber; i++)
                {
                    neurons.Add(new Neuron());
                }
                if (bias != null)
                    foreach (var neuron in neurons)
                    {
                        neuron.AddPredecessors(new List<Tuple<Neuron, double>>() { new Tuple<Neuron, double>(new Neuron(bias.Value), bias.Wage) });
                    }
            }
        }
    }
}
