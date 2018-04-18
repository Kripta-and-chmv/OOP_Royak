using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interface;
using System.Composition;

namespace Func1
{
    [Export(typeof(IFunction))]
    public class Func : IFunction
    {
        public string Name => "Func1";      

        public double Calculate()
        {
            return 0;
        }
    }
}
