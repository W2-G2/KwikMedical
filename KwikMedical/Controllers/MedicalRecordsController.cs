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
        // Set default values for MedicalRecord fields if they are null or empty
        if (string.IsNullOrEmpty(medicalRecord.LaboratoryReports)) medicalRecord.LaboratoryReports = "N/A";
        if (string.IsNullOrEmpty(medicalRecord.TelephoneCalls)) medicalRecord.TelephoneCalls = "N/A";
        if (string.IsNullOrEmpty(medicalRecord.Xrays)) medicalRecord.Xrays = "N/A";
        if (string.IsNullOrEmpty(medicalRecord.Letters)) medicalRecord.Letters = "N/A";
        if (string.IsNullOrEmpty(medicalRecord.PrescriptionCharts)) medicalRecord.PrescriptionCharts = "N/A";
        if (string.IsNullOrEmpty(medicalRecord.ClinicalNotes)) medicalRecord.ClinicalNotes = "N/A";

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,LaboratoryReports,TelephoneCalls,Xrays,Letters,PrescriptionCharts,ClinicalNotes,PatientId")] MedicalRecord medicalRecord)
    {
        if (id != medicalRecord.Id)
        {
            return NotFound();
        }

        // Setting default values if not provided
        medicalRecord.LaboratoryReports = medicalRecord.LaboratoryReports ?? "Default Laboratory Reports";
        medicalRecord.TelephoneCalls = medicalRecord.TelephoneCalls ?? "Default Telephone Calls";
        medicalRecord.Xrays = medicalRecord.Xrays ?? "Default Xrays";
        medicalRecord.Letters = medicalRecord.Letters ?? "Default Letters";
        medicalRecord.PrescriptionCharts = medicalRecord.PrescriptionCharts ?? "Default Prescription Charts";
        medicalRecord.ClinicalNotes = medicalRecord.ClinicalNotes ?? "Default Clinical Notes";

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


    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var medicalRecord = _context.MedicalRecords.Find(id);
        _context.MedicalRecords.Remove(medicalRecord);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

}
