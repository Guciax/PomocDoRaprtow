using System;
using System.Collections.Generic;

namespace PomocDoRaprtow
{
    public class LedStorage
    {
        public LedStorage(Dictionary<string, Lot> lots, Dictionary<string, WasteInfo> lotIdToWasteInfo, Dictionary<string, Led> serialNumbersToLed, HashSet<string> models)
        {
            Lots = lots;
            LotIdToWasteInfo = lotIdToWasteInfo;
            SerialNumbersToLed = serialNumbersToLed;
            Models = models;
        }

        public Dictionary<String, Lot> Lots { get; }
        public Dictionary<string, WasteInfo> LotIdToWasteInfo { get; }
        public Dictionary<string, Led> SerialNumbersToLed { get; }
        public HashSet<string> Models { get; }
    }
}