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

        public DbSet<AddPharmacyMedicationModel> PharmacyMedication { get; set; }
        
        public DbSet<AdmittedPatientsModel> AdmittedPatientsModel { get; set; }
        public DbSet<PatientVitals> PatientVitals { get; set; }

    }
}
