using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KwikMedical.Data;
using KwikMedical.Models;

namespace KwikMedical.Controllers;

public class MedicalRecordsController : Controller
{
    private readonly ApplicationDbContext _context;

    public MedicalRecordsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: MedicalRecords
    public async Task<IActionResult> Index()
    {
        return View(await _context.MedicalRecords.Include(m => m.Patient).ToListAsync());
    }

    // GET: MedicalRecords/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var medicalRecord = await _context.MedicalRecords
            .Include(m => m.Patient)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (medicalRecord == null)
        {
            return NotFound();
        }

        return View(medicalRecord);
    }

    // GET: MedicalRecords/Create
    public IActionResult Create()
    {
        ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Id");
        return View();
    }

    // POST: MedicalRecords/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,LaboratoryReports,TelephoneCalls,Xrays,Letters,PrescriptionCharts,ClinicalNotes,PatientId")] MedicalRecord medicalRecord)
    {
        if (ModelState.IsValid)
        {
            _context.Add(medicalRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Id", medicalRecord.PatientId);
        return View(medicalRecord);
    }

    // GET: MedicalRecords/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var medicalRecord = await _context.MedicalRecords.FindAsync(id);
        if (medicalRecord == null)
        {
            return NotFound();
        }
        ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Id", medicalRecord.PatientId);
        return View(medicalRecord);
    }

    // POST: MedicalRecords/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,LaboratoryReports,TelephoneCalls,Xrays,Letters,PrescriptionCharts,ClinicalNotes,PatientId")] MedicalRecord medicalRecord)
    {
        if (id != medicalRecord.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(medicalRecord);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicalRecordExists(medicalRecord.Id))
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
        ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Id", medicalRecord.PatientId);
        return View(medicalRecord);
    }

    // GET: MedicalRecords/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var medicalRecord = await _context.MedicalRecords
            .Include(m => m.Patient)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (medicalRecord == null)
        {
            return NotFound();
        }

        return View(medicalRecord);
    }

    // POST: MedicalRecords/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var medicalRecord = await _context.MedicalRecords.FindAsync(id);
        _context.MedicalRecords.Remove(medicalRecord);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MedicalRecordExists(int id)
    {
        return _context.MedicalRecords.Any(e => e.Id == id);
    }

    [HttpGet]
    public async Task<IActionResult> UpdateMedicalRecords(int ambulanceId)
    {
        var ambulance = await _context.Ambulances.FindAsync(ambulanceId);
        if (ambulance == null)
        {
            return NotFound();
        }

        var emergencyCall = await _context.EmergencyCalls.FindAsync(ambulance.CurrentEmergencyCallId);
        if (emergencyCall == null)
        {
            return NotFound();
        }

        var patient = await _context.Patients.FindAsync(emergencyCall.PatientId);
        if (patient == null)
        {
            return NotFound();
        }

        var medicalRecord = await _context.MedicalRecords.Where(m => m.PatientId == patient.Id).FirstOrDefaultAsync();
        if (medicalRecord == null)
        {
            return NotFound();
        }

        return Json(medicalRecord);
    }
}
