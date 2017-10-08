using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomocDoRaprtow
{
    public class BoxingUtilities
    {
        
        public static String PercentPalletised(Lot lot)
        {
            return MathUtilities.CalculatePercentage(lot.LedsInLot.Count, lot.LedsInLot.Count(l => l.Boxing.PalletisingDate.HasValue));
        }

        public static bool IsFullyPalletised(Lot lot)
        {
            return lot.LedsInLot.All(l => l.Boxing.PalletisingDate.HasValue);
        }

        public static String PercentBoxed(Lot lot)
        {
            return MathUtilities.CalculatePercentage(lot.LedsInLot.Count, lot.LedsInLot.Count(l => l.Boxing.BoxingDate.HasValue));
        }

        public static bool IsSplit(Lot lot)
        {
            return lot.WasteInfo != null;
        }

        public static List<string> LotToBoxesId (Lot lot)
        {
            List<string> result = new List<string>();
            result.AddRange( lot.LedsInLot.Select(l =>  l.Boxing.BoxId).Distinct().ToArray() );
            return result;
        }

        public static List<string> LotToBoxesDate(Lot lot)
        {
            List<string> result = new List<string>();
            result.AddRange(lot.LedsInLot.Select(l => l.Boxing.BoxingDate.ToString()).Distinct().ToArray());
            return result;
        }

        public static List<string> LotToPalletId(Lot lot)
        {
            List<string> result = new List<string>();
            result.AddRange(lot.LedsInLot.Select(l => l.Boxing.PalletId).Distinct().ToArray());
            return result;
        }

        public static List<string> LotToPalletDate(Lot lot)
        {
            List<string> result = new List<string>();
            result.AddRange(lot.LedsInLot.Select(l => l.Boxing.PalletisingDate.ToString()).Distinct().ToArray());
            return result;
        }
    }
}
