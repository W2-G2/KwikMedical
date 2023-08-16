using KwikMedical.Data;
using KwikMedical.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // GET: Hospitals
        public async Task<IActionResult> Index()
        {
            return View(await _context.Hospitals.ToListAsync());
        }

        // GET: Hospitals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hospital = await _context.Hospitals.FirstOrDefaultAsync(h => h.Id == id);
            if (hospital == null)
            {
                return NotFound();
            }

            return View(hospital);
        }

        public async Task<IActionResult> Dashboard(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hospital = await _context.Hospitals.FindAsync(id);
            if (hospital == null)
            {
                return NotFound();
            }

            var ongoingEmergencies = _context.EmergencyCalls
                .Where(e => !e.IsCompleted && e.NearestHospital == hospital.Name)
                .ToList();

            var availableAmbulances = _context.Ambulances
                .Where(a => a.IsAvailable && a.HospitalId == hospital.Id)
                .ToList();

            var newEmergencies = _context.EmergencyCalls
                .Where(e => !e.IsCompleted && e.Timestamp > DateTime.Now.AddHours(-1) && e.NearestHospital == hospital.Name)
                .ToList();

            var viewModel = new HospitalDashboardViewModel
            {
                Hospital = hospital,
                Emergencies = ongoingEmergencies,
                AvailableAmbulances = availableAmbulances,
                NewEmergencies = newEmergencies
            };

            return View(viewModel);
        }
    }
}
