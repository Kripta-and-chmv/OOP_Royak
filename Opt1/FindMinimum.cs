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
        public bool needPoints => false;
        public bool needBorders => true;

        public IFunction Function { get; set; }
        public List<Tuple<double, double>> Points { get; set; }

        double from;
        double to;
        public Tuple<double, double> Borders
        {
            set
            {
                from = value.Item1;
                to = value.Item2;
            }
        }

        public List<Tuple<double, double>> Optimize()
        {
            List<Tuple<double, double>> result = new List<Tuple<double, double>>();
            var val = to - from;
            for (int i = 0; i < 100; ++i)
            {
                double x = from + val * i / 100;
                result.Add(new Tuple<double, double>(x, Function.Calculate(x)));
            }
            result.Add(new Tuple<double, double>(to, Function.Calculate(to)));
            return result;
        }

        double EPS = 0.0005;
        double DELTA = 0.0002;

        void Dih()
        {
            double x1;
            double x2;
            double true_b = to;
            while (Math.Abs(to - from) > EPS)
            {
                x1 = (from + to - DELTA) / 2;
                x2 = (from + to + DELTA) / 2;
                if (Function.Calculate(x1) < Function.Calculate(x2))
                    to = x2;
                else
                    from = x1;
            }
        }
    }
}
