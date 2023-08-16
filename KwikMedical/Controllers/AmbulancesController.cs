using KwikMedical.Data;
using KwikMedical.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace KwikMedical.Controllers
{
    public class AmbulancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AmbulancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ambulances
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ambulances.Include(a => a.Hospital).ToListAsync());
        }

        // GET: Ambulances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ambulance = await _context.Ambulances
                .Include(a => a.Hospital)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ambulance == null)
            {
                return NotFound();
            }

            return View(ambulance);
        }

        // GET: Ambulances/UpdateStatus/5
        public async Task<IActionResult> UpdateStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ambulance = await _context.Ambulances.FindAsync(id);
            if (ambulance == null)
            {
                return NotFound();
            }

            ambulance.IsAvailable = !ambulance.IsAvailable; // Toggle the status
            _context.Update(ambulance);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Ambulances/SendMedicalRecords/5
        public async Task<IActionResult> SendMedicalRecords(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ambulance = await _context.Ambulances
                .Include(a => a.CurrentEmergencyCall)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (ambulance == null || ambulance.CurrentEmergencyCall == null)
            {
                return NotFound();
            }

            var medicalRecord = await _context.MedicalRecords
                .FirstOrDefaultAsync(m => m.PatientId == ambulance.CurrentEmergencyCall.PatientId);

            if (medicalRecord == null)
            {
                return NotFound();
            }

            // Logic to send medical records to the ambulance
            // This can be simulated for now as sending a notification or message

            return RedirectToAction(nameof(Index));
        }

        // GET: Ambulances/Dashboard/5
        public async Task<IActionResult> Dashboard(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ambulance = await _context.Ambulances
                .Include(a => a.CurrentEmergencyCall)
                .ThenInclude(e => e.Patient)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (ambulance == null || ambulance.CurrentEmergencyCallId == null)
            {
                return NotFound();
            }

            var medicalRecord = await _context.MedicalRecords.FirstOrDefaultAsync(m => m.PatientId == ambulance.CurrentEmergencyCall.PatientId);

            // Pass both the ambulance and medical record to the view
            var viewModel = new AmbulanceDashboardViewModel
            {
                Ambulance = ambulance,
                MedicalRecord = medicalRecord
            };

            return View(viewModel);
        }

        // GET: Ambulances/Create
        public IActionResult Create()
        {
            ViewData["HospitalId"] = new SelectList(_context.Hospitals, "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Ambulance ambulance)
        {
            if (ModelState.IsValid)
            {
                var hospital = _context.Hospitals.Find(ambulance.HospitalId);
                if (hospital == null)
                {
                    ModelState.AddModelError("", "Hospital not found in the database.");
                    ViewBag.Hospitals = new SelectList(_context.Hospitals, "Id", "Name");
                    return View(ambulance);
                }

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
