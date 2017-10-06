using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomocDoRaprtow
{
    class OccurenceCalculations
    {
        private Dictionary<int, int> occurences = new Dictionary<int, int>();
        private Dictionary<int, Dictionary<String, int>> modelOccurences =
            new Dictionary<int, Dictionary<string, int>>();

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

                    IncrementModel(idForModel.Item1, idForModel.Item2);
                    Increment(idForShift);
                    Increment(idForDay);
                    Increment(idForWeek);
                }
            }
        }

        public int GetOccurenceForModel(Led led, TesterData testerData)
        {
            var id = IdForModel(led.Lot.Model, testerData.ShiftNo, testerData.FixedDateTime);
            return modelOccurences[id.Item1][id.Item2];
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

        private Tuple<int, string> IdForModel(String model, int shiftNo, DateTime fixedDate)
        {
            var shiftId = IdForShift(shiftNo, fixedDate);
            return Tuple.Create(shiftId, model);
            //zeby zrobic model to int to moze .. kazdy znaczek na int * 100?
//            return model + IdForShift(shiftNo, fixedDate);
        }

        private int IdForShift(int shiftNo, DateTime fixedDate)
        {
            return IdForDay(fixedDate) * 1000 + shiftNo;
//            return fixedDate.Year.ToString() + fixedDate.Day + shiftNo;
        }

        private int IdForDay(DateTime fixedDate)
        {
            return IdForWeek(fixedDate) * 1000 + fixedDate.Day;
            //return fixedDate.Year.ToString() + fixedDate.Day;
        }

        private int IdForWeek(DateTime fixedDate)
        {
            return DateUtilities.GetRealWeekOfYear(fixedDate);
//            return DateUtilities.GetRealWeekOfYear(fixedDate).ToString();
        }

        private void Increment(int id)
        {
            int val = 0;
            occurences.TryGetValue(id, out val);
            occurences[id] = val + 1;
        }

        private void IncrementModel(int shiftId, String model)
        {
            Dictionary<String, int> modelToOccurence;
            modelOccurences.TryGetValue(shiftId, out modelToOccurence);

            if (modelToOccurence == null)
            {
                modelToOccurence = new Dictionary<string, int>();
                modelOccurences.Add(shiftId, modelToOccurence);
            }

            int val = 0;
            modelToOccurence.TryGetValue(model, out val);
            modelToOccurence[model] = val + 1;
        }
    }
}