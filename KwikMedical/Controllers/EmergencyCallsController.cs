using KwikMedical.Data;
using KwikMedical.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
        var emergencyCalls = _context.EmergencyCalls.Include(e => e.Patient);
        return View(await emergencyCalls.ToListAsync());
    }

    // GET: EmergencyCalls/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var emergencyCall = await _context.EmergencyCalls
            .Include(e => e.Patient)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (emergencyCall == null)
        {
            return NotFound();
        }

        return View(emergencyCall);
    }

    // Action to fetch patient details based on NHS number
    [HttpGet]
    public IActionResult GetPatientDetails(string nhsNumber)
    {
        var patient = _context.Patients.FirstOrDefault(p => p.NHSNumber == nhsNumber);
        if (patient == null)
        {
            return NotFound();
        }
        return Json(patient);
    }

    [HttpPost]
    public IActionResult Create(EmergencyCall emergencyCall, string NHSNumber)
    {
        // Check if a patient with the entered NHS number exists
        var patient = _context.Patients.FirstOrDefault(p => p.NHSNumber == NHSNumber);
        if (patient == null)
        {
            TempData["Message"] = "Patient with the provided NHS number doesn't exist. Please add the patient.";
            return RedirectToAction("Create", "Patients");
        }

        // If patient exists, set the PatientId for the emergency call
        emergencyCall.PatientId = patient.Id;

        // Determine the best way to help the patient.
        var medicalCondition = emergencyCall.MedicalCondition;

        // Generate an ambulance rescue request to one of the regional hospitals.
        var availableAmbulance = _context.Ambulances
            .FirstOrDefault(a => a.IsAvailable && a.CurrentCity == patient.City);
        if (availableAmbulance == null)
        {
            ModelState.AddModelError("", "No available ambulances in the patient's city.");
            ViewBag.Patients = new SelectList(_context.Patients, "Id", "FullName");
            return View(emergencyCall);
        }
        availableAmbulance.IsAvailable = false; // Mark the ambulance as occupied
        _context.Update(availableAmbulance);

        emergencyCall.AmbulanceId = availableAmbulance.Id;

        // Extract the patient’s medical records and simulate sending them to an ambulance's smartphone.
        var medicalRecords = _context.MedicalRecords.Where(m => m.PatientId == emergencyCall.PatientId).ToList();

        _context.EmergencyCalls.Add(emergencyCall);
        _context.SaveChanges();

        // Check if there's any error with the database save operation
        if (_context.Entry(emergencyCall).State == EntityState.Unchanged)
        {
            ViewBag.Patients = new SelectList(_context.Patients, "Id", "FullName", emergencyCall.PatientId);
            return View(emergencyCall);
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Create()
    {
        ViewBag.Patients = new SelectList(_context.Patients, "Id", "FullName");
        return View();
    }

    public IActionResult Edit(int id)
    {
        var emergencyCall = _context.EmergencyCalls.Find(id);
        if (emergencyCall == null)
        {
            return NotFound();
        }
        ViewBag.Patients = new SelectList(_context.Patients, "Id", "FullName", emergencyCall.PatientId);
        return View(emergencyCall);
    }

    [HttpPost]
    public IActionResult Edit(int id, EmergencyCall emergencyCall)
    {
        if (id != emergencyCall.Id)
        {
            return NotFound();
        }

        _context.Update(emergencyCall);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var emergencyCall = _context.EmergencyCalls.Find(id);
        if (emergencyCall == null)
        {
            return NotFound();
        }
        return View(emergencyCall);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var emergencyCall = _context.EmergencyCalls.Find(id);
        _context.EmergencyCalls.Remove(emergencyCall);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
