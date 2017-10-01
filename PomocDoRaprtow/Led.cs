namespace PomocDoRaprtow
{
    public class Led
    {
        public Led(string serialNumber, Lot lot, TesterData testerData)
        {
            SerialNumber = serialNumber;
            Lot = lot;
            TesterData = testerData;
        }

        public string SerialNumber { get; }
        public Lot Lot { get; }
        public TesterData TesterData { get; }
    }
}