using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataParser
{
    public class CsvParser
    {
        public static List<Tuple<List<double>, List<double>>> Parse(string path) =>
             File.ReadAllLines(path)
                .Skip(1)
                .Select(e => e.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(a => new Tuple<List<double>, List<double>>
                    (
                        new List<double>() { Double.Parse(a[0]), Double.Parse(a[1]) },
                        new List<double>() { Double.Parse(a[2]) }
                    )
                )
                .ToList();
    }
}
