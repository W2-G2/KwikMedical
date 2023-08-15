using KwikMedical.Data;
using KwikMedical.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace KwikMedical.Controllers
{
    public class AmbulanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AmbulanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult AmbulanceStatus(int id)
        {
            var ambulance = _context.Ambulances.FirstOrDefault(a => a.Id == id);
            return View(ambulance);
        }

        [HttpPost]
        public IActionResult UpdateStatus(Ambulance ambulance)
        {
            var existingAmbulance = _context.Ambulances.FirstOrDefault(a => a.Id == ambulance.Id);
            if (existingAmbulance != null)
            {
                existingAmbulance.IsAvailable = ambulance.IsAvailable;
                existingAmbulance.CurrentCity = ambulance.CurrentCity;
                _context.Ambulances.Update(existingAmbulance);
                _context.SaveChanges();
            }
            return RedirectToAction("AmbulanceStatus", new { id = ambulance.Id });
        }

        public IActionResult SendMedicalRecords(int ambulanceId)
        {
            var ambulance = _context.Ambulances.FirstOrDefault(a => a.Id == ambulanceId);
            if (ambulance != null && ambulance.CurrentEmergencyCallId.HasValue)
            {
                var emergencyCall = _context.EmergencyCalls.FirstOrDefault(e => e.Id == ambulance.CurrentEmergencyCallId.Value);
                if (emergencyCall != null)
                {
                    var patientMedicalRecord = _context.MedicalRecords.FirstOrDefault(m => m.PatientId == emergencyCall.PatientId);
                    if (patientMedicalRecord != null)
                    {
                        // Logic to send the medical records to the ambulance's smartphone
                        // This can be simulated for now
                    }
                }
            }

            return RedirectToAction("Index");
        }
    }
}
