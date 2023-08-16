using KwikMedical.Data;
using KwikMedical.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
            return View(await _context.Ambulances.Include(a => a.Hospital).ToListAsync());
        }

        // GET: Ambulances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ambulance = await _context.Ambulances
                .Include(a => a.Hospital)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ambulance == null)
            {
                return NotFound();
            }

            return View(ambulance);
        }

        // GET: Ambulances/UpdateStatus/5
        public async Task<IActionResult> UpdateStatus(int? id)
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

            ambulance.IsAvailable = !ambulance.IsAvailable; // Toggle the status
            _context.Update(ambulance);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Ambulances/SendMedicalRecords/5
        public async Task<IActionResult> SendMedicalRecords(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ambulance = await _context.Ambulances
                .Include(a => a.CurrentEmergencyCall)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (ambulance == null || ambulance.CurrentEmergencyCall == null)
            {
                return NotFound();
            }

            var medicalRecord = await _context.MedicalRecords
                .FirstOrDefaultAsync(m => m.PatientId == ambulance.CurrentEmergencyCall.PatientId);

            if (medicalRecord == null)
            {
                return NotFound();
            }

            // Logic to send medical records to the ambulance
            // This can be simulated for now as sending a notification or message

            return RedirectToAction(nameof(Index));
        }

        // GET: Ambulances/Dashboard/5
        public async Task<IActionResult> Dashboard(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ambulance = await _context.Ambulances
                .Include(a => a.CurrentEmergencyCall)
                .ThenInclude(e => e.Patient)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (ambulance == null || ambulance.CurrentEmergencyCallId == null)
            {
                return NotFound();
            }

            var medicalRecord = await _context.MedicalRecords.FirstOrDefaultAsync(m => m.PatientId == ambulance.CurrentEmergencyCall.PatientId);

            // Pass both the ambulance and medical record to the view
            var viewModel = new AmbulanceDashboardViewModel
            {
                Ambulance = ambulance,
                MedicalRecord = medicalRecord
            };

            return View(viewModel);
        }

    }
}
