using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Interface
{
    public interface IFunction
    {
        string Name { get; }
        double Calculate();
    }

    public interface IOptimizator
    {
        string Name { get; }
        IFunction Function { get; set; }
        List<Tuple<double, double>> Optimize();
        List<Tuple<double, double>> Points { get; set; }
    }

    public interface IResolver
    {
        void Resolve(IFunction function, IOptimizator optimizator);
    }

}
