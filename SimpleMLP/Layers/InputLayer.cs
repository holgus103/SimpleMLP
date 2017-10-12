using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        private class InputLayer : LayerBase
        {
            private class InputNeuron : INeuron
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
            }

            public InputLayer(int neuronsNumber) : this(Enumerable.Repeat<double>(0, neuronsNumber).ToList())
            {
                
            }

            public InputLayer(List<double> initialValues) : base(initialValues.Count)
            {
                for(var i = 0; i< initialValues.Count; i++)
                {
                    this.neurons.Add(new InputNeuron(initialValues[i]));
                }

            }

            public void SetInputs(List<double> inputs)
            {
                if (inputs.Count != this.neurons.Count)
                {
                    throw new Exception("Invalid number of inputs");
                }

                for (var i = 0; i < inputs.Count; i++)
                {
                    ((InputNeuron)this.neurons[0]).Set(inputs[i]);
                }
            }
        }
        

    }
}
