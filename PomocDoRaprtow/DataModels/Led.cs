using System.Collections.Generic;

namespace PomocDoRaprtow
{
    public class Led
    {
        public Led(string serialNumber, Lot lot, TesterData testerData)
        {
            SerialNumber = serialNumber;
            Lot = lot;
            TesterData = new List<TesterData>();
            TesterData.Add(testerData);
        }

        public string SerialNumber { get; }
        public Lot Lot { get; }
        public List<TesterData> TesterData { get; }
        
        public void AddTesterData(TesterData testerData)
        {
            TesterData.Add(testerData);
        }
    }
}