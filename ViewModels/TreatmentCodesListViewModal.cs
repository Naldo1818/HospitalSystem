using DEMO.Models;

namespace DEMO.ViewModels
{
    public class TreatmentCodesListViewModal
    {//TreatmentCodes
     
        public string TreatmentName { get; set; }
       
        public string TreatmentCode { get; set; }



        //List
        public TreatmentCodes treatmentCodes { get; set; }
        public List<TreatmentCodes> AllTreatmentCodes { get; set; }
    }
}
