using KwikMedical.Data;
using KwikMedical.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IActionResult Create()
        {
            ViewBag.Hospitals = new SelectList(_context.Hospitals, "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Ambulance ambulance)
        {
            if (ModelState.IsValid)
            {
                _context.Ambulances.Add(ambulance);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Hospitals = new SelectList(_context.Hospitals, "Id", "Name", ambulance.HospitalId);
            return View(ambulance);
        }

        public IActionResult Edit(int id)
        {
            var ambulance = _context.Ambulances.Find(id);
            if (ambulance == null)
            {
                return NotFound();
            }
            ViewBag.Hospitals = new SelectList(_context.Hospitals, "Id", "Name", ambulance.HospitalId);
            return View(ambulance);
        }

        [HttpPost]
        public IActionResult Edit(int id, Ambulance ambulance)
        {
            if (id != ambulance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(ambulance);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Hospitals = new SelectList(_context.Hospitals, "Id", "Name", ambulance.HospitalId);
            return View(ambulance);
        }

        public IActionResult Delete(int id)
        {
            var ambulance = _context.Ambulances.Find(id);
            if (ambulance == null)
            {
                return NotFound();
            }
            return View(ambulance);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var ambulance = _context.Ambulances.Find(id);
            _context.Ambulances.Remove(ambulance);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
