using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using PomocDoRaprtow;

namespace PomocDoRaportowTest
{
    [TestFixture]
    public class LedStorageLoaderTest
    {
        static LedStorage loader;

        public static String GetPathFor(String file)
        {
            var testPath = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.CurrentContext.TestDirectory));
            return Path.Combine(testPath, "..", file);
        }

        [OneTimeSetUp]
        public static void SetUp()
        {
            loader = new LedStorageLoader().BuildStorage(GetPathFor(LedStorageLoader.LotPath),
                GetPathFor(LedStorageLoader.WastePath), GetPathFor(LedStorageLoader.TesterPath));
        }

        [Test]
        public void LotLoadingTest()
        {
            Assert.AreEqual(1526, loader.Lots.Count);

            Assert.True(loader.Lots.ContainsKey("1210991"));

            var someLot = loader.Lots["1210991"];
            Assert.AreEqual("LLFMLK2-11L403A", someLot.Nc12);
            Assert.AreEqual("U2-J5-2", someLot.RankA);
            Assert.AreEqual("U1-J5-2", someLot.RankB);
            Assert.AreEqual("JEEU2U1A22", someLot.Mrm);
            Assert.Null(someLot.WasteInfo);

            Assert.NotNull(loader.Lots["1206838"].WasteInfo);
        }

        [Test]
        public void WasteInfoLoadingTest()
        {
            Assert.AreEqual(849, loader.LotIdToWasteInfo.Count);
            Assert.True(loader.LotIdToWasteInfo.ContainsKey("1206759"));
            var someWasteInfo = loader.LotIdToWasteInfo["1206759"];

            Assert.AreEqual(new[] {0, 0, 0, 0, 0, 1, 1, 1}.ToList(), someWasteInfo.WasteCounts);
        }
        
        [Test]
        public void LedLoadingTest()
        {
            Assert.True(loader.SerialNumbersToLed.ContainsKey("M7907J-D-GR5R518B8B800002"));
            Assert.AreEqual("7094754", loader.SerialNumbersToLed["M7907J-H-IR5R529A9A800001"].Lot.LotId);

            var someTesterData = loader.SerialNumbersToLed["M7907J-D-GR5R518B8B800001"].TesterData;
            Assert.AreEqual("1", someTesterData.TesterId);
            Assert.AreEqual(true, someTesterData.TestResult);
            Assert.AreEqual("", someTesterData.FailureReason);
        }
    }
}