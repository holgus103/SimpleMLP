using System;
using System.Collections.Generic;

namespace SimpleMLP
{
    public abstract class NetworkBase
    {
        public abstract NetworkBase BuildNetwork(int inputNeurons, List<int> hiddenNeurons, int outputNeurons, double learningRate, double momentum,
            IActivation activationFunction, INetwork networkType);
        public abstract List<double> Predict(List<double> inputs);
        public abstract List<double> Train(List<Tuple<List<double>, List<double>>> trainSet, int iterations);
        protected IActivation activationFunction;

        public static int GetClass(List<double> res)
        {
            double max = res[0];
            int index = 0;
            for (var i = 0; i < res.Count; i++)
            {
                if (max < res[i])
                {
                    max = res[i];
                    index = i;
                }
            }
            return index;
        }
    }
}