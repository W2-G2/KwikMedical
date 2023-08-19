namespace KwikMedical.Models
{
    public class UpdatedMedicalRecordModel
    {
        public string ClinicalNotes { get; set; }
        public string LaboratoryReports { get; set; }
        public string Letters { get; set; }
        public string PrescriptionCharts { get; set; }
        public string TelephoneCalls { get; set; }
        public string Xrays { get; set; }
        public int EmergencyCallId { get; set; }
    }
}
