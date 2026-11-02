using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public interface IRandomProvider
    {
        int Next(int minValue, int maxValue);
        float NextFloat();
    }
}
