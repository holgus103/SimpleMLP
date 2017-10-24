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
        CsvData Parse(string path, ref int classes, ref int attributesCount, bool attributesOnly);
    }

}
