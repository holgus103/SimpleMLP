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

            public OutputLayer(LayerBase incomingLayer, List<List<double>> initialWages, IActivation activationFunction, INetwork networkType, Bias bias = null)
                : base(incomingLayer, initialWages, activationFunction, networkType, bias)
            {
                //if (activationFunction is null)
                this.activationFunction = activationFunction;
                this.networkType = networkType;
            }

            public List<double> GetOutput() => this.neurons.Select(n => n.Output).ToList();
            public double GetTotalError(List<double> desiredOutputs)
            {
                if (networkType is ClassificationNetwork || networkType is RegressionNetwork)
                {
                    return
                    this.neurons
                    .Zip(desiredOutputs, (f, s) => (f.Output - s) * (f.Output - s))
                    .Sum() / 2;
                }
                else
                {
                    return desiredOutputs[0] - this.neurons[0].Output;
                }
            }
        }
    }
}
