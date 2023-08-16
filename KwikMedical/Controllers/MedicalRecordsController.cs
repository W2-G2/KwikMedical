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
        var medicalRecords = _context.MedicalRecords.Include(m => m.Patient);
        return View(await medicalRecords.ToListAsync());
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

    public IActionResult Create()
    {
        ViewBag.Patients = new SelectList(_context.Patients, "Id", "FullName");
        return View();
    }

    [HttpPost]
    public IActionResult Create(MedicalRecord medicalRecord)
    {
        if (ModelState.IsValid)
        {
            var patient = _context.Patients.Find(medicalRecord.PatientId);
            if (patient == null)
            {
                ModelState.AddModelError("", "Patient not found in the database.");
                ViewBag.Patients = new SelectList(_context.Patients, "Id", "FullName");
                return View(medicalRecord);
            }

            _context.MedicalRecords.Add(medicalRecord);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Patients = new SelectList(_context.Patients, "Id", "FullName", medicalRecord.PatientId);
        return View(medicalRecord);
    }

    public IActionResult Edit(int id)
    {
        var medicalRecord = _context.MedicalRecords.Find(id);
        if (medicalRecord == null)
        {
            return NotFound();
        }
        ViewBag.Patients = new SelectList(_context.Patients, "Id", "FullName", medicalRecord.PatientId);
        return View(medicalRecord);
    }

    [HttpPost]
    public IActionResult Edit(int id, MedicalRecord medicalRecord)
    {
        if (id != medicalRecord.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _context.Update(medicalRecord);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Patients = new SelectList(_context.Patients, "Id", "FullName", medicalRecord.PatientId);
        return View(medicalRecord);
    }

    public IActionResult Delete(int id)
    {
        var medicalRecord = _context.MedicalRecords.Find(id);
        if (medicalRecord == null)
        {
            return NotFound();
        }
        return View(medicalRecord);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var medicalRecord = _context.MedicalRecords.Find(id);
        _context.MedicalRecords.Remove(medicalRecord);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

}
