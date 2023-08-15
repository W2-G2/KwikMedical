using KwikMedical.Data;
using KwikMedical.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace KwikMedical.Controllers
{
    public class HospitalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HospitalController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult UpdatePatientRecord(int emergencyCallId, string actionTaken, string timeSpent)
        {
            var emergencyCall = _context.EmergencyCalls.FirstOrDefault(e => e.Id == emergencyCallId);
            if (emergencyCall != null)
            {
                // Logic to update the patient's record with the call-out details
                // This can include details like who, what, when, where, any action taken, and length of time spent on the call

                emergencyCall.EmergencyStatus = "Completed";
                _context.EmergencyCalls.Update(emergencyCall);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
