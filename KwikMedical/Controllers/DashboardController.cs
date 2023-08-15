using KwikMedical.Data;
using KwikMedical.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace KwikMedical.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var ongoingEmergencies = _context.EmergencyCalls.Where(e => e.IsAccepted == false).ToList();
            var availableAmbulances = _context.Ambulances.Where(a => a.IsAvailable == true).ToList();

            return View((ongoingEmergencies, availableAmbulances));
        }
    }
}
