using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interface;
using System.Composition;

namespace Opt2
{
    [Export(typeof(IOptimizator))]
    public class Interpolation : IOptimizator
    {
        //Points and function values
        List<double> _p = new List<double>();
        List<double> _fp = new List<double>();

        //Coefficients for Lagrange polynomial
        List<double> _l = new List<double>();

        //Delegates for needed parameters
        delegate double Operation(int x);

        public IFunction Function { get; set; }

        public List<Tuple<double, double>> Points
        {
            set
            {
                foreach (Tuple<double, double> point in value)
                {
                    _p.Add(point.Item1);
                    _fp.Add(point.Item2);
                }

                for (int i = 0; i < value.Count; i++)
                    _l.Add(_fp[i] / Product(Enumerable.Range(0, _p.Count).
                        Where(j => j != i), j => _p[i] - _p[j]));
            }
            get
            {
                return new List<Tuple<double, double>>();
            }
        }

        //Lagrange's formula
        public double Lp(double x)
        {
            return Enumerable.Range(0, _p.Count).Sum(i => _l[i]
               * Product(Enumerable.Range(0, _p.Count).
               Where(j => j != i), j => x - _p[j]));
        }

        //Auxiliary functions
        static double Product(IEnumerable<int> values, Operation oper)
        {
            return values.Aggregate(1D, (current, v) => current * oper(v));
        }

        public string Name => "Interpolation";
        
        public List<Tuple<double, double>> Optimize()
        {
            double start = _p[1];
            double end = _p[_p.Count - 1];
            List<Tuple<double, double>> result = new List<Tuple<double, double>>();
            for (int i = -10; i < 11; ++i)
                result.Add(new Tuple<double, double>(i, Lp(i)));
            return result;
        }
    }
}
