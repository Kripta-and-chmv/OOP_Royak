using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2
{
    class Functions
    {
        static private Dictionary<string, Func<List<Point>>> functions = new Dictionary<string, Func<List<Point>>>
            {
                {"func1", QuadricSpline}
            };

        static public List<string> All
        {
            get
            {
                return new List<string> { "func1" };
            }
        }

        static Func<List<Point>> getFunction(string name)
        {
            return functions[name];
        }

        static private List<Point> QuadricSpline()
        {
            return new List<Point> { new Point(0, 0) };
        }
    }
}
