using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KwikMedical.Models
{
    public class MedicalRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string LaboratoryReports { get; set; }
        public string TelephoneCalls { get; set; }
        public string Xrays { get; set; }
        public string Letters { get; set; }
        public string PrescriptionCharts { get; set; }
        public string ClinicalNotes { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public int? AmbulanceId { get; set; }
        public Ambulance Ambulance { get; set; }  // Add this line
    }
}
