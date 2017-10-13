using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMLP
{
    public partial class Network
    {
        protected abstract class LayerBase
        {
            public List<INeuron> Neurons => neurons;
            protected List<INeuron> neurons;
            public int Count => this.neurons.Count;

            public LayerBase(int count)
            {
                this.neurons = new List<INeuron>(count);
            }

            public virtual void AlterWeights()
            {
                
            }
        }
    }
}
