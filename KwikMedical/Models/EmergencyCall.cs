using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KwikMedical.Models
{
    public class EmergencyCall
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string EmergencyDetails { get; set; }
        public string EmergencyCity { get; set; }
        public string EmergencyStatus { get; set; }
        public string NearestHospital { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        public bool IsAccepted { get; set; } = false;

        [ForeignKey("Ambulance")]
        public int? AmbulanceId { get; set; }
        public virtual Ambulance Ambulance { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string OperatorName { get; set; }  // Name of the operator who received the call
        public string MedicalCondition { get; set; }  // Medical condition described during the call
    }
}
