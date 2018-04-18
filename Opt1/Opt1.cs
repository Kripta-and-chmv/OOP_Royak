using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interface;
using System.Composition;

namespace Opt1
{
    [Export(typeof(IOptimizator))]
    public class Opt : IOptimizator
    {
        public string Name => "Opt1";

        public IFunction Function { get; set; }
        public List<Tuple<double, double>> Points { get; set; }

        public List<Tuple<double, double>> Optimize()
        {

            return new List<Tuple<double, double>> { new Tuple<double, double> (1, 2), new Tuple<double, double>(2, 2) };
        }
    }
}
