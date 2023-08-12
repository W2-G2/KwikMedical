using System.ComponentModel.DataAnnotations.Schema;

namespace KwikMedical.Models;

public class EmergencyCall
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string EmergencyDetails { get; set; }
    public string EmergencyCity { get; set; }
    public string EmergencyStatus { get; set; }
    public string NearestHospital { get; set; }
    public int PatientId { get; set; }
    public bool IsAccepted { get; set; }
    public int? AmbulanceId { get; set; }
    [ForeignKey("PatientId")]
    public Patient Patient { get; set; }
    [ForeignKey("AmbulanceId")]
    [InverseProperty("CurrentEmergencyCall")]
    public virtual Ambulance Ambulance { get; set; }
}

