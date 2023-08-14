using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KwikMedical.Data;
using KwikMedical.Models;

namespace KwikMedical.Controllers;

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
        return View(await _context.EmergencyCalls.Include(e => e.Patient).ToListAsync());
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
            var availableAmbulance = await _context.Ambulances
                .Where(a => a.CurrentCity == emergencyCall.EmergencyCity && a.IsAvailable == true)
                .FirstOrDefaultAsync();

            if (availableAmbulance != null)
            {
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
        ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "NHSNumber", emergencyCall.PatientId);
        return View(emergencyCall);
    }

    // GET: EmergencyCalls/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
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
        var emergencyCall = await _context.EmergencyCalls.FindAsync(id);
        _context.EmergencyCalls.Remove(emergencyCall);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool EmergencyCallExists(int id)
    {
        return _context.EmergencyCalls.Any(e => e.Id == id);
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
        if (emergencyCall == null)
        {
            return Json(new { success = false });
        }

        var hospitalsInCity = _context.Hospitals.Where(h => h.City == emergencyCall.EmergencyCity).ToList();
        if (hospitalsInCity.Count == 0)
        {
            return Json(new { success = false });
        }

        var random = new Random();
        int index = random.Next(hospitalsInCity.Count);
        emergencyCall.NearestHospital = hospitalsInCity[index].Name;

        var patient = _context.Patients.Find(emergencyCall.PatientId);
        if (patient == null)
        {
            return Json(new { success = false });
        }

        emergencyCall.PatientId = patient.Id;

        if (!ModelState.IsValid)
        {
            ModelState.Remove("Patient");
            ModelState.Remove("NearestHospital");
            ModelState.Remove("Ambulance");
            if (!ModelState.IsValid)
            {
                return Json(new { success = false });
            }
        }

        _context.Entry(emergencyCall).State = EntityState.Added;
        try
        {
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        catch (Exception)
        {
            return Json(new { success = false });
        }
    }

    [HttpPut]
    public async Task<IActionResult> FinishTask(int id)
    {
        var ambulance = await _context.Ambulances.FindAsync(id);
        if (ambulance == null)
        {
            return NotFound();
        }

        ambulance.IsAvailable = true;
        ambulance.CurrentEmergencyCallId = null;

        _context.Update(ambulance);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
