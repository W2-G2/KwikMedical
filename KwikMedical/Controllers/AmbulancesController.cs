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
}
