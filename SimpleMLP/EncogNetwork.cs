using Encog.Engine.Network.Activation;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    class EncogNetwork
    {
        private BasicNetwork network;

        public EncogNetwork()
        {
            this.network = new BasicNetwork();

            //this.network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 2));
            //this.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 4));
            //this.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 1));
        }
    }
}
