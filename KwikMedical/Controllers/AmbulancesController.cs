using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KwikMedical.Data;
using KwikMedical.Models;

namespace KwikMedical.Controllers;

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
            .Include(a => a.CurrentEmergencyCall)
            .Include(a => a.Hospital)
            .ToListAsync();

        var unacceptedCalls = await _context.EmergencyCalls
            .Where(ec => !ec.IsAccepted)
            .ToListAsync();

        ViewData["UnacceptedCalls"] = unacceptedCalls;

        return View(ambulances);
    }


    // GET: Ambulances/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var ambulance = await _context.Ambulances
            .Include(a => a.CurrentEmergencyCall)
            .Include(a => a.Hospital)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (ambulance == null)
        {
            return NotFound();
        }

        return View(ambulance);
    }

    // GET: Ambulances/Create
    public IActionResult Create()
    {
        ViewData["HospitalId"] = new SelectList(_context.Hospitals, "Id", "Name");
        return View();
    }

    // POST: Ambulances/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Code,IsAvailable,HospitalId,CurrentCity")] Ambulance ambulance)
    {
        var selectedHospital = await _context.Hospitals.FindAsync(ambulance.HospitalId);
        if (selectedHospital != null)
        {
            ambulance.CurrentCity = selectedHospital.City;
        }

        if (ModelState.IsValid)
        {
            _context.Add(ambulance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["HospitalId"] = new SelectList(_context.Hospitals, "Id", "Name", ambulance.HospitalId);
        return View(ambulance);
    }

    // GET: Ambulances/Edit/5
    public async Task<IActionResult> Edit(int? id)
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

        ViewData["HospitalId"] = new SelectList(_context.Hospitals, "Id", "Name", ambulance.HospitalId);
        return View(ambulance);
    }

    // POST: Ambulances/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Code,IsAvailable,HospitalId,CurrentCity")] Ambulance ambulance)
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

        ViewData["HospitalId"] = new SelectList(_context.Hospitals, "Id", "Name", ambulance.HospitalId);
        return View(ambulance);
    }

    // GET: Ambulances/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
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
        var ambulance = await _context.Ambulances.FindAsync(id);
        _context.Ambulances.Remove(ambulance);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AmbulanceExists(int id)
    {
        return _context.Ambulances.Any(e => e.Id == id);
    }

    // Accept an emergency call
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

    // Update emergency call details
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

    // Update medical record
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

    // Get unaccepted emergency calls
    public async Task<IActionResult> GetUnacceptedEmergencyCalls()
    {
        var unacceptedCalls = await _context.EmergencyCalls
            .Where(ec => !ec.IsAccepted)
            .ToListAsync();

        return PartialView("_UnacceptedCallsPartial", unacceptedCalls);
    }

    // Accept a call
    [HttpPost]
    public async Task<IActionResult> AcceptCall(int? id)
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

        emergencyCall.IsAccepted = true;
        ambulance.IsAvailable = false;

        _context.Update(emergencyCall);
        _context.Update(ambulance);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult AutoAssignCall(string emergencyCity)
    {
        // Retrieve the oldest unassigned emergency call for the given city
        var emergencyCall = _context.EmergencyCalls
                                    .Where(e => e.EmergencyCity == emergencyCity && e.AmbulanceId == null)
                                    .OrderBy(e => e.Id)
                                    .FirstOrDefault();

        if (emergencyCall == null)
        {
            return Json(new { success = false, message = "No unassigned calls for the given city." });
        }

        // Find the next available ambulance in the same city
        var availableAmbulance = _context.Ambulances
                                        .Where(a => a.CurrentCity == emergencyCity && a.IsAvailable == true)
                                        .FirstOrDefault();

        if (availableAmbulance == null)
        {
            return Json(new { success = false, message = "No available ambulances in the given city." });
        }

        // Assign the emergency call to the ambulance
        emergencyCall.AmbulanceId = availableAmbulance.Id;
        availableAmbulance.IsAvailable = false;
        availableAmbulance.CurrentEmergencyCallId = emergencyCall.Id;

        _context.SaveChanges();

        return Json(new { success = true, message = "Emergency call assigned successfully." });
    }

    [HttpPost]
    public IActionResult AssignCall(int emergencyCallId, int ambulanceId)
    {
        var emergencyCall = _context.EmergencyCalls.Find(emergencyCallId);
        var ambulance = _context.Ambulances.Find(ambulanceId);

        if (emergencyCall == null || ambulance == null)
        {
            return Json(new { success = false, message = "Invalid emergency call or ambulance ID." });
        }

        // Assign the emergency call to the ambulance
        emergencyCall.AmbulanceId = ambulance.Id;
        ambulance.IsAvailable = false;
        ambulance.CurrentEmergencyCallId = emergencyCall.Id;

        _context.SaveChanges();

        return Json(new { success = true, message = "Emergency call assigned successfully." });
    }

    [HttpPost]
    public IActionResult UpdateMedicalRecord(int emergencyCallId, Patient updatedPatient, MedicalRecord updatedRecord)
    {
        var emergencyCall = _context.EmergencyCalls
                            .Include(e => e.Patient)
                            .ThenInclude(p => p.MedicalRecord)
                            .FirstOrDefault(e => e.Id == emergencyCallId);

        if (emergencyCall == null)
        {
            return Json(new { success = false, message = "Invalid emergency call ID." });
        }

        // Update patient and medical record details
        emergencyCall.Patient.FirstName = updatedPatient.FirstName;
        emergencyCall.Patient.LastName = updatedPatient.LastName;
        emergencyCall.Patient.NHSNumber = updatedPatient.NHSNumber;
        emergencyCall.Patient.Address = updatedPatient.Address;
        emergencyCall.Patient.City = updatedPatient.City;
        emergencyCall.Patient.Postcode = updatedPatient.Postcode;

        emergencyCall.Patient.MedicalRecord.LaboratoryReports = updatedRecord.LaboratoryReports;
        emergencyCall.Patient.MedicalRecord.TelephoneCalls = updatedRecord.TelephoneCalls;
        emergencyCall.Patient.MedicalRecord.Xrays = updatedRecord.Xrays;
        emergencyCall.Patient.MedicalRecord.Letters = updatedRecord.Letters;
        emergencyCall.Patient.MedicalRecord.PrescriptionCharts = updatedRecord.PrescriptionCharts;
        emergencyCall.Patient.MedicalRecord.ClinicalNotes = updatedRecord.ClinicalNotes;

        _context.SaveChanges();

        return Json(new { success = true, message = "Medical record updated successfully." });
    }

    [HttpPost]
    public IActionResult ConcludeEmergencyCall(int emergencyCallId)
    {
        var emergencyCall = _context.EmergencyCalls.Find(emergencyCallId);

        if (emergencyCall == null)
        {
            return Json(new { success = false, message = "Invalid emergency call ID." });
        }

        _context.EmergencyCalls.Remove(emergencyCall);
        _context.SaveChanges();

        return Json(new { success = true, message = "Emergency call concluded successfully." });
    }

}
