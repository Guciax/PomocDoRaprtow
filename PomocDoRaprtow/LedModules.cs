using System;

namespace PomocDoRaprtow
{
    public class LedModules
    {
        public string SerialNumber { get; set; }        //x tester.csv serial_no [0] 
        public string ProductionOrderId{ get; set; }    //x tester.csv wip_entity_name [4]
        public string ModelName { get; set; }           //zlecenia_produkcyjne [3]

        public string KittingDateTime { get; set; }     //x zlecenia_produkcyjne DataCzasWydruku [15]
        public int KittingOrderQuantity { get; set; }   //zlecenia_produkcyjne Ilosc_wyrobu_zlecona [4]
        public string KittingLineNumber { get; set; }   //zlecenia_produkcyjne LiniaProdukcyjna [27]
        
        public string SmtDateTimeStart { get; set; }         //???

        public string   TesterId { get; set; }          //tester.csv tester_id [2]
        public DateTime TesterTimeOfTest { get; set; }  //tester.csv inspection_time [1]
        public bool     TestResult { get; set; }        //tester.csv result [6]
        public string   TesterFailureReason { get; set; }//tester.csv ng_type [7]

        public string VisInspDateTime { get; set; }        //odpad DataCzas [10]
        public bool   VusInspResult { get; set; }           //???
        public string VisInspNgReason { get; set; }         //???

        public string BoxingDate { get; set; }              //wyrobLG_opakowanie Boxing_Date [4]

        public string PalletisingDate { get; set; }         //wyrobLG_opakowanie Palletising_Date [5]

        public string ProductionStatus { get; set; }    //zlecenia_produkcyjne STATUS [11]
    }
}