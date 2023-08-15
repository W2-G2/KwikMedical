using KwikMedical.Data;
using KwikMedical.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace KwikMedical.Controllers
{
    public class GPSController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GPSController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult UpdateLocation(int ambulanceId, string newLocation)
        {
            var ambulance = _context.Ambulances.FirstOrDefault(a => a.Id == ambulanceId);
            if (ambulance != null)
            {
                ambulance.Location = newLocation;
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
