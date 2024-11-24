using DEMO.Models;
using DEMO.Models.NurseModels;
using DEMO.Models.PharmacistModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DEMO.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Medication> Medication { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Suburb> Suburb { get; set; }
        public DbSet<Ward> Ward { get; set; }
        public DbSet<Bed> Bed { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Activeingredient> Activeingredient { get; set; }
        public DbSet<PatientInfo> PatientInfo { get; set; }
        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<BookSurgery> BookSurgery { get; set; }
        public DbSet<TreatmentCodes> TreatmentCodes { get; set; }
        public DbSet<Prescription> Prescription { get; set; }
        public DbSet<MedicationInstructions> MedicationInstructions { get; set; }
        public DbSet<SurgeryTreatmentCode> SurgeryTreatmentCode { get; set; }
        public DbSet<MedicationActiveIngredient> MedicationActiveIngredient { get; set; }
        public DbSet<PatientAllergy> PatientAllergy { get; set; }
        public DbSet<RejectedScriptsModel> RejectScriptModel { get; set; }
        public DbSet<DispensedScriptsModel> DispensedScriptsModel { get; set; }

        
        
        public DbSet<AdmittedPatientsModel> AdmittedPatients { get; set; }
        
        public DbSet<Condition> Condition { get; set; }
        public DbSet<PatientConditions> PatientConditions { get; set; }
        public DbSet<PharmMedModel> PharmacyMedication { get; set; }

        public DbSet<PatientMedication> patientMedication{ get; set; }
        public DbSet<CurrentMedication> CurrentMedication { get; set; }

        public DbSet<AdmissionStatus> AdmissionStatus { get; set; }
        
        public DbSet<PatientVitals> PatientVitals { get; set; }
        public DbSet<ConditionActiveIngredient> ConditionActiveIngredient { get; set; }


        public DbSet<PharmacyMedicationModel> PharmacyMedicationModel { get; set; }

        public DbSet<OrderStockModel> StockOrderedTable { get; set; }
        public DbSet<AdministerMedication> AdministerMedication { get; set; }


        public DbSet<PharmMedicationStockOrder> PharmacyMedicationStockOrderTable { get;set; }
        
    }
}
