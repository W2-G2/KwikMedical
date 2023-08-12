using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KwikMedical.Data;
using KwikMedical.Models;
using Newtonsoft.Json;

namespace KwikMedical.Controllers
{
    public class EmergencyCallsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmergencyCallsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EmergencyCalls
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.EmergencyCalls.Include(e => e.Patient);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: EmergencyCalls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EmergencyCalls == null)
            {
                return NotFound();
            }

            var emergencyCall = await _context.EmergencyCalls
                .Include(e => e.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emergencyCall == null)
            {
                return NotFound();
            }

            return View(emergencyCall);
        }

        // GET: EmergencyCalls/Create
        public IActionResult Create()
        {
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "NHSNumber");
            return View();
        }

        // POST: EmergencyCalls/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmergencyDetails,EmergencyCity,EmergencyStatus,NearestHospital,PatientId")] EmergencyCall emergencyCall)
        {
            if (ModelState.IsValid)
            {
                // Look for an available ambulance in the same city
                var availableAmbulance = await _context.Ambulances
                    .Where(a => a.CurrentCity == emergencyCall.EmergencyCity && a.IsAvailable == true)
                    .FirstOrDefaultAsync();

                if (availableAmbulance != null)
                {
                    // Assign the available ambulance to the emergency call
                    availableAmbulance.CurrentEmergencyCallId = emergencyCall.Id;
                    availableAmbulance.IsAvailable = false;
                    _context.Update(availableAmbulance);
                }

                _context.Add(emergencyCall);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "NHSNumber", emergencyCall.PatientId);
            return View(emergencyCall);
        }

        // GET: EmergencyCalls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emergencyCall = await _context.EmergencyCalls.FindAsync(id);
            if (emergencyCall == null)
            {
                return NotFound();
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "NHSNumber", emergencyCall.PatientId);
            return View(emergencyCall);
        }

        // POST: EmergencyCalls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmergencyDetails,EmergencyCity,EmergencyStatus,NearestHospital,PatientId")] EmergencyCall emergencyCall)
        {
            if (id != emergencyCall.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emergencyCall);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmergencyCallExists(emergencyCall.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Id", emergencyCall.PatientId);
            return View(emergencyCall);
        }

        // GET: EmergencyCalls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EmergencyCalls == null)
            {
                return NotFound();
            }

            var emergencyCall = await _context.EmergencyCalls
                .Include(e => e.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emergencyCall == null)
            {
                return NotFound();
            }

            return View(emergencyCall);
        }

        // POST: EmergencyCalls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EmergencyCalls == null)
            {
                return Problem("Entity set 'ApplicationDbContext.EmergencyCalls'  is null.");
            }
            var emergencyCall = await _context.EmergencyCalls.FindAsync(id);
            if (emergencyCall != null)
            {
                _context.EmergencyCalls.Remove(emergencyCall);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmergencyCallExists(int id)
        {
          return (_context.EmergencyCalls?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost]
        public JsonResult CheckPatient(string firstName, string lastName, string nhsNumber)
        {
            var patient = _context.Patients
                .Include(p => p.MedicalRecord)
                .FirstOrDefault(p => p.FirstName == firstName && p.LastName == lastName && p.NHSNumber == nhsNumber);

            if (patient == null)
            {
                return Json(null);
            }

            var result = new
            {
                patient.Id,
                patient.FirstName,
                patient.LastName,
                patient.NHSNumber,
                patient.Address,
                patient.City,
                patient.Postcode,
                MedicalRecord = new
                {
                    LaboratoryReports = patient.MedicalRecord?.LaboratoryReports ?? "N/A",
                    TelephoneCalls = patient.MedicalRecord?.TelephoneCalls ?? "N/A",
                    Xrays = patient.MedicalRecord?.Xrays ?? "N/A",
                    Letters = patient.MedicalRecord?.Letters ?? "N/A",
                    PrescriptionCharts = patient.MedicalRecord?.PrescriptionCharts ?? "N/A",
                    ClinicalNotes = patient.MedicalRecord?.ClinicalNotes ?? "N/A",
                }
            };

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmergencyCall([FromBody] EmergencyCall emergencyCall)
        {
            // Check if emergencyCall is null
            if (emergencyCall == null)
            {
                Console.WriteLine("Received null EmergencyCall");
                return Json(new { success = false });
            }

            // Randomly select a nearest hospital based on the EmergencyCity.
            var hospitalsInCity = _context.Hospitals.Where(h => h.City == emergencyCall.EmergencyCity).ToList();
            if (hospitalsInCity.Count == 0)
            {
                Console.WriteLine($"No hospitals found in city: {emergencyCall.EmergencyCity}");
                return Json(new { success = false });
            }

            var random = new Random();
            int index = random.Next(hospitalsInCity.Count);
            emergencyCall.NearestHospital = hospitalsInCity[index].Name;

            // Set the Patient object based on the PatientId.
            var patient = _context.Patients.Find(emergencyCall.PatientId);
            if (patient == null)
            {
                Console.WriteLine($"No patient found with id: {emergencyCall.PatientId}");
                return Json(new { success = false });
            }

            emergencyCall.PatientId = patient.Id; // Assign the patient id to emergencyCall.PatientId

            // Log the EmergencyCall object
            Console.WriteLine($"Received EmergencyCall: {JsonConvert.SerializeObject(emergencyCall)}");

            if (!ModelState.IsValid)
            {
                ModelState.Remove("Patient");
                ModelState.Remove("NearestHospital");
                ModelState.Remove("Ambulance");  // Add this line
                if (!ModelState.IsValid)
                {
                    // Log if the ModelState is not valid
                    Console.WriteLine("ModelState is not valid:");
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            Console.WriteLine($" - Error: {error.ErrorMessage}");
                        }
                    }

                    return Json(new { success = false });
                }
            }

            _context.Entry(emergencyCall).State = EntityState.Added;
            try
            {
                // Add the EmergencyCall object to your database and save changes.
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log any exception that is thrown
                Console.WriteLine($"Exception occurred: {ex}");
                return Json(new { success = false });
            }
        }

        // PUT: Ambulances/FinishTask/5
        [HttpPut]
        public async Task<IActionResult> FinishTask(int id) // the id here represents the AmbulanceId
        {
            var ambulance = await _context.Ambulances.FindAsync(id);
            if (ambulance == null)
            {
                return NotFound();
            }

            // Make the ambulance available again
            ambulance.IsAvailable = true;

            // Clear the current emergency call
            ambulance.CurrentEmergencyCallId = null;

            // Update the ambulance in the context and save changes.
            _context.Update(ambulance);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
