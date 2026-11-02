using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public class DefaultRandomProvider : IRandomProvider
    {
        private Random _random = new Random();

        public int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        public float NextFloat()
        {
            return (float)_random.NextDouble();
        }
    }
}
