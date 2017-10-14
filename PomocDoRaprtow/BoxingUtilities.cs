using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomocDoRaprtow
{
    public class BoxingUtilities
    {
        public static String PalletizingProgress(Lot lot)
        {
            return lot.LedsInLot.Count(l => l.Boxing.PalletisingDate.HasValue) +@"/"+lot.ManufacturedGoodQuantity;
        }

        public static int PalletisedQuantity(Lot lot)
        {
            return lot.LedsInLot.Count(l => l.Boxing.PalletisingDate.HasValue);
        }

        public static int BoxedQuantity(Lot lot)
        {
            return lot.LedsInLot.Count(l => l.Boxing.BoxingDate.HasValue);
        }

        public static bool IsFullyPalletised(Lot lot)
        {
            return (lot.LedsInLot.Count(l => l.Boxing.PalletisingDate.HasValue) >= lot.ManufacturedGoodQuantity);
        }

        public static bool IsFullyBoxed(Lot lot)
        {
            return (lot.LedsInLot.Count(l => l.Boxing.BoxingDate.HasValue) >= lot.ManufacturedGoodQuantity);
        }

        public static String BoxingProgress(Lot lot)
        {
            return lot.LedsInLot.Count(l => l.Boxing.BoxingDate.HasValue) + @"/" + lot.ManufacturedGoodQuantity;
        }

        public static bool IsSplit(Lot lot)
        {
            return lot.WasteInfo != null;
        }

        public static List<string> LotToBoxesId (Lot lot)
        {
            return lot.LedsInLot.Select(l => l.Boxing.BoxId).Distinct().ToList();
        }

        public static List<string> LotToBoxesDate(Lot lot)
        {
            return lot.LedsInLot.Select(l => l.Boxing.BoxingDate.ToString()).Distinct().ToList();
        }

        public static List<string> LotToPalletId(Lot lot)
        {
            return lot.LedsInLot.Select(l => l.Boxing.PalletId).Distinct().ToList();
        }

        public static List<string> LotToPalletDate(Lot lot)
        {
            return lot.LedsInLot.Select(l => l.Boxing.PalletisingDate.ToString()).Distinct().ToList();
        }
    }
}
