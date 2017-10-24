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
        double Activate(double val);
        IActivationFunction GetEncogActivationFunction();
    }

    public class SigmoidFunction : IActivation
    {
        public double Activate(double val) => 1 / (1 + Math.Exp(-val));

        public IActivationFunction GetEncogActivationFunction() => new ActivationSigmoid();
    }

    public class ReLUFunction : IActivation
    {
        public double Activate(double val) => Math.Max(0, val);

        public IActivationFunction GetEncogActivationFunction() => new ActivationSigmoid();
    }

    public class IdentityFunction : IActivation
    {
        public double Activate(double val) => val;

        public IActivationFunction GetEncogActivationFunction() => new ActivationSigmoid();
    }

}
