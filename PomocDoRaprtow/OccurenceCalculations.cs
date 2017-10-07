using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PomocDoRaprtow
{
    class OccurenceCalculations
    {
        public class OccurenceModel
        {
            public string Model;
            public int Occurences = 0;
        }

        public class OccurenceShift
        {
            public SortedDictionary<string, OccurenceModel> ModelToTree { get; } =
                new SortedDictionary<string, OccurenceModel>();

            public int ShiftNo { get; internal set; }
            public int Occurences { get; internal set; }
        }

        public class OccurenceDay
        {
            public SortedDictionary<int, OccurenceShift> ShiftToTree { get; } =
                new SortedDictionary<int, OccurenceShift>();

            public int Day { get; internal set; }
            public DateTime DateTime { get; internal set; }
            public int Occurences { get; internal set; }
        }

        public class OccurenceTreeWeek
        {
            public SortedDictionary<int, OccurenceDay> DayToTree { get; } = new SortedDictionary<int, OccurenceDay>();
            public int Week { get; internal set; }
            public int Occurences { get; internal set; }
        }

        public class OccurenceTree
        {
            internal SortedDictionary<int, OccurenceTreeWeek> WeekNoToTree { get; } =
                new SortedDictionary<int, OccurenceTreeWeek>();

            public int Occurences { get; internal set; }
        }

        public OccurenceTree Tree { get; }= new OccurenceTree();
        public Dictionary<int, int> CountOccurences { get; } = new Dictionary<int, int>();

        public OccurenceCalculations(List<Led> leds)
        {
            foreach (var led in leds)
            {
                var model = led.Lot.Model;
                Increment(CountOccurences, led.TesterData.Count);
                foreach (var testerData in led.TesterData)
                {
                    var weekTree = GetWeekTree(testerData.FixedDateTime);
                    var dayTree = GetDayTree(testerData.FixedDateTime, weekTree);
                    var shiftTree = GetShiftTree(testerData.ShiftNo, dayTree);
                    var modelTree = GetModeleTree(led.Lot.Model, shiftTree);

                    weekTree.Occurences++;
                    dayTree.Occurences++;
                    shiftTree.Occurences++;
                    modelTree.Occurences++;
                }
            }
        }

        private OccurenceTreeWeek GetWeekTree(DateTime testerDataFixedDateTime)
        {
            var week = DateUtilities.GetRealWeekOfYear(testerDataFixedDateTime);
            var t = GetOrAdd(week, Tree.WeekNoToTree);
            t.Week = week;

            return t;
        }

        private OccurenceDay GetDayTree(DateTime testerDataFixedDateTime, OccurenceTreeWeek weekTree)
        {
            var day = testerDataFixedDateTime.Day;
            var t = GetOrAdd(day, weekTree.DayToTree);
            t.Day = day;
            t.DateTime = testerDataFixedDateTime;

            return t;
        }

        private OccurenceShift GetShiftTree(int shiftNo, OccurenceDay dayTree)
        {
            var t = GetOrAdd(shiftNo, dayTree.ShiftToTree);
            t.ShiftNo = shiftNo;

            return t;
        }

        private OccurenceModel GetModeleTree(String model, OccurenceShift shiftTree)
        {
            var t = GetOrAdd(model, shiftTree.ModelToTree);
            t.Model = model;

            return t;
        }

        private TTree GetOrAdd<TTree, TKey>(TKey key, SortedDictionary<TKey, TTree> trees) where TTree : new()
        {
            TTree tree;
            trees.TryGetValue(key, out tree);

            if (tree == null)
            {
                tree = new TTree();
                trees.Add(key, tree);
            }

            return tree;
        }

        private static void Increment(Dictionary<int, int> occurencesToIncrement, int id)
        {
            int val = 0;
            occurencesToIncrement.TryGetValue(id, out val);
            occurencesToIncrement[id] = val + 1;
        }
    }
}