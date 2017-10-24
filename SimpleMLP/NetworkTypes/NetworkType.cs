using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataParser;

namespace SimpleMLP
{
    public interface INetwork
    {
        List<Tuple<List<double>, List<double>>> Parse(string path, out int classes, out int attributesCount);
    }

}
