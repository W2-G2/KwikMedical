using KwikMedical.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace KwikMedical.Models
{

    public class Ambulance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Code { get; set; }
        public bool IsAvailable { get; set; }
        public string CurrentCity { get; set; }
        public int? CurrentEmergencyCallId { get; set; }
        public EmergencyCall CurrentEmergencyCall { get; set; }

        // Add this for linking the ambulance to a hospital
        [ForeignKey("HospitalId")]
        public int HospitalId { get; set; }
        public Hospital Hospital { get; set; } // Navigation property

        // Add this for linking the ambulance to a medical record
        public int? MedicalRecordId { get; set; }
        public MedicalRecord MedicalRecord { get; set; } // Navigation property
        public string Location { get; set; }
        public double Latitude { get; set; }  // Simulated GPS Latitude
        public double Longitude { get; set; } // Simulated GPS Longitude
        public int? AssignedAmbulanceId { get; set; }
        public Ambulance AssignedAmbulance { get; set; }
    }
}