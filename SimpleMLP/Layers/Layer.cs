﻿using System;
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
            protected Neuron GetNeuron(int index) => (Neuron)this.neurons[index];

            protected Layer(int neuronsNumber, IActivation activationFunction, INetwork networkType,  Bias bias = null) : base(neuronsNumber)
            {
                this.networkType = networkType;
                if (activationFunction is null)
                    this.activationFunction = new SigmoidFunction();
                if (this is OutputLayer && networkType is RegressionNetwork)
                    this.activationFunction = new IdentityFunction();
                else
                    this.activationFunction = activationFunction;
                //if (!(activationFunction is null))
                //    this.activationFunction = activationFunction;
                for (int i = 0; i < neuronsNumber; i++)
                {
                    this.neurons.Add(new Neuron(activationFunction, this.networkType));
                }
                if (bias == null) return;
                //neurons.Add(new InputNeuron(bias.Value));
                foreach (var neuron in this.neurons)
                {
                    ((Neuron)neuron).AddPredecessors(new List<Tuple<INeuron, double>>() { new Tuple<INeuron, double>(new InputNeuron(bias.Value), bias.Wage) });
                }
            }

            public void CalculateLayer()
            {
                for (var i = 0; i < this.neurons.Count; i++)
                {
                    this.GetNeuron(i).CalculateNeuron();
                }
            }

            public override void AlterWeights(double learningRate, double momentum)
            {
                this.neurons.ForEach(n => n.AlterWeights(learningRate, momentum, this));
            }
        }
    }
}
