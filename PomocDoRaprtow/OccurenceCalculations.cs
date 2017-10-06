using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomocDoRaprtow
{
    class OccurenceCalculations
    {
        private Dictionary<String, int> occurences = new Dictionary<string, int>();
        public OccurenceCalculations(List<Led> leds)
        {
            foreach (var led in leds)
            {
                var model = led.Lot.Model;
                foreach (var testerData in led.TesterData)
                {
                    var idForModel = IdForModel(model, testerData.ShiftNo, testerData.FixedDateTime); 
                    var idForShift = IdForShift(testerData.ShiftNo, testerData.FixedDateTime); 
                    var idForDay = IdForDay(testerData.FixedDateTime); 
                    var idForWeek = IdForWeek(testerData.FixedDateTime);
                    
                    increment(idForModel);
                    increment(idForShift);
                    increment(idForDay);
                    increment(idForWeek);
                }
            }
        }

        public int GetOccurenceForModel(Led led, TesterData testerData)
        {
            var id = IdForModel(led.Lot.Model, testerData.ShiftNo, testerData.FixedDateTime);
            return occurences[id];
        }

        public int GetOccurenceForShift(TesterData testerData)
        {
            var id = IdForShift(testerData.ShiftNo, testerData.FixedDateTime);
            return occurences[id];
        }

        public int GetOccurenceForDay(TesterData testerData)
        {
            var id = IdForDay(testerData.FixedDateTime);
            return occurences[id];
        }

        public int GetOccurenceForWeek(TesterData testerData)
        {
            var id = IdForWeek(testerData.FixedDateTime);
            return occurences[id];
        }

        private string IdForModel(String model, int shiftNo, DateTime fixedDate)
        {
            return model + IdForShift(shiftNo, fixedDate);
        }

        private string IdForShift(int shiftNo, DateTime fixedDate)
        {
            return fixedDate.Year.ToString() + fixedDate.Day + shiftNo;
        }

        private string IdForDay(DateTime fixedDate)
        {
            return fixedDate.Year.ToString() + fixedDate.Day;
        }

        private string IdForWeek(DateTime fixedDate)
        {
            return DateUtilities.GetRealWeekOfYear(fixedDate).ToString();
        }

        private void increment(String id)
        {
            int val = 0;
            occurences.TryGetValue(id, out val);
            occurences[id] = val + 1;
        }
    }
}
