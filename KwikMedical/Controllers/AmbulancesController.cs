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

        public IActionResult Create()
        {
            ViewData["HospitalId"] = new SelectList(_context.Hospitals, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HospitalId")] Ambulance ambulance)
        {
            var associatedHospital = _context.Hospitals.Find(ambulance.HospitalId);
            if (associatedHospital != null)
            {
                ambulance.Location = associatedHospital.City;
                ambulance.CurrentCity = associatedHospital.City;
            }

            ambulance.Code = "Ambulance-" + DateTime.Now.Ticks; // Generating a unique code based on the current timestamp
            ambulance.IsAvailable = true; // Setting the ambulance as available by default
            ambulance.Latitude = 0.0; // Default value
            ambulance.Longitude = 0.0; // Default value

            _context.Add(ambulance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(int id, Ambulance ambulance)
        {
            if (id != ambulance.Id)
            {
                return NotFound();
            }

            var associatedHospital = _context.Hospitals.Find(ambulance.HospitalId);
            if (associatedHospital != null && !string.IsNullOrEmpty(associatedHospital.City))
            {
                ambulance.Location = associatedHospital.City;
                ambulance.CurrentCity = associatedHospital.City;
            }
            else
            {
                ambulance.Location = "Unknown"; // Default value if the city is not found
            }

            _context.Update(ambulance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var ambulance = await _context.Ambulances.FindAsync(id);
            if (ambulance == null)
            {
                return NotFound();
            }
            return View(ambulance);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ambulance = await _context.Ambulances.FindAsync(id);
            _context.Ambulances.Remove(ambulance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetEmergencyDetails(int ambulanceId)
        {
            var ambulance = _context.Ambulances.Include(a => a.CurrentEmergencyCall).ThenInclude(ec => ec.Patient).FirstOrDefault(a => a.Id == ambulanceId);
            if (ambulance?.CurrentEmergencyCall == null)
            {
                return NotFound();
            }

            var patient = ambulance.CurrentEmergencyCall.Patient;
            var medicalRecord = _context.MedicalRecords.FirstOrDefault(mr => mr.PatientId == patient.Id);

            return Json(new
            {
                patientDetails = new
                {
                    patient.FirstName,
                    patient.LastName,
                    patient.Address,
                    patient.City,
                    patient.Postcode
                },
                medicalRecord
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMedicalRecord([FromBody] MedicalRecord updatedRecord)
        {
            if (updatedRecord == null || updatedRecord.Id == 0)
            {
                return BadRequest("Invalid request payload.");
            }

            var existingRecord = await _context.MedicalRecords.FindAsync(updatedRecord.Id);
            if (existingRecord == null)
            {
                return NotFound($"Medical record with ID {updatedRecord.Id} not found.");
            }

            existingRecord.ClinicalNotes = updatedRecord.ClinicalNotes;
            existingRecord.LaboratoryReports = updatedRecord.LaboratoryReports;
            existingRecord.Letters = updatedRecord.Letters;
            existingRecord.PrescriptionCharts = updatedRecord.PrescriptionCharts;
            existingRecord.TelephoneCalls = updatedRecord.TelephoneCalls;
            existingRecord.Xrays = updatedRecord.Xrays;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ConcludeEmergencyCall(int emergencyCallId)
        {
            try
            {
                // Directly create an instance of EmergencyCall with the provided ID
                var emergencyCall = new EmergencyCall { Id = emergencyCallId };

                // Attach the instance to the DbContext
                _context.EmergencyCalls.Attach(emergencyCall);

                // Mark it for deletion
                _context.EmergencyCalls.Remove(emergencyCall);

                // Find the associated ambulance
                var ambulance = await _context.Ambulances
                    .Where(a => a.CurrentEmergencyCallId == emergencyCallId)
                    .FirstOrDefaultAsync();

                if (ambulance != null)
                {
                    ambulance.IsAvailable = true;
                    ambulance.CurrentEmergencyCallId = null;
                }

                await _context.SaveChangesAsync();

                return Ok(new { status = "success", message = "Emergency call concluded successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error concluding emergency call: {ex.Message}");
                return BadRequest(new { status = "error", message = ex.Message });
            }
        }
    }
}
