using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KwikMedical.Data;
using KwikMedical.Models;

namespace KwikMedical.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Patients
        public async Task<IActionResult> Index()
        {
            return View(await _context.Patients.ToListAsync());
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> FinalizePatient(int ambulanceId, string firstName, string lastName, string nhsNumber, string address, string city, string postcode)
        {
            var ambulance = await _context.Ambulances.FindAsync(ambulanceId);
            if (ambulance == null)
            {
                return NotFound();
            }

            var patient = new Patient
            {
                FirstName = firstName,
                LastName = lastName,
                NHSNumber = nhsNumber,
                Address = address,
                City = city,
                Postcode = postcode
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            var emergencyCall = await _context.EmergencyCalls.FindAsync(ambulance.CurrentEmergencyCallId);
            if (emergencyCall != null)
            {
                _context.EmergencyCalls.Remove(emergencyCall);
            }

            ambulance.CurrentEmergencyCallId = null;
            ambulance.IsAvailable = true;

            _context.Update(ambulance);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Ambulances");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Patient patient)
        {
            _context.Patients.Add(patient);
            _context.SaveChanges();

            // Create an empty medical record for the new patient
            var medicalRecord = new MedicalRecord
            {
                PatientId = patient.Id,
                LaboratoryReports = "N/A",
                TelephoneCalls = "N/A",
                Xrays = "N/A",
                Letters = "N/A",
                PrescriptionCharts = "N/A",
                ClinicalNotes = "N/A"
            };

            _context.MedicalRecords.Add(medicalRecord);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var patient = _context.Patients.Find(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        [HttpPost]
        public IActionResult Edit(int id, Patient patient)
        {
            if (id != patient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(patient);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        public IActionResult Delete(int id)
        {
            var patient = _context.Patients.Find(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var patient = _context.Patients.Find(id);
            _context.Patients.Remove(patient);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}