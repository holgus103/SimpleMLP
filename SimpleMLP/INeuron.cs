﻿using System;
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
            void AlterWeights(double eta);
            void AddToForwardDelta(double delta);
            double CalculateNewWeight(double d, double weight);
        }
    }

}
