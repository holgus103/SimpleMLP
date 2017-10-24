using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        protected interface INeuron
        {
            double Output { get; }
            void AlterWeights(double learningRate, double momentum, Layer layerType);
            void AddToForwardDelta(double delta);
        }
    }

}
