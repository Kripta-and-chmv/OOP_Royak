using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2
{
    class Methods
    {
        static private Dictionary<string, Func<Func<List<Point>>, List<Point>>> methods = new Dictionary<string, Func<Func<List<Point>>, List<Point>>>
            {
                {"quadric spline", MonteKarlo}
            };

        static public List<string> All
        {
            get
            {
                return new List<string> { "MonteKarlo" };
            }
        }

        static Func<Func<List<Point>>, List<Point>> getMethod(string name)
        {
            return methods[name];
        }

        static private List<Point> MonteKarlo(Func<List<Point>> function)
        {
            return new List<Point> { new Point(0, 0) };
        }
    }
}
