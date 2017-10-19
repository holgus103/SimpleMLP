using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        protected abstract class LayerBase
        {

            protected class InputNeuron : INeuron
            {
                private double value;

                public InputNeuron(double val)
                {
                    this.value = val;
                }
                public double Output => this.value;

                public void Set(double val)
                {
                    this.value = val;
                }

                public void AlterWeights(double eta, double momentum)
                {
                }

                public void AddToForwardDelta(double delta)
                {
                }

            }

            public List<INeuron> Neurons => neurons;
            protected List<INeuron> neurons;
            public int Count => this.neurons.Count;

            public LayerBase(int count)
            {
                this.neurons = new List<INeuron>(count);
            }

            public virtual void AlterWeights(double eta, double momentum)
            {
                
            }
        }
    }
}
