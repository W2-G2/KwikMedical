using KwikMedical.Data;
using KwikMedical.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KwikMedical.Controllers
{
    public class EmergencyCallsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmergencyCallsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult CreateEmergencyCall(EmergencyCall emergencyCall)
        {
            _context.EmergencyCalls.Add(emergencyCall);
            _context.SaveChanges();

            var medicalRecords = _context.MedicalRecords.FirstOrDefault(m => m.PatientId == emergencyCall.PatientId);
            if (medicalRecords != null)
            {
                // Simulate sending medical records to the ambulance
                SendMedicalRecordsToAmbulance(medicalRecords);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        private void SendMedicalRecordsToAmbulance(MedicalRecord medicalRecords)
        {
            // Assuming the ambulance has an associated user or device ID
            var ambulance = _context.Ambulances.FirstOrDefault(a => a.CurrentEmergencyCallId == medicalRecords.PatientId);
            if (ambulance != null)
            {
                // Here, you'd typically send the medical records to the ambulance's associated device.
                // For this prototype, we'll store the medical record ID in the ambulance's record as a simulation.
                ambulance.MedicalRecordId = medicalRecords.Id;
                _context.SaveChanges();
            }
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

        // GET: EmergencyCalls/Create
        public IActionResult Create()
        {
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "FullName"); // Assuming you have a FullName property in Patient model
            return View();
        }

        // POST: EmergencyCalls/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmergencyDetails,EmergencyCity,...")] EmergencyCall emergencyCall)
        {
            if (ModelState.IsValid)
            {
                // Find an available ambulance
                var availableAmbulance = await _context.Ambulances.FirstOrDefaultAsync(a => a.IsAvailable == true);
                double emergencyLatitude = 56.4907;  // Example latitude
                double emergencyLongitude = -4.2026; // Example longitude

                if (availableAmbulance != null)
                {
                    availableAmbulance.IsAvailable = false; // Mark the ambulance as unavailable
                    emergencyCall.AmbulanceId = availableAmbulance.Id; // Assign the ambulance to the emergency call

                    // Send medical records to the ambulance
                    var medicalRecord = await _context.MedicalRecords.FirstOrDefaultAsync(m => m.PatientId == emergencyCall.PatientId);
                    if (medicalRecord != null)
                    {
                        // Logic to send medical records to the ambulance. 
                        // This can be a simulated action, such as updating a field in the ambulance record indicating that medical records have been sent.
                        availableAmbulance.CurrentEmergencyCallId = emergencyCall.Id;
                    }

                    // Assign a random hospital as the nearest hospital (for demo purposes)
                    var hospitals = await _context.Hospitals.ToListAsync();
                    if (hospitals.Any())
                    {
                        var randomHospital = hospitals[new Random().Next(hospitals.Count)];
                        emergencyCall.NearestHospital = randomHospital.Name;
                    }
                    Task.Run(() => SimulateAmbulanceMovement(availableAmbulance, emergencyCall, emergencyLatitude, emergencyLongitude));

                    // Calculate ETA based on simulated distance and speed
                    double distance = CalculateDistance(availableAmbulance.Latitude, availableAmbulance.Longitude, emergencyLatitude, emergencyLongitude);
                    double speed = 60.0; // 60 km/h for simplicity
                    double timeHours = distance / speed;
                    emergencyCall.EstimatedArrivalTime = DateTime.Now.AddHours(timeHours);

                    _context.Update(emergencyCall);
                    await _context.SaveChangesAsync();

                    // Notify the nearest hospital
                    NotifyHospital(emergencyCall.NearestHospital, emergencyCall);

                    // Assign the nearest available ambulance based on city
                    var nearestAmbulance = _context.Ambulances
                        .Where(a => a.IsAvailable && a.CurrentCity == emergencyCall.EmergencyCity)
                        .FirstOrDefault();

                    // If no ambulance is available in the same city, assign any available ambulance
                    if (nearestAmbulance == null)
                    {
                        nearestAmbulance = _context.Ambulances
                            .Where(a => a.IsAvailable)
                            .FirstOrDefault();
                    }

                    if (nearestAmbulance != null)
                    {
                        nearestAmbulance.IsAvailable = false;
                        nearestAmbulance.CurrentEmergencyCallId = emergencyCall.Id;
                        _context.Update(nearestAmbulance);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                _context.Add(emergencyCall);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Notify the nearest hospital
            NotifyHospital(emergencyCall.NearestHospital, emergencyCall);

            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "FullName", emergencyCall.PatientId);
            return View(emergencyCall);
        }

        private void NotifyHospital(string hospitalName, EmergencyCall emergencyCall)
        {
            // For this demo, we'll just log the notification and update the hospital's preparation status.
            var hospital = _context.Hospitals.FirstOrDefault(h => h.Name == hospitalName);
            if (hospital != null)
            {
                hospital.PreparationStatus = $"Preparing for patient: {emergencyCall.Patient.FirstName} {emergencyCall.Patient.LastName} with ETA: {emergencyCall.EstimatedArrivalTime}";
                _context.Update(hospital);
                _context.SaveChanges();
            }

            System.Diagnostics.Debug.WriteLine($"Notified {hospitalName} about incoming patient: {emergencyCall.Patient.FirstName} {emergencyCall.Patient.LastName} with ETA: {emergencyCall.EstimatedArrivalTime}");
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // For simplicity, we'll use the Pythagorean theorem to calculate the distance
            // This is not accurate for real-world scenarios, but it's sufficient for our simulation
            return Math.Sqrt(Math.Pow(lat2 - lat1, 2) + Math.Pow(lon2 - lon1, 2));
        }

        private void SimulateAmbulanceMovement(Ambulance ambulance, EmergencyCall emergencyCall, double emergencyLatitude, double emergencyLongitude)
        {
            while (Math.Abs(ambulance.Latitude - emergencyLatitude) > 0.01 ||
                   Math.Abs(ambulance.Longitude - emergencyLongitude) > 0.01)
            {
                // Update ambulance's latitude and longitude in small increments
                ambulance.Latitude += (emergencyLatitude - ambulance.Latitude) * 0.01;
                ambulance.Longitude += (emergencyLongitude - ambulance.Longitude) * 0.01;

                // Save changes to the database
                _context.Update(ambulance);
                _context.SaveChanges();

                // Wait for a short duration before the next update
                System.Threading.Thread.Sleep(1000); // Wait for 1 second
            }

            // Once the ambulance reaches the destination, update the emergency call status
            emergencyCall.EmergencyStatus = "Ambulance Arrived";
            _context.Update(emergencyCall);
            _context.SaveChanges();

            // Simulate ambulance return to base after a short delay
            System.Threading.Thread.Sleep(5000); // Wait for 5 seconds

            ambulance.IsAvailable = true; // Mark the ambulance as available again
            ambulance.CurrentEmergencyCallId = null; // Clear the current emergency call
            _context.Update(ambulance);
            _context.SaveChanges();
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {
            var emergencyCall = await _context.EmergencyCalls.FindAsync(id);
            if (emergencyCall == null)
            {
                return NotFound();
            }

            emergencyCall.IsCompleted = true;
            _context.Update(emergencyCall);

            var ambulance = await _context.Ambulances.FindAsync(emergencyCall.AmbulanceId);
            if (ambulance != null)
            {
                ambulance.IsAvailable = true;
                ambulance.CurrentEmergencyCallId = null;
                _context.Update(ambulance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
