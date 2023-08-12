using System.ComponentModel.DataAnnotations.Schema;

namespace KwikMedical.Models;

public class MedicalRecord
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // This attribute specifies that the Id field is auto-generated.
    public int Id { get; set; }
    public string LaboratoryReports { get; set; }
    public string TelephoneCalls { get; set; }
    public string Xrays { get; set; }
    public string Letters { get; set; }
    public string PrescriptionCharts { get; set; }
    public string ClinicalNotes { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; }  // Navigation property
}
