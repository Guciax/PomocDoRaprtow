using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static PomocDoRaprtow.OccurenceCalculations;

namespace PomocDoRaprtow.Tabs
{
    public class ModelOperations
    {
        public LedStorage ledStorage { get; set; }
        private Form1 form;
        private readonly Chart chartModel;
        private readonly DataGridView dataGridViewModelInfo;
        private readonly TreeView treeViewModelInfo;
        private readonly ComboBox comboBoxModels;
        private readonly DateTimePicker pickerStart;
        private readonly DateTimePicker pickerEnd;

        public ModelOperations(Form1 form, Chart chartModel, DataGridView dataGridViewModelInfo, TreeView treeViewModelInfo, 
            ComboBox comboBoxModels, DateTimePicker pickerStart, DateTimePicker pickerEnd)
        {
            this.form = form;
            this.chartModel = chartModel;
            this.dataGridViewModelInfo = dataGridViewModelInfo;
            this.treeViewModelInfo = treeViewModelInfo;
            this.comboBoxModels = comboBoxModels;
            this.pickerStart = pickerStart;
            this.pickerEnd = pickerEnd;
        }

        public void GenerateCycleTimeChart(Model model)
        {
            var testerIdToTesterDates = PrepareTesterDataPcsPerHour(model);
            Dictionary<String, SortedDictionary<double, int>> cyclesOccurence = new Dictionary<string, SortedDictionary<double, int>>();

            foreach (var entryTesterIdTesterData in testerIdToTesterDates)
            {
                var testerId = entryTesterIdTesterData.Key;
                var testerDatas = entryTesterIdTesterData.Value;

                if (!cyclesOccurence.ContainsKey(testerId))
                    cyclesOccurence.Add(testerId, new SortedDictionary<double, int>());

                for (int i = 1; i < testerDatas.Count; i++)
                {
                    if (!cyclesOccurence[testerId].ContainsKey(testerDatas[i]))
                    {
                        cyclesOccurence[testerId].Add(testerDatas[i], 0);
                    }

                    cyclesOccurence[testerId][testerDatas[i]]++;
                }
            }

            Charting.CycleTimeHistogram(chartModel, cyclesOccurence);
        }

        private Dictionary<int, List<DateTime>> inspectionTimeSplitter(List<DateTime> inputList)
        {
            Dictionary<int, List<DateTime>> result = new Dictionary<int, List<DateTime>>();
            int lastCut = 0;

            for (int i = 1; i < inputList.Count; i++)
            {
                if ((inputList[i] - inputList[i - 1]).TotalMinutes < 15) continue;
                result.Add(lastCut, inputList.GetRange(lastCut, i - lastCut));
                lastCut = i;
            }
            result.Add(lastCut, inputList.GetRange(lastCut, inputList.Count  - lastCut));

            return result;
        }

        public void GenerateLotEfficiencyChart(List<Lot> lotList)
        {
            dataGridViewModelInfo.Rows.Clear();
            Dictionary<String, SortedDictionary<double, int>> lotCyclesOccurence = new Dictionary<string, SortedDictionary<double, int>>();


            SortedDictionary<string, List<double>> outputPerHourList = new SortedDictionary<string, List<double>>();
            double numberOfTestCycles = 0;
            double numberOfTestedLeds = 0;
            double testsTotalDurationHours = 0;
            HashSet<string> uniqueModels = new HashSet<string>();

            foreach (var lot in lotList)
            {

                if (lot.LedsInLot.Count == 0)
                    continue;
                if (lot.LedTest.TestEnd < pickerStart.Value || lot.LedTest.TestEnd > pickerEnd.Value) continue;

                uniqueModels.Add(lot.Model.ModelName);
                var lotId = lot.LotId;
                var ledsInLot = lot.LedsInLot;
                var testerID = lot.LedTest.TesterId;

                var inspTimeList = ledsInLot.SelectMany(l => l.TesterData).Select(td => td.TimeOfTest).OrderBy(o => o).ToList();
                Dictionary<int, List<DateTime>> splittedInspTimes = inspectionTimeSplitter(inspTimeList);

                numberOfTestCycles += inspTimeList.Count;
                numberOfTestedLeds += lot.LedTest.TestedUniqueQuantity;

                var begin = inspTimeList.Min();
                var end = inspTimeList.Max();
                testsTotalDurationHours += (end - begin).TotalHours;

                foreach (var timeList in splittedInspTimes)
                {
                    if (timeList.Value.Count < 20) continue;
                    testsTotalDurationHours += (timeList.Value.Max() - timeList.Value.Min()).TotalHours;
                    double outputPerHourUp50 = Math.Round((timeList.Value.Count / (timeList.Value.Max() - timeList.Value.Min()).TotalHours) / 50, 0) * 50;
                    if (!outputPerHourList.ContainsKey(testerID))
                    {
                        outputPerHourList.Add(testerID, new List<double>());
                    }
                    outputPerHourList[testerID].Add(outputPerHourUp50);

                    if (!lotCyclesOccurence.ContainsKey(lot.LedTest.TesterId))
                    {
                        lotCyclesOccurence.Add(lot.LedTest.TesterId, new SortedDictionary<double, int>());
                    }
                    if (!lotCyclesOccurence[lot.LedTest.TesterId].ContainsKey(outputPerHourUp50) )
                    {
                        lotCyclesOccurence[lot.LedTest.TesterId].Add(outputPerHourUp50, 0);
                    }
                    lotCyclesOccurence[lot.LedTest.TesterId][outputPerHourUp50]++;


                }

            }


            if (outputPerHourList.Count > 0)
            {
                List<string> allMin = new List<string>();
                List<string> allMax = new List<string>();
                List<string> allAvg = new List<string>();

                allMin.Add("Output/h min");
                allMax.Add("Output/h max");
                allAvg.Add("Output/h avg");

                foreach (var testerID in outputPerHourList)
                {
                    if (testerID.Value.Count > 0) 
                    {
                        allMin.Add(Math.Round(testerID.Value.Min(),0).ToString());
                        allMax.Add(Math.Round(testerID.Value.Max(),0).ToString());
                        allAvg.Add(Math.Round(testerID.Value.Average(),0).ToString());
                    }
                }
                dataGridViewModelInfo.Rows.Add(allMin.ToArray());
                dataGridViewModelInfo.Rows.Add(allMax.ToArray());
                dataGridViewModelInfo.Rows.Add(allAvg.ToArray());
                dataGridViewModelInfo.Rows.Add("Tested modules", numberOfTestedLeds);
                dataGridViewModelInfo.Rows.Add("Number of tests", numberOfTestCycles);
                foreach (var item in uniqueModels)
                {
                    dataGridViewModelInfo.Rows.Add("Model", item);
                }
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

        private Dictionary<String, List<double>> PrepareTesterDataPcsPerHour(Model model)
        {
            IEnumerable<Lot> modelLot = ledStorage.Lots.Values
                 .Where(l => l.Model == model);

            Dictionary<String, List<double>> result = new Dictionary<string, List<double>>();

            foreach (var mdl in modelLot)
            {
                if (!mdl.LotStatus.TestDone || mdl.LedsInLot.Count==0) continue;
                List<double> tdsForThisTester;
                if (!result.TryGetValue(mdl.LedsInLot[0].TesterData[0].TesterId, out tdsForThisTester))
                {
                    tdsForThisTester = new List<double>();
                    result.Add(mdl.LedsInLot[0].TesterData[0].TesterId, tdsForThisTester);
                }
                tdsForThisTester.Add(Math.Floor(mdl.LedTest.TestedUniqueQuantity / (mdl.LedTest.TestEnd - mdl.LedTest.TestStart).TotalHours / 50) * 50);
            }
            foreach (var entry in result)
            {
                entry.Value.Sort();
            }

            return result;
        }

        public void DisplayTesterDataOccurences(IEnumerable<Led> leds, List<Lot> lots)
        {
            var occurencesCalculations = new OccurenceCalculations(lots, form.LotToTesterProductionDetails);
            RebuildOccurenceTreeView(treeViewModelInfo, occurencesCalculations);
        }

        private void RebuildOccurenceTreeView(TreeView targetTreeView, OccurenceCalculations occurenceCalculations)
        {
            targetTreeView.BeginUpdate();
            targetTreeView.Nodes.Clear();
            foreach (var weekTree in occurenceCalculations.Tree.WeekNoToTree.Values)
            {
                var weekTreeViewNode =
                    new TreeNode(FormatTreeViewNodeName(weekTree.Week.ToString(), weekTree.Occurences));
                targetTreeView.Nodes.Add(weekTreeViewNode);
                foreach (var dayTree in weekTree.DayToTree.Values)
                {
                    string dayMonth = dayTree.DateTime.Day.ToString("d2") + "-" + dayTree.DateTime.Month.ToString("d2");
                    var dayTreeViewNode = weekTreeViewNode.Nodes.Add(FormatTreeViewNodeName(dayMonth, dayTree.Occurences));
                    foreach (var shiftTree in dayTree.ShiftToTree)
                    {

                        if (shiftTree.Occurences == 0) continue;
                        var shiftTreeViewNode = dayTreeViewNode.Nodes.Add(FormatTreeViewNodeName(shiftTree.ShiftNo.ToString(), shiftTree.Occurences));
                        foreach (var modelTree in shiftTree.ModelToTree.Values)
                        {

                            shiftTreeViewNode.Nodes.Add(FormatTreeViewNodeName(modelTree.Model, modelTree.Occurences));
                        }
                    }
                }
            }
            targetTreeView.EndUpdate();
        }
        private string FormatTreeViewNodeName(String mainName, int occurences)
        {
            return mainName + " " + occurences;
        }
    }
}
