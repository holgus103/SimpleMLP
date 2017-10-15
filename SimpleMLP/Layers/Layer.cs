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
            protected Neuron getNeuron(int index) => (Neuron) this.neurons[index];

            protected Layer(int neuronsNumber, Bias bias = null) : base(neuronsNumber)
            {
                for (int i = 0; i < neuronsNumber; i++)
                {
                    this.neurons.Add(new Neuron());
                }
                if (bias == null) return;
                foreach (var neuron in this.neurons)
                {
                    ((Neuron)neuron).AddPredecessors(new List<Tuple<INeuron, double>>() { new Tuple<INeuron, double>(new InputNeuron(bias.Value), bias.Wage) });
                }
            }

            public void CalculateLayer()
            {
                Parallel.For(0, this.neurons.Count, i => this.getNeuron(i).CalculateNeuron());
            }

            public override void AlterWeights(double eta)
            {
                this.neurons.ForEach(n => n.AlterWeights(eta));
            }
        }
    }
}
