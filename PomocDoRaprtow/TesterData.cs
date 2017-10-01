using System;

namespace PomocDoRaprtow
{
    public class TesterData
    {
        public TesterData(string testerId, DateTime timeOfTest, bool testResult, string failureReason)
        {
            TesterId = testerId;
            TimeOfTest = timeOfTest;
            TestResult = testResult;
            FailureReason = failureReason;
        }

        public String TesterId { get; }
        public DateTime TimeOfTest { get;  }
        public bool TestResult { get;  }
        public String FailureReason { get; }
    }
}