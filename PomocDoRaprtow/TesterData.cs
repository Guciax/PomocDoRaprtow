using System;

namespace PomocDoRaprtow
{
    public class TesterData
    {
        public String TesterId { get; set; }
        public DateTime TimeOfTest { get; set; }
        public bool TestResult { get; set; }
        public String FailureReason { get; set; }
    }
}