using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomocDoRaprtow
{
    class MathUtilities
    {
        public static string CalculatePercentage(int total, int fraction)
        {
            double result = Math.Round((double)fraction / total * 100, 4);

            return result.ToString("F2") + "%";
        }

    }
}
