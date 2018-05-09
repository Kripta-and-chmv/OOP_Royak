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
        double Calculate(double x);
    }

    public interface IOptimizator
    {
        string Name { get; }
        IFunction Function { set; }
        List<Tuple<double, double>> Optimize();
        List<Tuple<double, double>> Points { set; }
        Tuple<double, double> Borders { set; }
        bool needPoints { get; }
        bool needBorders { get; }
    }

    public interface IResolver
    {
        void Resolve(IFunction function, IOptimizator optimizator);
    }

}
