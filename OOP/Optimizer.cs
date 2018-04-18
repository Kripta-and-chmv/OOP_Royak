using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2
{
    class Optimizer
    {
        static public List<List<Point>> optimize(string functionNamw,
                                           string methodName,
                                           List<Point> dots)
        {
            var lines = new List<List<Point>>();
            for(int y = 0; y < 3; y++)
            {
                var buf = new List<Point>();
                for (int x = -10; x < 11; x++)
                    buf.Add(new Point(x, y));
                lines.Add(buf);
            }
            return lines;
        }
    }
}
