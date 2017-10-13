using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        private Layer inputLayer;
        private Layer outputLayer;
        private List<Layer> hiddenLayers;

        public Network(int hiddenLayersNumber)
        {
            inputLayer = new InputLayer(new List<double>() { 0.5, 0.1 });
            hiddenLayers = new List<Layer>();
            Layer incomingLayer;
            for (int i = 0; i < hiddenLayersNumber; i++)
            {
                if (i == 0)
                    incomingLayer = inputLayer;
                else
                    incomingLayer = hiddenLayers[i - 1];
                hiddenLayers.Add(new HiddenLayer(incomingLayer, new List<double>() { 0, 0 },
                    new List<List<double>>() { new List<double>() { 0.15, 0.2 }, new List<double>() { 0.25, 0.3 } },
                    new Bias() { Value = 1, Wage=0.35}));
            }
            if (hiddenLayers.Count == 0)
                incomingLayer = inputLayer;
            else
                incomingLayer = hiddenLayers.Last();
            outputLayer = new OutputLayer(incomingLayer, new List<double>() { 0.01, 0.99 },
                new List<List<double>>() { new List<double>() { 0.4, 0.45 }, new List<double>() { 0.5, 0.55 } },
                new Bias() { Value=1, Wage = 0.6});
        }
    }
}
