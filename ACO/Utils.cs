using System;
using System.Collections.Generic;
using System.Linq;

namespace ACO
{
    public static class Utils
    {
        private static int seed;

        public static Random Random { get; private set; }
        public static int Seed
        {
            get { return seed; }
            set
            {
                seed = value;
                Random = new Random(seed);
                Console.WriteLine("Seed set to: " + Seed);
            }
        }
        static Utils()
        {
            Seed = (int)DateTime.Now.Ticks;
            Random = new Random(Seed);
        }

        public static int Pow(int baseNumber, int exponent)
        {
            int result = 1;
            while (exponent != 0)
            {
                if ((exponent & 1) == 1)
                {
                    result *= baseNumber;
                }
                exponent >>= 1;
                baseNumber *= baseNumber;
            }

            return result;
        }

        public static double StdDev(this List<Ant> values)
        {
            double ret = 0;
            int count = values.Count;
            if (count > 1)
            {
                //Compute the Average
                double avg = values.Average(a => a.Distance);

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(a => (a.Distance - avg) * (a.Distance - avg));

                //Put it all together
                ret = Math.Sqrt(sum / count);
            }
            return ret;
        }
    }
}
