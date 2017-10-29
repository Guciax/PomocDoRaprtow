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
            public Dictionary<Lot, List<ProductionDetail>> ProductionDetails { get; } = new Dictionary<Lot, List<ProductionDetail>>();
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
            public List<OccurenceShift> ShiftToTree { get; } =
                new List<OccurenceShift>() { new OccurenceShift(), new OccurenceShift(), new OccurenceShift() };

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

        public OccurenceTree Tree { get; } = new OccurenceTree();

        public OccurenceCalculations(List<Lot> lots, Func<Lot, IEnumerable<ProductionDetail>> converter)
        {
            foreach (var lot in lots)
            {
                var interestingDates = converter(lot).ToList();
                build(lot, interestingDates);
            }
        }

        private void build(Lot lot, IEnumerable<ProductionDetail> productionDetails)
        {
            var lotModel = lot.Model;

            foreach (var productionDetail in productionDetails)
            {
                var shiftInfo = DateUtilities.DateToShiftInfo(productionDetail.ProductionFixedDate);
                var weekTree = GetWeekTree(productionDetail.ProductionFixedDate);
                var dayTree = GetDayTree(productionDetail.ProductionFixedDate, weekTree);
                var shiftTree = GetShiftTree(shiftInfo.ShiftNo, dayTree);
                var modelTree = GetModelsTree(lotModel.ModelName, shiftTree);

                weekTree.Occurences += productionDetail.ProducedAmount;
                dayTree.Occurences += productionDetail.ProducedAmount;
                shiftTree.Occurences += productionDetail.ProducedAmount;
                modelTree.Occurences += productionDetail.ProducedAmount;

                if(!modelTree.ProductionDetails.ContainsKey(lot))
                {
                    modelTree.ProductionDetails.Add(lot, new List<ProductionDetail>());
                }

                modelTree.ProductionDetails[lot].Add(productionDetail);
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
            var t = dayTree.ShiftToTree[ShiftUtilities.ShiftNoToIndex(shiftNo) - 1];
            t.ShiftNo = shiftNo;

            return t;
        }

        private OccurenceModel GetModelsTree(String model, OccurenceShift shiftTree)
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
    }
}