using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KwikMedical.Data;
using KwikMedical.Models;

namespace KwikMedical.Controllers;

public class HospitalsController : Controller
{
    private readonly ApplicationDbContext _context;

    public HospitalsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Hospitals
    public async Task<IActionResult> Index()
    {
        return View(await _context.Hospitals.ToListAsync());
    }

    // GET: Hospitals/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var hospital = await _context.Hospitals.FirstOrDefaultAsync(m => m.Id == id);
        if (hospital == null)
        {
            return NotFound();
        }

        return View(hospital);
    }

    // GET: Hospitals/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Hospitals/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,City")] Hospital hospital)
    {
        if (ModelState.IsValid)
        {
            _context.Add(hospital);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(hospital);
    }

    // GET: Hospitals/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var hospital = await _context.Hospitals.FindAsync(id);
        if (hospital == null)
        {
            return NotFound();
        }
        return View(hospital);
    }

    // POST: Hospitals/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,City")] Hospital hospital)
    {
        if (id != hospital.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(hospital);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HospitalExists(hospital.Id))
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
        return View(hospital);
    }

    // GET: Hospitals/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var hospital = await _context.Hospitals.FirstOrDefaultAsync(m => m.Id == id);
        if (hospital == null)
        {
            return NotFound();
        }

        return View(hospital);
    }

    // POST: Hospitals/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var hospital = await _context.Hospitals.FindAsync(id);
        _context.Hospitals.Remove(hospital);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool HospitalExists(int id)
    {
        return _context.Hospitals.Any(e => e.Id == id);
    }
}
