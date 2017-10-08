using PomocDoRaprtow.DataModels;
using System.Collections.Generic;

namespace PomocDoRaprtow
{
    public class Led
    {
        public Led(string serialNumber, Lot lot, TesterData testerData, Boxing boxing)
        {
            SerialNumber = serialNumber;
            Lot = lot;
            Boxing = boxing;
            TesterData = new List<TesterData>();
            TesterData.Add(testerData);
        }

        public string SerialNumber { get; }
        public Lot Lot { get; }
        public Boxing Boxing { get; } //can be null.
        public List<TesterData> TesterData { get; }
        public bool TestOk { get; set; }
        
        public void AddTesterData(TesterData testerData)
        {
            TesterData.Add(testerData);
        }
    }
}