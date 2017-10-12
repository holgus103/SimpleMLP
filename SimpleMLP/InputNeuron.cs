using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        class InputNeuron : INeuron
        {
            public double Input { get; set; }
            public double Output => this.Input;
        }
    }
}
