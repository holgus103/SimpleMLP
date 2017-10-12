using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        class InputLayer : Layer
        {
            public InputLayer(int neuronsNumber) : base(neuronsNumber) { }
            public InputLayer(List<double> initialValues) : base(initialValues) { }
        }
    }
}
