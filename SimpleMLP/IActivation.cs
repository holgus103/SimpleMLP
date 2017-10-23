using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Encog.Engine.Network.Activation;

namespace SimpleMLP
{
    public interface IActivation
    {
        double Activate(int val);
        IActivationFunction GetEncogActivationFunction();
    }

    public class SigmoidFunction : IActivation
    {
        public double Activate(int val) => 1 / (1 + Math.Exp(-val));

        public IActivationFunction GetEncogActivationFunction() => new ActivationSigmoid();
    }

    public class ReLUFunction : IActivation
    {
        public double Activate(int val) => Math.Max(0, val);

        public IActivationFunction GetEncogActivationFunction() => new ActivationSigmoid();
    }

}
