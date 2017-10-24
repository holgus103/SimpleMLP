using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataParser
{
    public class CsvParser
    {
        public static CsvData Parse(string path, ref int classes, ref int attributesCount, bool attributesOnly = false)
        {
            var data = File.ReadAllLines(path)
                .Skip(1)
                .Select(e => e.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries));
            int classesCount;
            if (attributesOnly)
            {
                classesCount = classes;
                attributesCount = data.First().Length;
            }
            else
            {
                attributesCount = data.First().Length - 1;
                classesCount = data.Select(e => e[e.Length - 1]).Distinct().Count();
                classes = classesCount;
            }
            var output = data.Select(a =>
                {
                    List<double> outputs = null;
                    if (!attributesOnly)
                    {
                        var cls = Int32.Parse(a[2]);
                        outputs = new List<double>(classesCount);
                        for (var i = 1; i < cls; i++)
                        {
                            outputs.Add((double)0);
                        }
                        outputs.Add((double)1);
                        for (var i = 0; i < classesCount - cls; i++)
                        {
                            outputs.Add((double)0);
                        }
                    }

                    return new Tuple<List<double>, List<double>>
                    (
                        new List<double>() { Double.Parse(a[0].Replace(".", ",")), Double.Parse(a[1].Replace(".", ",")) },
                        outputs
                    );
                })
                .ToList();

            return new CsvData(output);
        }
    }
}
