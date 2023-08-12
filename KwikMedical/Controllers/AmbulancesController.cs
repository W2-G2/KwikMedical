using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KwikMedical.Data;
using KwikMedical.Models;

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
            var ambulances = await _context.Ambulances
                .Include(a => a.CurrentEmergencyCall) // Eager load the current emergency call
                .Include(a => a.Hospital) // Eager load the hospital
                .ToListAsync();


            foreach (var ambulance in ambulances)
            {
                // Check if the ambulance is available and does not have a current emergency call
                if (ambulance.IsAvailable && ambulance.CurrentEmergencyCall == null)
                {
                    // Find an unaccepted emergency call in the same city as the ambulance
                    var emergencyCall = await _context.EmergencyCalls
                        .Where(ec => !ec.IsAccepted && ec.EmergencyCity == ambulance.CurrentCity)
                        .FirstOrDefaultAsync();

                    if (emergencyCall != null)
                    {
                        // Assign the emergency call to the ambulance
                        ambulance.CurrentEmergencyCall = emergencyCall;
                        emergencyCall.AmbulanceId = ambulance.Id;
                        ambulance.IsAvailable = false;

                        _context.Update(ambulance);
                        _context.Update(emergencyCall);
                    }
                }
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            return View(ambulances);
        }

        // GET: Ambulances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Hospitals == null)
            {
                return NotFound();
            }

            var hospital = await _context.Hospitals
                .Include(h => h.Ambulances)  // Add this line to include the hospital's ambulances
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hospital == null)
            {
                return NotFound();
            }

            return View(hospital);
        }

        // GET: Ambulances/Create
        public IActionResult Create()
        {
            ViewBag.Hospitals = new SelectList(_context.Hospitals, "Id", "Name");
            return View();
        }


        // POST: Ambulances/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,IsAvailable,HospitalId,CurrentCity,Hospital")] Ambulance ambulance)
        {
            // find the selected hospital
            var selectedHospital = await _context.Hospitals.FindAsync(ambulance.HospitalId);

            // if the selected hospital is found, assign the city
            if (selectedHospital != null)
            {
                ambulance.CurrentCity = selectedHospital.City;
            }
            // Add the ambulance to the context and save changes
            _context.Add(ambulance);
            await _context.SaveChangesAsync();

            return View(ambulance);
        }

        // GET: Ambulances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Ambulances == null)
            {
                return NotFound();
            }

            var ambulance = await _context.Ambulances.FindAsync(id);
            if (ambulance == null)
            {
                return NotFound();
            }
            return View(ambulance);
        }

        // POST: Ambulances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,IsAvailable")] Ambulance ambulance)
        {
            if (id != ambulance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ambulance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AmbulanceExists(ambulance.Id))
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
            return View(ambulance);
        }

        // GET: Ambulances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Ambulances == null)
            {
                return NotFound();
            }

            var ambulance = await _context.Ambulances
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ambulance == null)
            {
                return NotFound();
            }

            return View(ambulance);
        }

        // POST: Ambulances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Ambulances == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Ambulances'  is null.");
            }
            var ambulance = await _context.Ambulances.FindAsync(id);
            if (ambulance != null)
            {
                _context.Ambulances.Remove(ambulance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AmbulanceExists(int id)
        {
            return (_context.Ambulances?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> AcceptEmergencyCall(int id)
        {
            var emergencyCall = await _context.EmergencyCalls.FindAsync(id);
            if (emergencyCall == null)
            {
                return NotFound();
            }

            emergencyCall.IsAccepted = true;
            _context.Update(emergencyCall);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmergencyCallDetails(int id, string details)
        {
            var emergencyCall = await _context.EmergencyCalls.FindAsync(id);
            if (emergencyCall == null)
            {
                return NotFound();
            }

            emergencyCall.EmergencyDetails = details;
            _context.Update(emergencyCall);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMedicalRecord(int patientId, string clinicalNotes)
        {
            var medicalRecord = await _context.MedicalRecords
                .FirstOrDefaultAsync(mr => mr.PatientId == patientId);

            if (medicalRecord == null)
            {
                return NotFound();
            }

            medicalRecord.ClinicalNotes = clinicalNotes;
            _context.Update(medicalRecord);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> GetUnacceptedEmergencyCalls()
        {
            var unacceptedCalls = await _context.EmergencyCalls
                .Where(ec => !ec.IsAccepted)
                .ToListAsync();

            return PartialView("_UnacceptedCallsPartial", unacceptedCalls);
        }

        // PUT: Ambulances/AcceptCall/5
        [HttpPost]
        public async Task<IActionResult> AcceptCall(int? id) // the id here represents the AmbulanceId
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

            var emergencyCall = await _context.EmergencyCalls.FindAsync(ambulance.CurrentEmergencyCallId);
            if (emergencyCall == null)
            {
                return NotFound();
            }

            // Mark the emergency call as accepted.
            emergencyCall.IsAccepted = true;
            ambulance.IsAvailable = false;

            // Update the emergency call and ambulance in the context and save changes.
            _context.Update(emergencyCall);
            _context.Update(ambulance);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
