using System;

namespace PomocDoRaprtow
{
    public class TesterData
    {
        public TesterData(string testerId, DateTime timeOfTest, DateTime fixedDateTime, int shiftNo, bool testResult, string failureReason)
        {
            TesterId = testerId;
            TimeOfTest = timeOfTest;
            FixedDateTime = fixedDateTime;
            ShiftNo = shiftNo;
            TestResult = testResult;
            FailureReason = failureReason;
        }

        public String TesterId { get; }
        public DateTime TimeOfTest { get;  }
        public DateTime FixedDateTime { get; }
        public int ShiftNo { get; }
        public bool TestResult { get;  }
        public String FailureReason { get; }
    }
}