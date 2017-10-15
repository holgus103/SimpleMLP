using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataParser
{
    public class CsvParser
    {
        public static List<Tuple<List<double>, List<double>>> Parse(string path, int classes) =>
             File.ReadAllLines(path)
                .Skip(1)
                .Select(e => e.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(a =>
                 {
                     var cls = Int32.Parse(a[2]);
                     var outputs = new List<double>(classes);
                     for (var i = 1; i < cls; i++)
                     {
                         outputs.Add((double)0);
                     }
                     outputs.Add((double)1);
                     for (var i = 0; i < classes - cls; i++)
                     {
                         outputs.Add((double)0);
                     }

                     return new Tuple<List<double>, List<double>>
                     (
                         new List<double>() {Double.Parse(a[0].Replace(".", ",")), Double.Parse(a[1].Replace(".", ","))},
                         outputs
                     );
                 })
                .ToList();
    }
}
