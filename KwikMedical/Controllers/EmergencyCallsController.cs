using KwikMedical.Data;
using KwikMedical.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace KwikMedical.Controllers
{
    public class EmergencyCallsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmergencyCallsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult CreateEmergencyCall(EmergencyCall emergencyCall)
        {
            _context.EmergencyCalls.Add(emergencyCall);
            _context.SaveChanges();

            var medicalRecords = _context.MedicalRecords.FirstOrDefault(m => m.PatientId == emergencyCall.PatientId);
            if (medicalRecords != null)
            {
                // Simulate sending medical records to the ambulance
                SendMedicalRecordsToAmbulance(medicalRecords);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        private void SendMedicalRecordsToAmbulance(MedicalRecord medicalRecords)
        {
            // Assuming the ambulance has an associated user or device ID
            var ambulance = _context.Ambulances.FirstOrDefault(a => a.CurrentEmergencyCallId == medicalRecords.PatientId);
            if (ambulance != null)
            {
                // Here, you'd typically send the medical records to the ambulance's associated device.
                // For this prototype, we'll store the medical record ID in the ambulance's record as a simulation.
                ambulance.MedicalRecordId = medicalRecords.Id;
                _context.SaveChanges();
            }
        }
    }
}
