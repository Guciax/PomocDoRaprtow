using PomocDoRaprtow.DataModels;
using System;
using System.Collections.Generic;

namespace PomocDoRaprtow
{
    public class LedStorage
    {
        public LedStorage(Dictionary<string, Lot> lots, Dictionary<string, WasteInfo> lotIdToWasteInfo, Dictionary<string, Led> serialNumbersToLed, Dictionary<string, Model> models, Dictionary<string, Boxing> serialInBox)
        {
            Lots = lots;
            LotIdToWasteInfo = lotIdToWasteInfo;
            SerialNumbersToLed = serialNumbersToLed;
            Models = models;
            SerialInBox = serialInBox;
        }

        public Dictionary<String, Lot> Lots { get; }
        public Dictionary<string, WasteInfo> LotIdToWasteInfo { get; }
        public Dictionary<string, Led> SerialNumbersToLed { get; }
        public Dictionary<string, Model> Models { get; }
        public Dictionary<string, Boxing> SerialInBox { get; }

        public IEnumerable<Led> Leds => SerialNumbersToLed.Values;
    }
}