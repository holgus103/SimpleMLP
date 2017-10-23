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

            public OutputLayer(LayerBase incomingLayer, List<List<double>> initialWages, IActivation activationFunction, Bias bias = null) : base(incomingLayer, initialWages, activationFunction, bias) { }

            public List<double> GetOutput() => this.neurons.Select(n => n.Output).ToList();
            public double GetTotalError(List<double> desiredOutputs) => 
                this.neurons
                .Zip(desiredOutputs, (f, s) => (f.Output - s) * (f.Output - s))
                .Sum() / 2;                
            
        }
    }
}
