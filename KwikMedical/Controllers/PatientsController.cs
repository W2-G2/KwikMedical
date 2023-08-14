using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KwikMedical.Data;
using KwikMedical.Models;

namespace KwikMedical.Controllers;

public class PatientsController : Controller
{
    private readonly ApplicationDbContext _context;

    public PatientsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Patients
    public async Task<IActionResult> Index()
    {
        return View(await _context.Patients.ToListAsync());
    }

    // GET: Patients/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var patient = await _context.Patients
            .FirstOrDefaultAsync(m => m.Id == id);
        if (patient == null)
        {
            return NotFound();
        }

        return View(patient);
    }

    // GET: Patients/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Patients/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,NHSNumber,Address,City,Postcode")] Patient patient)
    {
        if (ModelState.IsValid)
        {
            _context.Add(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(patient);
    }

    // GET: Patients/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }
        return View(patient);
    }

    // POST: Patients/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,NHSNumber,Address,City,Postcode")] Patient patient)
    {
        if (id != patient.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(patient);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(patient.Id))
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
        return View(patient);
    }

    // GET: Patients/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var patient = await _context.Patients
            .FirstOrDefaultAsync(m => m.Id == id);
        if (patient == null)
        {
            return NotFound();
        }

        return View(patient);
    }

    // POST: Patients/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PatientExists(int id)
    {
        return _context.Patients.Any(e => e.Id == id);
    }

    [HttpPost]
    public async Task<IActionResult> FinalizePatient(int ambulanceId, string firstName, string lastName, string nhsNumber, string address, string city, string postcode)
    {
        var ambulance = await _context.Ambulances.FindAsync(ambulanceId);
        if (ambulance == null)
        {
            return NotFound();
        }

        var patient = new Patient
        {
            FirstName = firstName,
            LastName = lastName,
            NHSNumber = nhsNumber,
            Address = address,
            City = city,
            Postcode = postcode
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        var emergencyCall = await _context.EmergencyCalls.FindAsync(ambulance.CurrentEmergencyCallId);
        if (emergencyCall != null)
        {
            _context.EmergencyCalls.Remove(emergencyCall);
        }

        ambulance.CurrentEmergencyCallId = null;
        ambulance.IsAvailable = true;

        _context.Update(ambulance);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Ambulances");
    }
}
