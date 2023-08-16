using Microsoft.AspNetCore.Mvc;

namespace KwikMedical.Models
{
    public class HospitalDashboardViewModel
    {
        public Hospital Hospital { get; set; }
        public List<EmergencyCall> Emergencies { get; set; }
        public List<Ambulance> AvailableAmbulances { get; set; }
        public List<EmergencyCall> NewEmergencies { get; set; }
    }
}
