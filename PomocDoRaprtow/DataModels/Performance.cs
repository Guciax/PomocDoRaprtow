using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomocDoRaprtow.DataModels
{
    public class PerformanceTest
    {
        public PerformanceTest(double outputPerHour, int mintesLotDuration)
        {
            OutputPerHour = outputPerHour;
            MintesLotDuration = mintesLotDuration;
        }

        public double OutputPerHour { get; set; }
        public double MintesLotDuration { get; set; }
    }
}
