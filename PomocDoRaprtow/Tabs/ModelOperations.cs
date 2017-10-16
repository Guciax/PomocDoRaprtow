using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PomocDoRaprtow.Tabs
{
    public class ModelOperations
    {
        public LedStorage ledStorage { get; set; }
        private Form1 form;
        private readonly Chart chartModel;
        private readonly DataGridView dataGridViewModelInfo;

        public ModelOperations(Form1 form, Chart chartModel, DataGridView dataGridViewModelInfo)
        {
            this.form = form;
            this.chartModel = chartModel;
            this.dataGridViewModelInfo = dataGridViewModelInfo;
        }

        public void GenerateCycleTimeChart(Model model)
        {
            var testerIdToTesterDates = PrepareTesterData(model);
            Dictionary<String, SortedDictionary<double, int>> cyclesOccurence = new Dictionary<string, SortedDictionary<double, int>>();

            foreach (var entryTesterIdTesterData in testerIdToTesterDates)
            {
                var testerId = entryTesterIdTesterData.Key;
                var testerDatas = entryTesterIdTesterData.Value;
                var occurences = new SortedDictionary<double, int>();
                cyclesOccurence.Add(testerId, occurences);
                for (int i = 1; i < testerDatas.Count; i++)
                {
                    var cycleDuration = (int) (testerDatas[i] - testerDatas[i - 1]).TotalSeconds;
                    if (cycleDuration < 5 || cycleDuration>240) continue;

                    var count = 0;
                    occurences.TryGetValue(cycleDuration, out count);
                    occurences[cycleDuration] = count + 1;
                }
            }

            Charting.CycleTimeHistogram(chartModel, cyclesOccurence);
        }

        public void GenerateLotEfficiencyChart(Model model)
        {
            dataGridViewModelInfo.Rows.Clear();
            Dictionary<String, SortedDictionary<double, int>> lotCyclesOccurence = new Dictionary<string, SortedDictionary<double, int>>();
            lotCyclesOccurence.Add("1", new SortedDictionary<double, int>());

            List<double> totalLotDuration = new List<double>();
            List<int> totalTestedQuantity = new List<int>();


            foreach (var lotEntry in model.Lots)
            {

                if (lotEntry.LedsInLot.Count == 0)
                    continue;

                var lotId = lotEntry.LotId;
                var ledsInLot = lotEntry.LedsInLot;

                var inspTimeList = ledsInLot.SelectMany(l => l.TesterData).Select(ll => ll.TimeOfTest).OrderBy(o => o).ToList();
                var begin = inspTimeList.Min(d => d);
                var end = inspTimeList.Max(d => d);
                var ledsTestedInLot = lotEntry.TestedQuantity;
                var lotDuration = (end - begin).TotalSeconds;
                var avgCycleTime = Math.Ceiling((lotDuration / ledsTestedInLot) * 2) / 2;
                if (avgCycleTime > 30)
                {
                    //continue;
                    int previousCut = 0;
                    List<List<DateTime>> splittedLotList = new List<List<DateTime>>();
                    for (int i = 1; i < inspTimeList.Count; i++) 
                    {
                        if ((inspTimeList[i]-inspTimeList[i-1]).TotalSeconds>60)
                        {
                            if (i - previousCut < 5) continue;
                            {
                                splittedLotList.Add(new List<DateTime>());
                                for (int j = previousCut; j < i; j++)
                                {
                                    splittedLotList[splittedLotList.Count - 1].Add(inspTimeList[j]);
                                }
                                previousCut = i;
                            }

                        }

                    }
                    double sumOfDuration = 0;
                    

                    foreach (var list in splittedLotList)
                    {
                        sumOfDuration += (list.Max() - list.Min()).TotalSeconds;
                        
                        
                    }

                    avgCycleTime = Math.Ceiling((sumOfDuration / ledsTestedInLot) * 2) / 2;
                    lotDuration = sumOfDuration;
                    
                }
                totalLotDuration.Add(lotDuration);
                totalTestedQuantity.Add(ledsTestedInLot);
                
                
                if (!lotCyclesOccurence["1"].ContainsKey(avgCycleTime)) lotCyclesOccurence["1"].Add(avgCycleTime, 0);
                lotCyclesOccurence["1"][avgCycleTime] += 1;

            }
            if (totalLotDuration.Count > 0)
            {
                var avgDurationTime = Math.Round(totalLotDuration.Average(), 0);
                var avgCT = Math.Round(totalLotDuration.Sum() / totalTestedQuantity.Sum(), 1);
                dataGridViewModelInfo.Rows.Add("LOT avg. duration", avgDurationTime + " sec");
                dataGridViewModelInfo.Rows.Add("Cycle time avg.", avgCT + " sec");
                dataGridViewModelInfo.Rows.Add("Tested modules", totalTestedQuantity.Sum());
                //dataGridViewModelInfo.Rows.Add("data", totalLotDuration.Count);
            }

            foreach (DataGridViewColumn col in dataGridViewModelInfo.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            Charting.CycleTimeHistogram(chartModel, lotCyclesOccurence);
        }

        private Dictionary<String, List<DateTime>> PrepareTesterData(Model model)
        {
            IEnumerable<TesterData> tds = ledStorage.Lots.Values
                 .Where(l => l.Model == model)
                 .SelectMany(l => l.LedsInLot)
                 .SelectMany(led => led.TesterData).Where(td => form.DateFilter(td.FixedDateTime));

            Dictionary<String, List<DateTime>> result = new Dictionary<string, List<DateTime>>();

            foreach (var td in tds)
            {
                if (td.TesterId == "0") continue;
                List<DateTime> tdsForThisTester;
                if (!result.TryGetValue(td.TesterId, out tdsForThisTester))
                {
                    tdsForThisTester = new List<DateTime>();
                    result.Add(td.TesterId, tdsForThisTester);
                }
                tdsForThisTester.Add(td.TimeOfTest);
            }
            foreach (var entry in result)
            {
                entry.Value.Sort();
            }

            return result;
        }
    }
}
